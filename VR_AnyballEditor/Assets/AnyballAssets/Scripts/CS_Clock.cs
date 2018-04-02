using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Clock : MonoBehaviour {


	[SerializeField] GameObject pointerCenter;

	public float remainingTime;

	float angle;

	void Start () {
		angle = 360f / remainingTime;
	}

	void Update () {
//
//		if (remainingTime > 1) {
//			remainingTime -= Time.deltaTime;
//		}

		pointerCenter.transform.RotateAround (pointerCenter.transform.position, Vector3.forward, angle * Time.deltaTime);
		
	}
}
