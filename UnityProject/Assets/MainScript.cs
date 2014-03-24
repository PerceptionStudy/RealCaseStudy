using UnityEngine;
using System.Diagnostics;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Linq;
using System.Collections.Generic;

public class MainScript : MonoBehaviour 
{
	public float molSize = 10;

	public float attractionScale = 0;
	public float bindingScale = 0;
	public float repulsionScale = 0;

	private int molCount= 0;
	private int reactionCounter = 0;

	private MolObject[] molObjects;	
	private GameObject previousSelectedObject = null;

	private GameObject collideBox;
	private GameObject rootObject;

	private int currentSession = 1;
	private int currentStimulusType = 1;

	private List<int> currentSpeciesColor;
	private List<int> currentSpeciesCount;
	private List<int> currentReactionSequence; 

	private List<int> currentReactionEnzymes;
	private List<int[]> currentReactionSubstrates;
	private List<int[]> currentReactionProducts;

	public static Vector3 BoxSize = new Vector3();

	private int delay = 15000;

	List<int> speciesCount_1 = new List<int>
	{
		100,					// 1-methy-NAM
		100,					// ADP-Ribose
		100,					// ATP
		100,					// NAD
		100,					// NAM
		100,					// NMN
		0,						// PPi
		800,					// PRPP
		100,					// SAH
		100,					// SAM
		50,						// NAMTP
		50,						// NMAT1
		50,						// NMNT
		50,						// PARP1
	};

	List<int> speciesCount_2 = new List<int>
	{
		100,					// 1-methy-NAM
		0,						// ADP-Ribose
		800,					// ATP
		100,					// NAD
		100,					// NAM
		100,					// NMN
		100,					// PPi
		100,					// PRPP
		100,					// SAH
		100,					// SAM
		50,						// NAMTP
		50,						// NMAT1
		50,						// NMNT
		50,						// PARP1
	};

	List<int> speciesCount_3 = new List<int>
	{
		100,					// 1-methy-NAM
		100,					// ADP-Ribose
		100,					// ATP
		100,					// NAD
		800,					// NAM
		100,					// NMN
		100,					// PPi
		100,					// PRPP
		100,						// SAH
		0,					// SAM
		50,						// NAMTP
		50,						// NMAT1
		50,						// NMNT
		50,						// PARP1
	};

	List<int> reactionSequence_1 = new List<int>
	{
		0,					// 
		1,					// 
		2,					// 
	};

	List<int> reactionSequence_2 = new List<int>
	{
		1,					// 
		2,					// 
		3,					// 
	};

	List<int> reactionSequence_3 = new List<int>
	{
		4,					// 
		0,					// 
		1,					// 
	};

	List<int> speciesColor_1 = new List<int>
	{
		1,					// 1-methy-NAM
		2,					// ADP-Ribose
		3,					// ATP
		4,					// NAD
		5,					// NAM
		6,					// NMN
		7,					// PPi
		8,					// PRPP
		9,					// SAH
		10,					// SAM
		11,					// 
		12,					// 
		13,					// 
		14					// 
	};

	List<int> speciesColor_2 = new List<int>
	{
		10,					// 1-methy-NAM
		9,					// ADP-Ribose
		7,					// ATP
		8,					// NAD
		6,					// NAM
		5,					// NMN
		4,					// PPi
		3,					// PRPP
		2,					// SAH
		1,					// SAM
		11,					// 
		12,					// 
		13,					// 
		14					// 
	};

	List<int> speciesColor_3 = new List<int>
	{
		2,					// 1-methy-NAM
		7,					// ADP-Ribose
		3,					// ATP
		9,					// NAD
		10,					// NAM
		4,					// NMN
		6,					// PPi
		1,					// PRPP
		5,					// SAH
		8,					// SAM
		11,					// 
		12,					// 
		13,					// 
		14					// 
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
		13,						// PARP1		
		12,						// NMNT		
		12,						// NMNT_enzyme
	};

	List<int[]> reactionSubstrates = new List<int[]>
	{
		new int[2] {4, 7},		// NAMTP 
		new int[2] {5, 2},		// NMAT1
		new int[1] {3},			// PARP1		
		new int[2] {4, 9},		// NMNT		
		new int[2] {0, 8},		// NMNT_reversed
	};

