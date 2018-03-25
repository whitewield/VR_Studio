using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_VR_Trash : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<CS_VR_Object> () != null) {
			other.GetComponent<CS_VR_Object> ().Delete ();
		}
	}
}
