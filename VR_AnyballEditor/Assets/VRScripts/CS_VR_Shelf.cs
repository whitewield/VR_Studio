using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Editor;

public class CS_VR_Shelf : MonoBehaviour {
	[SerializeField] GameObject[] myPrefabs;
	private List<GameObject> myObjects = new List<GameObject> ();

	[SerializeField] int myColumnCount = 6;
	[SerializeField] float myObjectSize = 0.1f;
	[SerializeField] float myObjectDistance = 2f; 
	[SerializeField] Vector3 myFacingDirection;

	// Use this for initialization
	void Start () {

		foreach (GameObject f_prefab in myPrefabs) {
			GameObject f_object = Instantiate (f_prefab, this.transform);
			myObjects.Add (f_object);

			// disable the CS_AnyLevelObject script;
			f_object.GetComponent<CS_AnyLevelObject> ().enabled = false;
			Destroy (f_object.GetComponent<CS_AnyLevelObject> ());

			f_object.GetComponent<CS_VR_Object> ().enabled = false;
			Destroy (f_object.GetComponent<CS_VR_Object> ());

			f_object.GetComponent<CS_VR_Shelf_Object> ().Prefab = f_prefab;
		}

		InitObjects_Position ();

		for (int i = 0; i < myObjects.Count; i++) {
			myObjects [i].transform.localScale = Vector3.one * myObjectSize;
		}

		this.transform.forward = myFacingDirection;
	}

	private void InitObjects_Position () {
		float t_worldDistance = myObjectSize * myObjectDistance;

		int t_rowCount = myObjects.Count / myColumnCount + 1;

		Vector2 t_topLeftPosition = 
			new Vector2 (-(myColumnCount - 1) * t_worldDistance * 0.5f, (t_rowCount - 1) * t_worldDistance * 0.5f);

		// move objects
		for (int y = 0; y < t_rowCount; y++) {
			for (int x = 0; x < myColumnCount; x++) {
				int f_index = y * myColumnCount + x;
				if (f_index >= myObjects.Count)
					break;
				myObjects [f_index].transform.localPosition = 
					t_topLeftPosition + new Vector2 (x * t_worldDistance, y * t_worldDistance * -1);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