	List<int[]> reactionProducts = new List<int[]>
	{
		new int[2] {5, 6},		// NAMTP 
		new int[2] {3, 6},		// NMAT1		
		new int[2] {4, 1},		// PARP1
		new int[2] {0, 8},		// NMNT
		new int[2] {4, 9},		// NMNT_reversed
	};

	/***** Test *****/

	List<int> speciesCount_test = new List<int>
	{
		50,					// 1-methy-NAM
		50,					// ADP-Ribose
		0,					// ATP
		200,					// NAD
		50,					// NAM
		50,					// NMN
		50,						// PPi
		50,					// PRPP
		0,					// SAH
		0,					// SAM
		30,						// NAMTP
		30,						// NMAT1
		50,						// NMNT
		30,						// PARP1
	};

	List<int> speciesColor_test = new List<int>
	{
		9,					// 1-methy-NAM
		1,					// ADP-Ribose
		6,					// ATP
		3,					// NAD
		5,					// NAM
		2,					// NMN
		7,					// PPi
		4,					// PRPP
		10,					// SAH
		8,					// SAM
		11,					// 
		12,					// 
		13,					// 
		14					// 
	};

	List<int> reactionEnzymes_test = new List<int>
	{
		12, 					//  
		10,						// 
		13,						// 	
	};

	List<int> reactionSequence_test = new List<int>
	{
		0,					// 
		1,					// 
		2,					// 
	};

	List<int[]> reactionSubstrates_test = new List<int[]>
	{
		new int[2] {1, 7},		//  
		new int[2] {4, 2},		// 
		new int[2] {5, 6},		// 
	};
	
	List<int[]> reactionProducts_test = new List<int[]>
	{
		new int[2] {2, 3},		//  
		new int[2] {5, 3},		// 		
		new int[2] {8, 7},		// 
	};

	public MolObject CreateMol(Transform parent, string name, int type)
	{
		Vector3 position = new Vector3 ((UnityEngine.Random.value - 0.5f) * MainScript.BoxSize.x, (UnityEngine.Random.value - 0.5f) * MainScript.BoxSize.y, (UnityEngine.Random.value - 0.5f) * MainScript.BoxSize.z);
		
		GameObject gameObject = Instantiate(Resources.Load("Prefabs/" + species[type]), position, UnityEngine.Random.rotation) as GameObject;		
		gameObject.name = name;
		gameObject.transform.parent = parent;
		gameObject.transform.localScale = new Vector3(molSize, molSize, molSize);	
		gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + currentSpeciesColor[type]);

		MolObject molObject = gameObject.GetComponent<MolObject>();
		molObject.type = type;

