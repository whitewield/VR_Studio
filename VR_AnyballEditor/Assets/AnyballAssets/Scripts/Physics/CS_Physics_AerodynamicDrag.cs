using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Physics_AerodynamicDrag : MonoBehaviour {

	[SerializeField] float myAerodynamicDrag = 1;
	[SerializeField] Transform myDragCenterTransform;
	private Rigidbody myRigidbody;

	void Awake () {
		myRigidbody = this.GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		myRigidbody.AddForceAtPosition (
			myRigidbody.velocity.normalized * -1 * myRigidbody.velocity.sqrMagnitude * myAerodynamicDrag,
			myDragCenterTransform.position
		);
	}


}
