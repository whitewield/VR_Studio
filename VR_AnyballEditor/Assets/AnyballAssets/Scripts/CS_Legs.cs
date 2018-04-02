using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Legs : MonoBehaviour {



	[SerializeField] float footLength=2;
	[SerializeField] float maxFootDistance =3;

	LineRenderer lineRend;
	Rigidbody parentRigidBody;
	Vector3 movingDirection;

	bool stepping = false;

	[SerializeField] float footMoveSpeed;
//	float startTime;
	float journeyLength;



	void Awake() {
		lineRend = GetComponent<LineRenderer> ();
//		parentRigidBody = transform.parent.GetComponent<Rigidbody> ();
		parentRigidBody = GetComponentInParent<Rigidbody>();
	}


	void Start () {
//		startTime = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		movingDirection = parentRigidBody.velocity.normalized;
			
		UpdateLegpoint1 ();
		CheckFootDistance ();
	}

	void UpdateLegpoint1 () {
		lineRend.SetPosition (0, transform.position);

	}

	void Step() {
		stepping = true;
		Vector3 newPos = new Vector3 (transform.position.x+footLength*movingDirection.x,
			transform.position.y-1f,
			transform.position.z+footLength*movingDirection.z);

//		float distCovered = (Time.time - startTime) * footMoveSpeed;
//		float fracJourney = distCovered / journeyLength;
//		Vector3 currentPos = Vector3.Lerp(startMarker.position, newPos, fracJourney);
		lineRend.SetPosition (1, newPos);

		if (lineRend.GetPosition (1) == newPos) {
			stepping = false;
		}


	}

	void CheckFootDistance() {
		Vector3 currentFootPos = lineRend.GetPosition (1);

		if (Vector3.Distance(transform.position, currentFootPos)>maxFootDistance) {
			Step ();

			if (!stepping) {
				Step ();
			}
		}


	}
}