		return molObject;
	}

	public void LoadScene()
	{
		switch(currentSession)
		{
			case 1: currentSpeciesCount = speciesCount_1; currentSpeciesColor = speciesColor_1; currentReactionSequence = reactionSequence_1; break;
			case 2: currentSpeciesCount = speciesCount_2; currentSpeciesColor = speciesColor_2; currentReactionSequence = reactionSequence_2; break;
			case 3: currentSpeciesCount = speciesCount_3; currentSpeciesColor = speciesColor_3; currentReactionSequence = reactionSequence_3; break;
			case 4: currentSpeciesCount = speciesCount_test; currentSpeciesColor = speciesColor_test; currentReactionSequence = reactionSequence_test; break;
		}

		if (currentSession == 4)
		{
			currentReactionEnzymes = reactionEnzymes_test;
			currentReactionProducts = reactionProducts_test;
			currentReactionSubstrates = reactionSubstrates_test;
		} 
		else 
		{
			currentReactionEnzymes = reactionEnzymes;
			currentReactionProducts = reactionProducts;
			currentReactionSubstrates = reactionSubstrates;
		}

		BoxSize.x = Screen.width;
		BoxSize.y = Screen.height;
		BoxSize.z = 300;
				
		collideBox.transform.localScale = new Vector3(BoxSize.x, BoxSize.y, BoxSize.z);

		// Destroy previous game objects
		if(molObjects != null)
		{
			foreach(MolObject obj in molObjects)
			{
				Destroy(obj.gameObject);
			}
		}

		molObjects = new MolObject[currentSpeciesCount.Sum()];

		molCount = 0;
		for (var i = 0; i < species.Count(); i++)
		{			
			for (var j = 0; j < currentSpeciesCount[i]; j++)
			{
				molObjects[molCount ++] = CreateMol(gameObject.transform, "Mol_" + molCount, i);
			}
		}

		if (currentStimulusType == 3) 
		{
			foreach(var m in molObjects)
			{
				m.gameObject.GetComponent<MeshRenderer>().enabled = false;
			}
		}
	}

	void Start () 
	{
		collideBox = GameObject.Find("Box Object");
		rootObject = GameObject.Find("Root Object");

		int metaboliteLayer = LayerMask.NameToLayer("Metabolite");

		Physics.IgnoreLayerCollision(metaboliteLayer, metaboliteLayer);

		Time.timeScale = 0;
	}

	bool startScreen = true;

	private Stopwatch stopWatch = new Stopwatch ();	

	void OnGUI()
	{
		if (startScreen) 
		{
			GUI.DrawTexture (new Rect (0.0f, 0.0f, Screen.width, Screen.height), (UnityEngine.Texture)Resources.Load ("startScreen")); 
		}		
	}

	bool reactionToken = true;

	void Update () 
	{
		if (startScreen)
		{
			if(Input.GetKeyDown (KeyCode.Alpha1))
			{				
				currentSession = 1;
				currentStimulusType = 1;
				startScreen = false;
			}

			if(Input.GetKeyDown (KeyCode.Alpha2))
			{				
				currentSession = 1;
				currentStimulusType = 3;
				startScreen = false;
			}

			if(Input.GetKeyDown (KeyCode.Alpha3))
			{				
				currentSession = 1;
				currentStimulusType = 2;
				startScreen = false;
			}

			if(Input.GetKeyDown (KeyCode.Alpha4))
			{				
				currentSession = 2;
				currentStimulusType = 1;
				startScreen = false;
			}

			if(Input.GetKeyDown (KeyCode.Alpha5))	
			{				
				currentSession = 2;
				currentStimulusType = 3;
				startScreen = false;
			}

			if(Input.GetKeyDown (KeyCode.Alpha6))	
			{				
				currentSession = 2;
				currentStimulusType = 2;
				startScreen = false;
			}

			if(Input.GetKeyDown (KeyCode.Alpha7))	
			{				
				currentSession = 3;
				currentStimulusType = 1;
				startScreen = false; 
			}

			if(Input.GetKeyDown (KeyCode.Alpha8))
			{				
				currentSession = 3;
				currentStimulusType = 3;
				startScreen = false;
			}

			if(Input.GetKeyDown (KeyCode.Alpha9))		
			{				
				currentSession = 3;
				currentStimulusType = 2;
				startScreen = false;
			}

			if(Input.GetKeyDown (KeyCode.A))	
			{				
				currentSession = 4;
				currentStimulusType = 1;
				startScreen = false; 
			}
			
			if(Input.GetKeyDown (KeyCode.B))
			{				
				currentSession = 4;
				currentStimulusType = 3;
				startScreen = false;
			}
			
			if(Input.GetKeyDown (KeyCode.C))		
			{				
				currentSession = 4;
				currentStimulusType = 2;
				startScreen = false;
			}

			if(!startScreen)
			{
				LoadScene();
				stopWatch.Start();
				Time.timeScale = 1;
			}
		}
		else
		{
			Camera.main.orthographicSize = Screen.height * 0.5f;
			
			BoxSize.x = Screen.width;
			BoxSize.y = Screen.height;
			BoxSize.z = 300;
			
			collideBox.transform.localScale = new Vector3(BoxSize.x, BoxSize.y, BoxSize.z);		

			int d = 18000;

			if(reactionCounter == 0)
			{
				d = 5000;
			}

			if( reactionToken && stopWatch.ElapsedMilliseconds / d > reactionCounter && reactionCounter < 13)
			{
				bool reactionFailed = true;
				
				int reactionType = currentReactionSequence[reactionCounter%currentReactionSequence.Count];
				
				int enzymeType = currentReactionEnzymes[reactionType];
				
				MolObject[] availableElements = (from element in molObjects 
				                                 where element.reactionId == -1	&& element.type == enzymeType
				                                 orderby Guid.NewGuid()
				                                 select element).ToArray();
				
				if(availableElements.Count() > 0)
				{
					GameObject selectedObject = availableElements.First().gameObject;
					MolObject enzyme = selectedObject.GetComponent<MolObject>();
					
					if(currentReactionSubstrates[reactionType].Count() == 1)
					{
						int partnerType = currentReactionSubstrates[reactionType][0];
						
						int metaboliteLayer = LayerMask.NameToLayer("Metabolite");
						int enzymeLayer = LayerMask.NameToLayer("Enzyme");
						
						int radius = Math.Max(Screen.height, Screen.width) / 8;
						
						Collider[] results = Physics.OverlapSphere(selectedObject.transform.position, radius, 1 << metaboliteLayer | 1 << enzymeLayer);
						
						MolObject[] partners = (from element in results 
						                        where Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position) > molSize * 2 && element.gameObject.GetComponent<MolObject>().type == partnerType && element.gameObject.GetComponent<MolObject>().reactionId == -1 
						                        orderby Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position)
						                        select element.gameObject.GetComponent<MolObject>()).ToArray();		
						
						if(partners.Count() > 0)
						{
							MolObject partner = partners.First();
							
							enzyme.reactionId = partner.reactionId = reactionCounter ++;
							enzyme.reactionType = partner.reactionType = reactionType;
							enzyme.reactionCat = partner.reactionCat = 0;
							
							enzyme.StartStimulus(currentStimulusType);
							partner.StartStimulus(currentStimulusType);
							
							previousSelectedObject = selectedObject;
							
							print ("Start reaction: " + enzyme.reactionId + " of type: " + reactionType );

							reactionToken = false;
						}
						else
						{
							reactionFailed = true;
						}
					}
					else if(currentReactionSubstrates[reactionType].Count() == 2)
					{
						int partnerType_1 = currentReactionSubstrates[reactionType][0];
						int partnerType_2 = currentReactionSubstrates[reactionType][1];
						
						int metaboliteLayer = LayerMask.NameToLayer("Metabolite");
						int enzymeLayer = LayerMask.NameToLayer("Enzyme");
						
						int radius = Math.Max(Screen.height, Screen.width) / 8;
						
						Collider[] results = Physics.OverlapSphere(selectedObject.transform.position, radius, 1 << metaboliteLayer | 1 << enzymeLayer);
						
						MolObject[] partners_1 = (from element in results 
						                          where Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position) > molSize * 2 && element.gameObject.GetComponent<MolObject>().type == partnerType_1 && element.gameObject.GetComponent<MolObject>().reactionId == -1 
						                          orderby Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position)
						                          select element.gameObject.GetComponent<MolObject>()).ToArray();	
						
						MolObject[] partners_2 = (from element in results 
						                          where Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position) > molSize * 2 && element.gameObject.GetComponent<MolObject>().type == partnerType_2 && element.gameObject.GetComponent<MolObject>().reactionId == -1 
						                          orderby Vector3.Distance(selectedObject.transform.position, element.gameObject.transform.position)
						                          select element.gameObject.GetComponent<MolObject>()).ToArray();	
						
						if(partners_1.Count() > 0 && partners_2.Count() > 0)
						{
							MolObject partner_1 = partners_1.First();
							MolObject partner_2 = partners_2.First();
							
							enzyme.reactionId = partner_1.reactionId = partner_2.reactionId = reactionCounter ++;
							enzyme.reactionType = partner_1.reactionType = partner_2.reactionType = reactionType;
							enzyme.reactionCat = partner_1.reactionCat = partner_2.reactionCat = 1;
							
							enzyme.StartStimulus(currentStimulusType);
							partner_1.StartStimulus(currentStimulusType);
							partner_2.StartStimulus(currentStimulusType);

							//enzyme.HighlightColor();
							//partner_1.HighlightColor();						
							//partner_2.HighlightColor();
							
							previousSelectedObject = selectedObject;
							
							print ("Start reaction: " + enzyme.reactionId + " of type: " + reactionType );

							reactionToken = false;
						}	
						else
						{
							reactionFailed = true;
						}
					}
				}
			}

			if(Input.GetKeyDown (KeyCode.R))
			{
				startScreen = true;					
				Time.timeScale = 0;
				reactionCounter = 0;
				stopWatch.Stop();
				stopWatch.Reset();
				reactionToken = true;
			}
		}
	}


	//TODO: Update species count

	void FixedUpdate()
	{
		if (startScreen)
						return;

		var reactingElements = (from molObject in molObjects 
		                        where molObject.reactionId != -1 && molObject.reactionCat == 0		 
		                        group molObject by molObject.reactionId into g
		                        select g);

		foreach(var g in reactingElements)
		{
			var l = g.OrderByDescending(x => x.type).ToArray();
			
			MolObject enzyme = l[0];
			MolObject mol = l[1];

			enzyme.rigidbody.transform.rotation = Quaternion.Lerp (enzyme.transform.rotation, Quaternion.identity, 0.08f);

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
						int productType_1 = currentReactionProducts[reactionType][0];
						int productType_2 = currentReactionProducts[reactionType][1];
						
						mol.gameObject.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Meshes/" + species[productType_1]);
						mol.gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + currentSpeciesColor[productType_1]);
						mol.defaultColor = new MolColor(mol.gameObject.GetComponent<MeshRenderer>().material.color);
						//mol.HighlightColor();
						mol.type = productType_1;

						// Create new molecules

						molCount ++;

						GameObject gameObject = Instantiate(Resources.Load("Prefabs/" + species[productType_2]), mol.transform.position, UnityEngine.Random.rotation) as GameObject;		
						gameObject.name = "Mol_" + molCount;
						gameObject.transform.parent = mol.transform.parent;
						gameObject.transform.localScale = new Vector3(molSize, molSize, molSize);
						gameObject.GetComponent<SphereCollider>().enabled = true;
						gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + currentSpeciesColor[productType_2]);

						Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, gameObject.rigidbody.collider, true);

						MolObject newMol = gameObject.GetComponent<MolObject>();
						newMol.type = productType_2;
						newMol.defaultColor = new MolColor(mol.gameObject.GetComponent<MeshRenderer>().material.color);
						//newMol.HighlightColor();

						Array.Resize(ref molObjects, molCount);
						molObjects[molCount-1] = newMol;

						newMol.reactionId = mol.reactionId;


						enzyme.StopStimulus(currentStimulusType);
						mol.StopStimulus(currentStimulusType);

						mol.reactionCat = enzyme.reactionCat = newMol.reactionCat = 1;

						newMol.isReacting = true;
						newMol.StartStimulus(currentStimulusType);
