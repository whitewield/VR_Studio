using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_BodyRotateJoint : MonoBehaviour {

	private Quaternion myDefaultRotation;
	private Vector3 myLastPosition;

	// Use this for initialization
	void Start () {
		myDefaultRotation = this.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 t_direction = this.transform.position - myLastPosition;

		this.transform.right = -t_direction;

		this.transform.rotation = Quaternion.Lerp (this.transform.rotation, myDefaultRotation, Time.deltaTime * 10);

		myLastPosition = this.transform.position;
	}
}
