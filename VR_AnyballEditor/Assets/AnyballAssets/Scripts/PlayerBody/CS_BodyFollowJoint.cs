using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_BodyFollowJoint : MonoBehaviour {

	[SerializeField] Transform myParent;
	private Vector3 myDeltaPosition;
	private Vector3 myLastPosition;
	[SerializeField] float myMinimalRatio = 0.5f;
	[SerializeField] float myFollowAcceleration = 10;
	[SerializeField] float myFollowFriction = 0.1f;
	private Vector3 myFollowSpeed = Vector3.zero;

	// Use this for initialization
	void Start () {
		myDeltaPosition = this.transform.position - myParent.position;
		myLastPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void  FixedUpdate () {
		
		Vector3 t_targetPosition = myParent.position + myDeltaPosition;

		//get the acc
		Vector3 t_followDirection = t_targetPosition - myLastPosition;

		//update speed
		myFollowSpeed += myFollowAcceleration * t_followDirection;

		//apply friction
		myFollowSpeed *= (1 - myFollowFriction);

		//move  
		this.transform.position = myLastPosition + myFollowSpeed * Time.fixedDeltaTime;

		//update the last position
		myLastPosition = this.transform.position;
	}
}
