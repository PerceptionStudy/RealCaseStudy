using UnityEngine;
using System.Collections;

public class MolObject : MonoBehaviour
{
	public float randomForce = 1000;
	public float randomTorque = 25;

	public int type = -1;
	public int timeout = -1;	
	public int reactionId = -1;
	public int reactionType = -1;

	public bool isMoving = true;
	public bool isFreezed = false;
	public bool isReacting = false;
		
	private Color color;
	private Vector3 randomVector;

	// Use this for initialization
	void Start ()
	{
		color = gameObject.GetComponent<MeshRenderer> ().material.color;
	}

	void FixedUpdate() 
	{
		if (!isMoving || isFreezed) return;

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
		timeout ++;
		return (timeout > time);
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

	public void HighlightReaction()
	{
		gameObject.GetComponent<MeshRenderer> ().material.color = color + new Color(0.75f, 0.75f, 0.75f);
	}

	public void StartReaction()
	{
		isReacting = true;
		timeout = 0;
	}

	public void EndReaction()
	{
		isReacting = false;

		reactionType = -1;
		reactionId = -1;
		timeout = 0;

		gameObject.GetComponent<MeshRenderer> ().material.color = color;
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