using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;

public class MolObject : MonoBehaviour
{
	public float randomForce = 1000;
	public float randomTorque = 25;

	public int type = -1;
	public int timeout = -1;	
	public int reactionId = -1;	
	public int reactionCat = -1;
	public int reactionType = -1;

	public bool isFreezed = false;
	public bool isReacting = false;	
	public bool isBinding = false;

	private bool isMoving = true;

	private float drag;
		
	//public Color color;	

	public MolColor defaultColor;	
	public MolColor currentColor;
	private Vector3 randomVector;

	public int intensity;
	
	private bool up = true;
	private bool stimulus = false;	
	private bool firstWave = false;
	
	private int periodBegin = 50;
	private int periodEnd = 500;

	private int amplitudeBegin = 100;
	private int amplitudeEnd = 25;

	private int transitionBegin = 500;
	private int transitionEnd = 2000;


	private Stopwatch stopWatch = new Stopwatch ();		
	private Stopwatch stopWatch_2 = new Stopwatch ();	

	private bool phaseTwo = false;

	// Use this for initialization
	void Start ()
	{
		//print ("Material color: " + gameObject.GetComponent<MeshRenderer> ().material.color); 
		defaultColor = new MolColor (gameObject.GetComponent<MeshRenderer> ().material.color);
		currentColor = defaultColor; 
		drag = rigidbody.drag;
			
		rigidbody.collider.enabled = true;
	}

	void Update()
	{
		if(stimulus) StimulusUpdate();
	}

	void FixedUpdate() 
	{
		if (!isMoving || isFreezed) return;

		randomVector = UnityEngine.Random.insideUnitSphere;

		if(isBinding)
		{
			rigidbody.drag = drag * 5;		
		}
		else
		{
			rigidbody.drag = drag * 3;
		}				
		
		rigidbody.AddForce(randomVector * randomForce);		
		rigidbody.AddTorque(randomVector * randomTorque);

		Vector3 temp = rigidbody.position;
		
		if(rigidbody.position.x > transform.parent.position.x + MainScript.BoxSize.x * 0.5f)
		{
			temp.x = transform.parent.position.x + MainScript.BoxSize.x * 0.5f - gameObject.GetComponent<SphereCollider>().radius * 2; 
		}
		else if(rigidbody.position.x < transform.parent.position.x - MainScript.BoxSize.x * 0.5f)
		{
			temp.x = transform.parent.position.x - MainScript.BoxSize.x * 0.5f + gameObject.GetComponent<SphereCollider>().radius * 2; 
		}
		
		if(rigidbody.position.y > transform.parent.position.y + MainScript.BoxSize.y * 0.5f)
		{
			temp.y = transform.parent.position.y + MainScript.BoxSize.y * 0.5f - gameObject.GetComponent<SphereCollider>().radius * 2; 
		}		
		else if(rigidbody.position.y < transform.parent.position.y - MainScript.BoxSize.y * 0.5f)
		{
			temp.y = transform.parent.position.y - MainScript.BoxSize.y * 0.5f + gameObject.GetComponent<SphereCollider>().radius * 2; 
		}
		
		if(rigidbody.position.z > transform.parent.position.z + MainScript.BoxSize.z * 0.5f)
		{
			temp.z = transform.parent.position.z + MainScript.BoxSize.z * 0.5f - gameObject.GetComponent<SphereCollider>().radius * 2; 
		}		
		else if(rigidbody.position.z < transform.parent.position.z - MainScript.BoxSize.z * 0.5f)
		{
			temp.z = transform.parent.position.z - MainScript.BoxSize.z * 0.5f + gameObject.GetComponent<SphereCollider>().radius * 2; 
		}
		
		rigidbody.position = temp;
	}

