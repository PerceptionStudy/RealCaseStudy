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
		50,
		1000,
		1000,
		50,
		50,
	};

	List<string> species = new List<string>
	{
		"NAMPT",
		"ATP",
		"NAMPT_3",
		"NAMPT_4",
		"NAMPT_5",
	};

	List<int[]> reactionSubstrates = new List<int[]>
	{
		new int[2] {0, 1},
		new int[2] {1, 2},
		new int[2] {2, 3},
		new int[2] {3, 4},
		new int[2] {4, 0},
	};

	List<int[]> reactionProducts = new List<int[]>
	{
		new int[2] {2, 3},
		new int[2] {3, 4},
		new int[2] {4, 0},
		new int[2] {0, 1},
		new int[2] {1, 2},
	};

	public MolObject CreateMol(Transform parent, string name, int type)
	{
		Vector3 position = new Vector3 ((UnityEngine.Random.value - 0.5f) * MainScript.BoxSize.x, (UnityEngine.Random.value - 0.5f) * MainScript.BoxSize.y, (UnityEngine.Random.value - 0.5f) * MainScript.BoxSize.z);
		
		GameObject gameObject = Instantiate(Resources.Load(species[type]), position, UnityEngine.Random.rotation) as GameObject;		
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

		int count = 0;
		for (var i = 0; i < species.Count(); i++)
		{			
			for (var j = 0; j < speciesCount[i]; j++)
			{
				molObjects[count ++] = CreateMol(gameObject.transform, species[i] + "_" + j, i);
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
			int reactionType = UnityEngine.Random.Range(0, 4);

			int currentType = reactionSubstrates[reactionType][0];
			int partnerType = reactionSubstrates[reactionType][1];

			MolObject[] availableElements = (from element in molObjects 
			                                 where element.reactionId == -1	&& element.type == currentType
			                                 orderby Guid.NewGuid()
			                                 select element).ToArray();

			if(availableElements.Count() > 0)
			{
				GameObject selectedObject = availableElements.First().gameObject;
	
				MolObject selectedMol = selectedObject.GetComponent<MolObject>();
				
				int layer = LayerMask.NameToLayer("MolLayer");
				
				MolObject[] partners;
				
				int radius = Math.Max(Screen.height, Screen.width) / 16;
				
				Collider[] results = Physics.OverlapSphere(selectedObject.transform.position, radius, 1 << layer);
								
				partners = (from element in results 
				            where element.gameObject.GetComponent<MolObject>().type == partnerType && element.gameObject.GetComponent<MolObject>().reactionId == -1 
				            orderby Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position)
				            select element.gameObject.GetComponent<MolObject>()).ToArray();				
				
				if(partners.Count() > 0)
				{
					MolObject partnerMol = partners.First();
					
					selectedMol.reactionId = partnerMol.reactionId = reactionCounter ++;
					selectedMol.reactionType = partnerMol.reactionType = reactionType;

					selectedMol.HighlightReaction();
					partnerMol.HighlightReaction();

					previousSelectedObject = selectedObject;

					print ("Start reaction: " + selectedMol.reactionId);
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

	void FixedUpdate()
	{
		MolObject[] reactingElement = (from molObject in molObjects 
		                               where molObject.reactionId != -1	
		                               orderby molObject.reactionId 
		                               ascending  select molObject).ToArray();

		for (int i = 0; i < reactingElement.Count(); i+=2) 
		{
			MolObject mol1 = reactingElement[i];
			MolObject mol2 = reactingElement[i+1];

			if(mol1.isReacting)
			{
				bool timeout = false;

				if(mol1.type < mol2.type) 
					timeout = mol1.Timeout(120);
				else
					timeout = mol2.Timeout(120);
	
				if(timeout)
				{					
					print("End reaction " + mol1.reactionId);

					//mol1.ChangeType(reactionProducts[mol1.reactionType][0]);
					//mol2.ChangeType(reactionProducts[mol1.reactionType][1]);

					mol1.EndReaction();
					mol2.EndReaction();

					mol2.gameObject.transform.parent = mol1.gameObject.transform.parent;
					mol2.Defreeze();

					Physics.IgnoreCollision(mol1.gameObject.rigidbody.collider, mol2.gameObject.rigidbody.collider, false);
				}
			}
			else
			{
				float distance = Vector3.Distance(mol1.transform.position, mol2.transform.position);
				float collideDistance = (mol1.gameObject.GetComponent<SphereCollider>().radius + mol2.gameObject.GetComponent<SphereCollider>().radius) * molSize;

				if(distance < collideDistance)
				{
					bool timeout = false;

					if(mol1.type < mol2.type) 
						timeout = mol1.Timeout(5);
					else
						timeout = mol2.Timeout(5);
					
					if(timeout)
					{
						mol1.StartReaction();
						mol2.StartReaction();
						
						mol2.Freeze();
						mol2.gameObject.transform.parent = mol1.gameObject.transform;
						
						Physics.IgnoreCollision(mol1.gameObject.rigidbody.collider, mol2.gameObject.rigidbody.collider, true);

						mol1.timeout = 0;
						mol2.timeout = 0;
					}
				}
				else
				{
					Vector3 attractionForce = (mol2.transform.position - mol1.transform.position).normalized;

					mol1.rigidbody.AddForce(attractionForce * 100);
					mol2.rigidbody.AddForce(-attractionForce * 100);
				}
			}
		}
	}
}