//						enzyme.RestoreColor();
//						mol.RestoreColor();

						mol.StartStimulus(currentStimulusType, true);
						newMol.StartStimulus(currentStimulusType, true);
					}
				}
				else
				{
					Vector3 attractionForce = (enzyme.transform.position - mol.transform.position).normalized;
					mol.rigidbody.AddForce(-attractionForce * repulsionScale);
					
					if(enzyme.Timeout(120))
					{	
						// TODO: Restore collision between 
						Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol.gameObject.rigidbody.collider, false);					
						
						mol.isReacting = false;		
						
						enzyme.EndReaction(currentStimulusType);
						mol.EndReaction(currentStimulusType);
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
						mol.rigidbody.AddForce(attractionForce * attractionScale);
						
						float distance = Vector3.Distance(enzyme.transform.position, mol.transform.position);
						float collideDistance = (enzyme.gameObject.GetComponent<SphereCollider>().radius + mol.gameObject.GetComponent<SphereCollider>().radius) * molSize;
						
						if(distance <= collideDistance  + 1.0f)
						{
							print("Collision 1");
							
							if(mol.Timeout(0))
							{
								mol.isBinding = true;
								Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol.gameObject.rigidbody.collider, true);	
							}
						}
					}
					else
					{
						Vector3 attractionForce = (enzyme.transform.Find("Anchor").position - mol.transform.position).normalized;
						mol.rigidbody.AddForce(attractionForce * bindingScale);
						
						float distance = Vector3.Distance(enzyme.transform.Find("Anchor").position, mol.transform.position);
						
						if(distance <= 5.0f)
						{
							print("Binding 1");
							
							if(mol.Timeout(0))
							{
								
								mol.isFreezed = true;
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

			enzyme.rigidbody.transform.rotation = Quaternion.Lerp (enzyme.transform.rotation, Quaternion.identity, 0.025f);

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
						int productType_1 = currentReactionProducts[reactionType][0];
						int productType_2 = currentReactionProducts[reactionType][1];
						
						mol1.gameObject.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Meshes/" + species[productType_1]);
						mol2.gameObject.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Meshes/" + species[productType_2]);

						mol1.gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + currentSpeciesColor[productType_1]);
						mol2.gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + currentSpeciesColor[productType_2]);

						mol1.defaultColor = new MolColor( mol1.gameObject.GetComponent<MeshRenderer>().material.color );
						mol2.defaultColor = new MolColor( mol2.gameObject.GetComponent<MeshRenderer>().material.color );

						mol1.type = productType_1;
						mol2.type = productType_2;

						mol1.StartStimulus(currentStimulusType, true);
						mol2.StartStimulus(currentStimulusType, true);
					}
				}
				else
				{
					Vector3 attractionForce = (enzyme.transform.position - mol1.transform.position).normalized;
					mol1.rigidbody.AddForce(-attractionForce * repulsionScale);

					attractionForce = (enzyme.transform.position - mol2.transform.position).normalized;
					mol2.rigidbody.AddForce(-attractionForce * repulsionScale);

					if(enzyme.Timeout(200))
					{
						print("End reaction " + mol1.reactionId);

						if(enzyme.reactionId == 12)
						{
							startScreen = true;					
							Time.timeScale = 0;
							reactionCounter = 0;
							stopWatch.Stop();
							stopWatch.Reset();
						}

						reactionToken = true;

						Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol1.gameObject.rigidbody.collider, false);
						Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol2.gameObject.rigidbody.collider, false);						
						
						mol1.isReacting = mol2.isReacting = false;		

						enzyme.EndReaction(currentStimulusType);
						mol1.EndReaction(currentStimulusType);
						mol2.EndReaction(currentStimulusType);

						enzyme.StopStimulus(currentStimulusType);
						mol1.StopStimulus(currentStimulusType);
						mol2.StopStimulus(currentStimulusType);

						mol1.gameObject.layer = LayerMask.NameToLayer("Metabolite");
						mol2.gameObject.layer = LayerMask.NameToLayer("Metabolite");
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
						mol1.rigidbody.AddForce(attractionForce * attractionScale);
						
						float distance = Vector3.Distance(enzyme.transform.position, mol1.transform.position);
						float collideDistance = (enzyme.gameObject.GetComponent<SphereCollider>().radius + mol1.gameObject.GetComponent<SphereCollider>().radius) * molSize;
						
						if(distance <= collideDistance  + 1.0f)
						{
							print("Collision 1");
							
							if(mol1.Timeout(0))
							{
								mol1.isBinding = true;
								Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol1.gameObject.rigidbody.collider, true);		
								mol1.gameObject.layer = 0;
							}
						}
					}
					else
					{
						Vector3 attractionForce = (enzyme.transform.Find("Anchor").position - mol1.transform.position).normalized;
						mol1.rigidbody.AddForce(attractionForce * bindingScale);
						
						float distance = Vector3.Distance(enzyme.transform.Find("Anchor").position, mol1.transform.position);
						
						if(distance <= 15.0f)
						{
							print("Binding 1");
							
							if(mol1.Timeout(0))
							{
								mol1.isFreezed = true;
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
						mol2.rigidbody.AddForce(attractionForce * attractionScale);

						float distance = Vector3.Distance(enzyme.transform.position, mol2.transform.position);
						float collideDistance = (enzyme.gameObject.GetComponent<SphereCollider>().radius + mol2.gameObject.GetComponent<SphereCollider>().radius) * molSize;
						
						if(distance <= collideDistance  + 1.0f)
						{
							print("Collision 2");
							
							if(mol2.Timeout(0))
							{
								mol2.isBinding = true;								

								Physics.IgnoreCollision(enzyme.gameObject.rigidbody.collider, mol2.gameObject.rigidbody.collider, true);									
								mol2.gameObject.layer = 0;
							}
						}
					}
					else
					{
						Vector3 attractionForce = (enzyme.transform.Find("Anchor").position - mol2.transform.position).normalized;
						mol2.rigidbody.AddForce(attractionForce * bindingScale);

						float distance = Vector3.Distance(enzyme.transform.Find("Anchor").position, mol2.transform.position);
						
						if(distance <= 15.0f)
						{
							print("Binding 2");
							
							if(mol2.Timeout(0))
							{
								mol2.isFreezed = true;
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