	public void StartStimulus(int stimulusType, bool phaseTwo = false)
	{
		this.phaseTwo = phaseTwo;

		if (stimulusType == 3) 
		{
			gameObject.GetComponent<MeshRenderer>().enabled = true;
		}
		else if (stimulusType == 2)
		{
			stimulus = true;
			firstWave = true;
			
			stopWatch.Reset ();
			stopWatch.Start ();

			stopWatch_2.Reset();
			stopWatch_2.Start();

		}
		if (stimulusType == 1)
		{
			//gameObject.GetComponent<MeshRenderer> ().material.color = new MolColor(defaultColor.L + 60, defaultColor.a, defaultColor.b).rgba; 
		}
	}
	
	public void StopStimulus(int stimulusType)
	{
		stimulus = false; 
		firstWave = false;
		
		//print("Set default color: " + defaultColor.rgba); 
		currentColor = defaultColor; 
		gameObject.GetComponent<MeshRenderer> ().material.color = defaultColor.rgba; 
		
		stopWatch.Stop();
		stopWatch.Reset();

		stopWatch_2.Stop ();
		stopWatch_2.Stop ();

		phaseTwo = false;
	}
	
	private void StimulusUpdate()
	{
		int currentWaveTime = (int)stopWatch.ElapsedMilliseconds;

		float ct = Math.Max((float)(stopWatch_2.ElapsedMilliseconds - transitionBegin), 0.0f);
		float t = Math.Min(ct / (transitionEnd - transitionBegin), 1.0f);

		if (phaseTwo) t = 1.0f;

		int amplitude = (int)Mathf.Lerp (amplitudeBegin, amplitudeEnd, t);
		int halfAmplitude = amplitude / 2;

		int period = (int)Mathf.Lerp (periodBegin, periodEnd, t);
		int halfPeriod = period / 2;

		if(firstWave)
		{
			currentWaveTime += periodBegin / 2;
		}		

		float progress = (float) currentWaveTime / halfPeriod;	
		float intensityShift = Mathf.Clamp((up) ? progress * 2.0f - 1.0f : (1.0f-progress) * 2.0f - 1.0f, -1.0f, 1.0f) * (float)halfAmplitude;
		
		float currentIntensity = intensityShift + defaultColor.L; // = Mathf.Clamp(defaultColor.L + intensityShift, 0, 100);		
		
		int deltaUp = (int)defaultColor.L + halfAmplitude;
		int deltaDown = (int)defaultColor.L - halfAmplitude;
		
		if (deltaUp > 100)
		{
			currentIntensity -= deltaUp - 100;
		}
		else if (deltaDown < 0)
		{
			currentIntensity += Math.Abs(deltaDown);
		}
		
		currentIntensity = Mathf.Clamp (currentIntensity, 0.0f, 100.0f);
		currentColor = new MolColor(currentIntensity, defaultColor.a, defaultColor.b);
		
		gameObject.GetComponent<MeshRenderer> ().material.color = currentColor.rgba;
		
		if(currentWaveTime > halfPeriod)
		{
			up = !up;
			stopWatch.Reset();
			stopWatch.Start();
			firstWave = false;
		}
	}

	public void Freeze()
	{
		isFreezed = true;		
		rigidbody.isKinematic = true;
	}

	public void Defreeze()
	{
		isFreezed = false;			
		rigidbody.isKinematic = false;
	}

	public bool Timeout(int time)
	{
		if(timeout ++ >= time)
		{	
			timeout = 0;
			return true;
		}

		return false;
	}

	public void MuteAnimation()
	{
		isMoving = false;
	}

	public void ResumeAnimation()
	{
		isMoving = true;
	}

	public bool IsReacting()
	{
		return reactionId != -1;
	}

//	public void HighlightColor()
//	{
//		gameObject.GetComponent<MeshRenderer> ().material.color = color + new Color(0.75f, 0.75f, 0.75f);
//	}
//
//	public void RestoreColor()
//	{
//		gameObject.GetComponent<MeshRenderer> ().material.color = color;
//	}

	public void EndReaction(int stimulusType)
	{
		reactionType = -1;
		reactionId = -1;
		timeout = 0;

		if(stimulusType == 3)gameObject.GetComponent<MeshRenderer>().enabled = false;		

	}

//	void OnPreRender() 
//	{
//
//	}
//	
//	void OnMouseDown()
//	{
//		//print ("Hello");
//	}
}