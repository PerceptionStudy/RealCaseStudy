using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Linq;
using System.Collections.Generic;

public class MainScript : MonoBehaviour 
{
	public float molSize = 10;

	private int molCount= 0;
	private int reactionCounter = 0;

	private MolObject[] molObjects;	
	private GameObject previousSelectedObject = null;

	private GameObject collideBox;
	private GameObject rootObject;

	public static Vector3 BoxSize = new Vector3();

	List<int> speciesCount = new List<int>
	{
		100,					// 1-methy-NAM
		100,					// ADP-Ribose
		100,					// ATP
		100,					// NAD
		100,					// NAM
		100,					// NMN
		100,					// PPi
		100,					// PRPP
		100,					// SAH
		100,					// SAM
		25,						// NAMTP
		25,						// NMAT1
		25,						// NMNT
		25,						// PARP1
	};

	List<string> species = new List<string>
	{
		"1-methy-NAM",			// 0
		"ADP-Ribose",			// 1
		"ATP",					// 2
		"NAD",					// 3
		"NAM",					// 4
		"NMN",					// 5
		"PPi",					// 6
		"PRPP",					// 7
		"SAH",					// 8
		"SAM",					// 9
		"NAMTP",				// 10
		"NMAT1",				// 11
		"NMNT",					// 12
		"PARP1",				// 13
	};

	List<int> reactionEnzymes = new List<int>
	{
		10, 					// NAMTP 
		11,						// NMAT1
		12,						// NMNT
		13,						// PARP1
	};

	List<int[]> reactionSubstrates = new List<int[]>
	{
		new int[2] {4, 7},		// NAMTP 
		new int[2] {5, 2},		// NMAT1
		new int[2] {4, 9},		// NMNT
		new int[1] {3},			// PARP1
	};

	List<int[]> reactionProducts = new List<int[]>
	{
		new int[2] {5, 6},		// NAMTP 
		new int[2] {3, 6},		// NMAT1
		new int[2] {0, 8},		// NMNT
		new int[2] {4, 1},		// PARP1
	};

	public MolObject CreateMol(Transform parent, string name, int type)
	{
		Vector3 position = new Vector3 ((UnityEngine.Random.value - 0.5f) * MainScript.BoxSize.x, (UnityEngine.Random.value - 0.5f) * MainScript.BoxSize.y, (UnityEngine.Random.value - 0.5f) * MainScript.BoxSize.z);
		
		GameObject gameObject = Instantiate(Resources.Load("Prefabs/" + species[type]), position, UnityEngine.Random.rotation) as GameObject;		
		gameObject.name = name;
		gameObject.transform.parent = parent;
		gameObject.transform.localScale = new Vector3(molSize, molSize, molSize);
		
		MolObject molObject = gameObject.GetComponent<MolObject>();
		molObject.type = type;
		
		return molObject;
	}

	public void LoadScene()
	{
		// Destroy previous game objects
		if(molObjects != null)
		{
			foreach(MolObject obj in molObjects)
			{
				Destroy(obj.gameObject);
			}
		}

		molObjects = new MolObject[speciesCount.Sum()];

		molCount = 0;
		for (var i = 0; i < species.Count(); i++)
		{			
			for (var j = 0; j < speciesCount[i]; j++)
			{
				molObjects[molCount ++] = CreateMol(gameObject.transform, "Mol_" + molCount, i);
			}
		}
	}

	void Start () 
	{
		BoxSize.x = Screen.width;
		BoxSize.y = Screen.height;
		BoxSize.z = 200;
		
		collideBox = GameObject.Find("Box Object");
		collideBox.transform.localScale = new Vector3(BoxSize.x, BoxSize.y, BoxSize.z);

		rootObject = GameObject.Find("Root Object");

		int metaboliteLayer = LayerMask.NameToLayer("Metabolite");

		Physics.IgnoreLayerCollision(metaboliteLayer, metaboliteLayer);

		LoadScene();
	}

