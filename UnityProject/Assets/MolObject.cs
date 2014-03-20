using UnityEngine;
using System.Collections;

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
	public Color color;
	private Vector3 randomVector;

	// Use this for initialization
	void Start ()
	{
		color = gameObject.GetComponent<MeshRenderer> ().material.color;
		drag = rigidbody.drag;
	}

	void FixedUpdate() 
	{
		if (!isMoving || isFreezed) return;

		if(!isBinding)
		{
			rigidbody.drag = drag * 2;
		}
		else
		{
			rigidbody.drag = drag;
		}

		randomVector = Random.insideUnitSphere;

		rigidbody.AddForce(randomVector * randomForce);
		rigidbody.AddTorque(randomVector * randomTorque);

//		Vector3 temp = rigidbody.position;
//		
//		if(rigidbody.position.x > transform.parent.position.x + MainScript.BoxSize.x * 0.5f)
//		{
//			temp.x = transform.parent.position.x + MainScript.BoxSize.x * 0.5f - gameObject.GetComponent<SphereCollider>().radius * 2; 
//		}
//		else if(rigidbody.position.x < transform.parent.position.x - MainScript.BoxSize.x * 0.5f)
//		{
//			temp.x = transform.parent.position.x - MainScript.BoxSize.x * 0.5f + gameObject.GetComponent<SphereCollider>().radius * 2; 
//		}
//		
//		if(rigidbody.position.y > transform.parent.position.y + MainScript.BoxSize.y * 0.5f)
//		{
//			temp.y = transform.parent.position.y + MainScript.BoxSize.y * 0.5f - gameObject.GetComponent<SphereCollider>().radius * 2; 
//		}		
//		else if(rigidbody.position.y < transform.parent.position.y - MainScript.BoxSize.y * 0.5f)
//		{
//			temp.y = transform.parent.position.y - MainScript.BoxSize.y * 0.5f + gameObject.GetComponent<SphereCollider>().radius * 2; 
//		}
//		
//		if(rigidbody.position.z > transform.parent.position.z + MainScript.BoxSize.z * 0.5f)
//		{
//			temp.z = transform.parent.position.z + MainScript.BoxSize.z * 0.5f - gameObject.GetComponent<SphereCollider>().radius * 2; 
//		}		
//		else if(rigidbody.position.z < transform.parent.position.z - MainScript.BoxSize.z * 0.5f)
//		{
//			temp.z = transform.parent.position.z - MainScript.BoxSize.z * 0.5f + gameObject.GetComponent<SphereCollider>().radius * 2; 
//		}
//		
//		rigidbody.position = temp;
	}

	void Update () 
	{
		rigidbody.collider.enabled = true;
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

	public void HighlightColor()
	{
		gameObject.GetComponent<MeshRenderer> ().material.color = color + new Color(0.75f, 0.75f, 0.75f);
	}

	public void RestoreColor()
	{
		gameObject.GetComponent<MeshRenderer> ().material.color = color;
	}

	public void EndReaction()
	{
		reactionType = -1;
		reactionId = -1;
		timeout = 0;
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