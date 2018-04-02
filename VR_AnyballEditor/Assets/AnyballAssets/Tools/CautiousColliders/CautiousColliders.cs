using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CautiousColliders : MonoBehaviour {

	enum ColliderType {
		Box,
		Cap
	}

	enum Axis {
		X,
		Y,
		Z
	}
	
	[SerializeField] bool active = false;

	[SerializeField] bool clear = false;

	[SerializeField] float myRadius = 5;
	[SerializeField] Axis myAxis = Axis.Y;
	[SerializeField] string mySegmentName = "CautiousCollider";
	[SerializeField] int mySegmentCount = 8;
	[SerializeField] Vector3 mySegmentSize = Vector3.one;

	[SerializeField] List<GameObject> mySegmentList;

	// Use this for initialization
	void Start () {
		
	}
	
	void Update () {
		if (active == true) {
			clear = false;
			UpdateRing ();
		}

		if (clear == true) {
			active = false;
			clear = false;

			foreach (GameObject f_Segment in mySegmentList) {
				DestroyImmediate (f_Segment);
			}
			mySegmentList.Clear ();
		}
	}

	void UpdateRing () {
		if (mySegmentList == null) {
			mySegmentList = new List<GameObject> ();
		}

		if (mySegmentCount <= 0) {
			Debug.LogWarning ("the segment count need to be bigger than 0");
			return;
		}

		while (mySegmentList.Count != mySegmentCount) {
			if (mySegmentList.Count > mySegmentCount) {
				DestroyImmediate (mySegmentList [mySegmentList.Count - 1]);
				mySegmentList.RemoveAt (mySegmentList.Count - 1);
			} else {
				GameObject w_segment = new GameObject (mySegmentName + " (" + mySegmentList.Count.ToString ("0") + ")");
				w_segment.transform.SetParent (this.transform);
				w_segment.transform.localPosition = Vector3.zero;
				mySegmentList.Add (w_segment);
			}
		}

		Vector3 f_center = new Vector3 ();
		Vector3 f_euler = Vector3.zero;
		switch (myAxis) {
		case Axis.X:
			f_center = Vector3.up;
			f_euler = new Vector3 (360f / mySegmentCount, 0, 0);
			break;
		case Axis.Y:
			f_center = Vector3.left;
			f_euler = new Vector3 (0, 360f / mySegmentCount, 0);
			break;
		case Axis.Z:
			f_center = Vector3.left;
			f_euler = new Vector3 (0, 0, 360f / mySegmentCount);
			break;
		}
		f_center *= myRadius;

		for (int i = 0; i < mySegmentList.Count; i++) {
			BoxCollider f_collider = mySegmentList[i].GetComponent<BoxCollider> ();

			if (f_collider == null) {
				f_collider = mySegmentList[i].AddComponent<BoxCollider> ();
			}

			f_collider.size = mySegmentSize;
			f_collider.center = f_center;
            mySegmentList [i].transform.localRotation = Quaternion.Euler (f_euler * i);
		}
	}
}
