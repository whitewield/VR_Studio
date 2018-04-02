using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ConveyorBelt : MonoBehaviour {

	[SerializeField] float mySpeed;
	[SerializeField] Material myBeltMaterial;
	private float myBeltMaterial_OffsetY;
	private float myBeltSpeed;
	[SerializeField] float myBeltFriction;

	void Start () {
		myBeltSpeed = mySpeed / myBeltMaterial.mainTextureScale.y * Global.Constants.MATERIAL_TILING_CONVEYORBELT;
	}

	void OnTriggerStay (Collider g_Collider) {
		Rigidbody t_rigidbody = g_Collider.GetComponent<Rigidbody> ();
		if (t_rigidbody != null) {
			Vector3 t_project = Vector3.Project (t_rigidbody.velocity, this.transform.forward);
			float t_projectSqrMagnitude = t_project.sqrMagnitude;

			if (Vector3.Angle(t_rigidbody.velocity, this.transform.forward) < 90 && t_projectSqrMagnitude > mySpeed * mySpeed) {
				//faster than belt

				Debug.Log ("faster");

//				t_rigidbody.AddForce (transform.forward * myBeltFriction * -1, ForceMode.VelocityChange);
			} else {

				Debug.Log ("slower");
				t_rigidbody.AddForce (transform.forward * myBeltFriction, ForceMode.VelocityChange);
			}
		}
	}

    void Update () {
		myBeltMaterial_OffsetY += myBeltSpeed * Time.deltaTime;
		if (myBeltMaterial_OffsetY >= 1) {
			myBeltMaterial_OffsetY -= 1f;
		}
		Vector2 t_matOffset = new Vector2 (0, myBeltMaterial_OffsetY);
		myBeltMaterial.mainTextureOffset = t_matOffset;
	}
}
