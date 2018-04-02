using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using AnyBall.Property;

public class CS_EverythingManager : MonoBehaviour {

	private static CS_EverythingManager instance = null;

	public static CS_EverythingManager Instance {
		get { 
			return instance;
		}
	}
		
	private Dictionary<string,GameObject> myAnyLevelDictionary = new Dictionary<string, GameObject> ();

	private GameObject[] myEverything;
	private List<GameObject> myEverythingList;
	[SerializeField] float myStageRadius = 15;

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}

		myEverything = Resources.LoadAll<GameObject> (Constants.PATH_OBJECTS);

		InitEverythingList ();

		InitAnyLevelDictionary ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Vector3 GetRandomPosition (float g_height = 0) {
		Vector3 t_pos = Random.onUnitSphere * myStageRadius;
		t_pos.y = g_height;
		return t_pos;
	}

	public List<GameObject> GetRandomPrefabs (string g_property, int g_count) {
		
		List<GameObject> t_prefabList = new List<GameObject> ();

		//put all the prefabs that have the prop into the list
		foreach (GameObject f_prefab in myEverythingList) {
			System.Type f_type = System.Type.GetType (Constants.NAME_PROP_BASE + g_property);

			if (f_type == null) {
				Debug.LogError ("cannot find type: " + Constants.NAME_PROP_BASE + g_property);
				return null;
			}

			//check the object and its children
			if (f_prefab.GetComponentInChildren (System.Type.GetType (Constants.NAME_PROP_BASE + g_property)) != null) {
//				Debug.Log (f_prefab.name);
				t_prefabList.Add (f_prefab);
			}
		}

		//remove the extra amount
		int t_removeCount = t_prefabList.Count - g_count;
		if (t_removeCount < 0) {
			Debug.LogWarning ("cannot find " + g_count.ToString () + " " + g_property + " objects");
		} else {
			for (int i = 0; i < t_removeCount; i++) {
				t_prefabList.RemoveAt (Random.Range (0, t_prefabList.Count));
			}
		}

		foreach (GameObject f_prefab in t_prefabList) {
			myEverythingList.Remove (f_prefab);
		}

		return t_prefabList;
	}

	public GameObject GetRandomPrefab (string g_property) {
		
		List<GameObject> t_prefabList = new List<GameObject> ();

		//put all the prefabs that have the prop into the list
		foreach (GameObject f_prefab in myEverythingList) {
			System.Type f_type = System.Type.GetType (Constants.NAME_PROP_BASE + g_property);

			if (f_type == null) {
				Debug.LogError ("cannot find type: " + Constants.NAME_PROP_BASE + g_property);
				return null;
			}

			//check the object and its children
			if (f_prefab.GetComponentInChildren (System.Type.GetType (Constants.NAME_PROP_BASE + g_property)) != null) {
//				Debug.Log (f_prefab.name);
				t_prefabList.Add (f_prefab);
			}
		}

		GameObject t_prefab = t_prefabList [Random.Range (0, t_prefabList.Count)];
//		if (t_prefabList.Count > 1)
//			myEverythingList.Remove (t_prefab);

		//remove a random prefab from the list
		return t_prefab;
	}

	public void InitEverythingList () {
		if (myEverythingList != null)
			myEverythingList.Clear ();
		myEverythingList = new List<GameObject> (myEverything);
	}

	public void InitAnyLevelDictionary () {
		myAnyLevelDictionary = Functions.GetAnyLevelDictionary ();
	}

	public GameObject GetAnyLevelPrefab (string g_prefabName) {
//		Debug.Log ("TryGet: " + g_prefabName);
		return myAnyLevelDictionary [g_prefabName];
	}
}
