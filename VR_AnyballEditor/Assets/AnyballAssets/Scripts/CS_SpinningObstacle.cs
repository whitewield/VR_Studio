using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_SpinningObstacle : MonoBehaviour {

	[SerializeField] float rotationSpeed;


	void Update () {
		transform.Rotate(new Vector3(0,rotationSpeed*Time.deltaTime,0));
	}
}
