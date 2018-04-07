using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Map : MonoBehaviour {

	private static CS_Map instance = null;

	public static CS_Map Instance {
		get { 
			return instance;
		}
	}

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}


		mySpawnPositions = new List<Transform> (this.gameObject.GetComponentsInChildren<Transform> ());
		mySpawnPositions.Remove (this.transform);
	}

	private List<Transform> mySpawnPositions;
	// Use this for initialization
	void Start () {
	}
	
	public List<Transform> GetSpawnPositions () {
		return mySpawnPositions;
	}
}