	void Update () 
	{
		Camera.main.orthographicSize = Screen.height * 0.5f;
		
		BoxSize.x = Screen.width;
		BoxSize.y = Screen.height;
		BoxSize.z = 200;
		
		collideBox.transform.localScale = new Vector3(BoxSize.x, BoxSize.y, BoxSize.z);

		if(Input.GetKeyDown (KeyCode.R))
		{
			LoadScene();
		}

		if(Input.GetKeyDown (KeyCode.N))
		{
			int reactionType = UnityEngine.Random.Range(0, 3);
			int enzymeType = reactionEnzymes[reactionType];

			MolObject[] availableElements = (from element in molObjects 
			                                 where element.reactionId == -1	&& element.type == enzymeType
			                                 orderby Guid.NewGuid()
			                                 select element).ToArray();

			if(availableElements.Count() > 0)
			{
				GameObject selectedObject = availableElements.First().gameObject;
				MolObject enzyme = selectedObject.GetComponent<MolObject>();

				if(reactionSubstrates[reactionType].Count() == 1)
				{
					int partnerType = reactionSubstrates[reactionType][0];
					
					int metaboliteLayer = LayerMask.NameToLayer("Metabolite");
					int enzymeLayer = LayerMask.NameToLayer("Enzyme");
					
					int radius = Math.Max(Screen.height, Screen.width) / 8;
					
					Collider[] results = Physics.OverlapSphere(selectedObject.transform.position, radius, 1 << metaboliteLayer | 1 << enzymeLayer);
					
					MolObject[] partners = (from element in results 
					                          where element.gameObject.GetComponent<MolObject>().type == partnerType && element.gameObject.GetComponent<MolObject>().reactionId == -1 
					                          orderby Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position)
					                          select element.gameObject.GetComponent<MolObject>()).ToArray();		
					
					if(partners.Count() > 0)
					{
						MolObject partner = partners.First();
						
						enzyme.reactionId = partner.reactionId = reactionCounter ++;
						enzyme.reactionType = partner.reactionType = reactionType;
						enzyme.reactionCat = partner.reactionCat = 0;
						
						enzyme.HighlightColor();
						partner.HighlightColor();			
						
						previousSelectedObject = selectedObject;
						
						print ("Start reaction: " + enzyme.reactionId + "of type: " + enzyme.reactionType);
					}
				}
				else if(reactionSubstrates[reactionType].Count() == 2)
				{
					int partnerType_1 = reactionSubstrates[reactionType][0];
					int partnerType_2 = reactionSubstrates[reactionType][1];

					int metaboliteLayer = LayerMask.NameToLayer("Metabolite");
					int enzymeLayer = LayerMask.NameToLayer("Enzyme");
										
					int radius = Math.Max(Screen.height, Screen.width) / 8;
					
					Collider[] results = Physics.OverlapSphere(selectedObject.transform.position, radius, 1 << metaboliteLayer | 1 << enzymeLayer);
					
					MolObject[] partners_1 = (from element in results 
					            				where element.gameObject.GetComponent<MolObject>().type == partnerType_1 && element.gameObject.GetComponent<MolObject>().reactionId == -1 
					            				orderby Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position)
					           					select element.gameObject.GetComponent<MolObject>()).ToArray();	

					MolObject[] partners_2 = (from element in results 
					                          where element.gameObject.GetComponent<MolObject>().type == partnerType_2 && element.gameObject.GetComponent<MolObject>().reactionId == -1 
					                          orderby Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position)
					                          select element.gameObject.GetComponent<MolObject>()).ToArray();	
					
					if(partners_1.Count() > 0 && partners_2.Count() > 0)
					{
						MolObject partner_1 = partners_1.First();
						MolObject partner_2 = partners_2.First();

						enzyme.reactionId = partner_1.reactionId = partner_2.reactionId = reactionCounter ++;
						enzyme.reactionType = partner_1.reactionType = partner_2.reactionType = reactionType;
						enzyme.reactionCat = partner_1.reactionCat = partner_2.reactionCat = 1;
						
						enzyme.HighlightColor();
						partner_1.HighlightColor();						
						partner_2.HighlightColor();
						
						previousSelectedObject = selectedObject;
						
						print ("Start reaction: " + enzyme.reactionId);
					}	
				}
			}				
		}
		
