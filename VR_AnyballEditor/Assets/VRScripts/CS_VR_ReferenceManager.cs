using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_VR_ReferenceManager : MonoBehaviour {
	
	private static CS_VR_ReferenceManager instance = null;
	public static CS_VR_ReferenceManager Instance { get { return instance; } }

	[SerializeField] int myReferenceDefaultCount = 2;
	private List<GameObject> myReferenceList = new List<GameObject> ();
	[SerializeField] GameObject myReferencePrefab;

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
		for (int i = 0; i < myReferenceDefaultCount; i++) {
			GameObject t_newReference = Instantiate (myReferencePrefab, this.transform);
			myReferenceList.Add (t_newReference);
			t_newReference.SetActive (false);

		}
	}

	
	public GameObject GetIdleReference () {
		for (int i = 0; i < myReferenceList.Count; i++) {
			if (myReferenceList [i].activeSelf == false) {
				myReferenceList [i].SetActive (true);
				return myReferenceList [i];
			}
		}

		GameObject t_newReference = Instantiate (myReferencePrefab, this.transform);
		myReferenceList.Add (t_newReference);
		return t_newReference;
	}
}
