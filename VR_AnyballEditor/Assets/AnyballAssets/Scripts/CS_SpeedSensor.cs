using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_SpeedSensor : MonoBehaviour {


	[SerializeField] float rotationSpeed;
	[SerializeField] GameObject target;

	void Start () {
		
	}
	

	void Update () {
		transform.LookAt (target.transform);
	}
}