//		// Cast ray to gameObjects
//		if (Input.GetMouseButtonDown (0) && Camera.current!= null) 
//		{
//
//			Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
//			RaycastHit hit;
//			if (Physics.Raycast(ray, out hit)) 
//			{
//				Debug.Log ("Name = " + hit.collider.name);
//				Debug.Log ("Tag = " + hit.collider.tag);
//				Debug.Log ("Hit Point = " + hit.point);
//				Debug.Log ("Object position = " + hit.collider.gameObject.transform.position);
//				Debug.Log ("--------------");
//			}
//		}
	}

	//TODO: Update species count

	void FixedUpdate()
	{
		var reactingElements = (from molObject in molObjects 
		                        where molObject.reactionId != -1 && molObject.reactionCat == 0		 
		                        group molObject by molObject.reactionId into g
		                        select g);

		foreach(var g in reactingElements)
		{
			var l = g.OrderByDescending(x => x.type).ToArray();
			
			MolObject enzyme = l[0];
			MolObject mol = l[1];

			if(mol.isReacting)
			{
				if(mol.isBinding)
				{
					if(enzyme.Timeout(120))
					{
						print("End reaction " + mol.reactionId);
						
						mol.isFreezed = false;
						mol.isBinding = false;
						mol.rigidbody.isKinematic = false;
						
						mol.gameObject.transform.parent = enzyme.gameObject.transform.parent;		
						
						// Change type of elements
						int reactionType = enzyme.reactionType;
						int productType_1 = reactionProducts[reactionType][0];
						int productType_2 = reactionProducts[reactionType][1];
						
						mol.gameObject.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Meshes/" + species[productType_1]);
						mol.gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + species[productType_1]);
						mol.color = mol.gameObject.GetComponent<MeshRenderer>().material.color;
						mol.HighlightColor();
						mol.type = productType_1;

						// Create new molecules

						molCount ++;

						GameObject gameObject = Instantiate(Resources.Load("Prefabs/" + species[productType_2]), mol.transform.position, UnityEngine.Random.rotation) as GameObject;		
						gameObject.name = "Mol_" + molCount;
						gameObject.transform.parent = mol.transform.parent;
						gameObject.transform.localScale = new Vector3(molSize, molSize, molSize);

						print (mol.transform.position);

						MolObject newMol = gameObject.GetComponent<MolObject>();
						newMol.type = productType_2;
						//newMol.HighlightColor();

						Array.Resize(ref molObjects, molCount);
						molObjects[molCount-1] = newMol;

						enzyme.RestoreColor();
						mol.RestoreColor();
					}
				}
				else
				{
					Vector3 attractionForce = (enzyme.transform.position - mol.transform.position).normalized;
					mol.rigidbody.AddForce(-attractionForce * 250);
					
					if(enzyme.Timeout(60))
					{						
						Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol.gameObject.rigidbody.collider, false);					
						
						mol.isReacting = false;		
						
						enzyme.EndReaction();
						mol.EndReaction();
					}
				}
			}
			else
			{
				if(!mol.isReacting)
				{
					if(!mol.isBinding)
					{
						Vector3 attractionForce = (enzyme.transform.Find("Anchor").position - mol.transform.position).normalized;
						mol.rigidbody.AddForce(attractionForce * 200);
						
						float distance = Vector3.Distance(enzyme.transform.position, mol.transform.position);
						float collideDistance = (enzyme.gameObject.GetComponent<SphereCollider>().radius + mol.gameObject.GetComponent<SphereCollider>().radius) * molSize;
						
						if(distance <= collideDistance  + 1.0f)
						{
							print("Collision 1");
							
							if(mol.Timeout(0))
							{
								mol.isBinding = true;
								mol.isFreezed = true;
								Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol.gameObject.rigidbody.collider, true);								
							}
						}
					}
					else
					{
						Vector3 attractionForce = (enzyme.transform.Find("Anchor").position - mol.transform.position).normalized;
						mol.rigidbody.AddForce(attractionForce * 500);
						
						float distance = Vector3.Distance(enzyme.transform.Find("Anchor").position, mol.transform.position);
						
						if(distance <= 4.0f)
						{
							print("Binding 1");
							
							if(mol.Timeout(0))
							{
								mol.isReacting = true;
								mol.gameObject.rigidbody.isKinematic = true;
								mol.gameObject.transform.parent = enzyme.gameObject.transform;							
							}
						}
					}
				}									
			}
		}

		reactingElements = (from molObject in molObjects 
		                        where molObject.reactionId != -1	&& molObject.reactionCat == 1		 
		                        group molObject by molObject.reactionId into g
		                        select g);

		foreach(var g in reactingElements)
		{
			var l = g.OrderByDescending(x => x.type).ToArray();

			MolObject enzyme = l[0];
			MolObject mol1 = l[1];			
			MolObject mol2 = l[2];
			
			if(mol1.isReacting && mol2.isReacting)
			{
				if(mol1.isBinding && mol2.isBinding)
				{
					if(enzyme.Timeout(120))
					{						
						mol1.isFreezed = mol2.isFreezed = false;
						mol1.isBinding = mol2.isBinding = false;
						mol1.rigidbody.isKinematic = mol2.rigidbody.isKinematic = false;
						
						mol1.gameObject.transform.parent = enzyme.gameObject.transform.parent;
						mol2.gameObject.transform.parent = enzyme.gameObject.transform.parent;			

						// Change type of elements
						int reactionType = enzyme.reactionType;
						int productType_1 = reactionProducts[reactionType][0];
						int productType_2 = reactionProducts[reactionType][1];
						
						mol1.gameObject.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Meshes/" + species[productType_1]);
						mol2.gameObject.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Meshes/" + species[productType_2]);

						mol1.gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + species[productType_1]);
						mol2.gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + species[productType_2]);

						mol1.color = mol1.gameObject.GetComponent<MeshRenderer>().material.color;
						mol2.color = mol2.gameObject.GetComponent<MeshRenderer>().material.color;

						enzyme.RestoreColor();

						mol1.type = productType_1;
						mol2.type = productType_2;
					}
				}
				else
				{
					Vector3 attractionForce = (enzyme.transform.position - mol1.transform.position).normalized;
					mol1.rigidbody.AddForce(-attractionForce * 250);

					attractionForce = (enzyme.transform.position - mol2.transform.position).normalized;
					mol2.rigidbody.AddForce(-attractionForce * 250);

					if(enzyme.Timeout(60))
					{
						print("End reaction " + mol1.reactionId);
						
						Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol1.gameObject.rigidbody.collider, false);
						Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol2.gameObject.rigidbody.collider, false);						
						
						mol1.isReacting = mol2.isReacting = false;		

						enzyme.EndReaction();
						mol1.EndReaction();
						mol2.EndReaction();


					}
				}
			}
			else
			{
				if(!mol1.isReacting)
				{
					if(!mol1.isBinding)
					{
						Vector3 attractionForce = (enzyme.transform.Find("Anchor").position - mol1.transform.position).normalized;
						mol1.rigidbody.AddForce(attractionForce * 200);
						
						float distance = Vector3.Distance(enzyme.transform.position, mol1.transform.position);
						float collideDistance = (enzyme.gameObject.GetComponent<SphereCollider>().radius + mol1.gameObject.GetComponent<SphereCollider>().radius) * molSize;
						
						if(distance <= collideDistance  + 1.0f)
						{
							print("Collision 1");
							
							if(mol1.Timeout(0))
							{
								mol1.isBinding = true;
								mol1.isFreezed = true;
								Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol1.gameObject.rigidbody.collider, true);								
							}
						}
					}
					else
					{
						Vector3 attractionForce = (enzyme.transform.Find("Anchor").position - mol1.transform.position).normalized;
						mol1.rigidbody.AddForce(attractionForce * 500);
						
						float distance = Vector3.Distance(enzyme.transform.Find("Anchor").position, mol1.transform.position);
						
						if(distance <= 4.0f)
						{
							print("Binding 1");
							
							if(mol1.Timeout(0))
							{
								mol1.isReacting = true;
								mol1.gameObject.rigidbody.isKinematic = true;
								mol1.gameObject.transform.parent = enzyme.gameObject.transform;							
							}
						}
					}
				}	
				
				if(!mol2.isReacting)
				{
					if(!mol2.isBinding)
					{
						Vector3 attractionForce = (enzyme.transform.Find("Anchor").position - mol2.transform.position).normalized;
						mol2.rigidbody.AddForce(attractionForce * 200);

						float distance = Vector3.Distance(enzyme.transform.position, mol2.transform.position);
						float collideDistance = (enzyme.gameObject.GetComponent<SphereCollider>().radius + mol2.gameObject.GetComponent<SphereCollider>().radius) * molSize;
						
						if(distance <= collideDistance  + 1.0f)
						{
							print("Collision 2");
							
							if(mol2.Timeout(0))
							{
								mol2.isBinding = true;								
								mol2.isFreezed = true;
								Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol2.gameObject.rigidbody.collider, true);								
							}
						}
					}
					else
					{
						Vector3 attractionForce = (enzyme.transform.Find("Anchor").position - mol2.transform.position).normalized;
						mol2.rigidbody.AddForce(attractionForce * 500);

						float distance = Vector3.Distance(enzyme.transform.Find("Anchor").position, mol2.transform.position);
						
						if(distance <= 4.0f)
						{
							print("Binding 2");
							
							if(mol2.Timeout(0))
							{
								mol2.isReacting = true;							
								mol2.gameObject.rigidbody.isKinematic = true;
								mol2.gameObject.transform.parent = enzyme.gameObject.transform;							
							}
						}
					}
				}									
			}
		}
	}
}