using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Editor;
using Global;

public class CS_VR_LevelManager : MonoBehaviour {
	
	private static CS_VR_LevelManager instance = null;
	public static CS_VR_LevelManager Instance { get { return instance; } }

	private const string FILENAME_DEFAULT = "new any map";

	private string myFileName = FILENAME_DEFAULT;
	private CS_AnyLevelSave_Level myLevel;

	[SerializeField] Transform myPrimitiveParent;
	[SerializeField] Transform myObjectParent;
	[SerializeField] Transform myInvisibleParent;
	private Dictionary<string,GameObject> myDictionary = new Dictionary<string, GameObject> ();

	[SerializeField] Material myEmissionMaterial;
	public Material EmissionMaterial { get { return myEmissionMaterial; } }

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
//		DontDestroyOnLoad(this.gameObject);

//		Time.timeScale = 0;

		myDictionary = Functions.GetAnyLevelDictionary ();

		//				myInputField_FileName.text = myFileName;
		//				foreach (KeyValuePair<string,GameObject> f_pair in myDictionary) {
		//					//Now you can access the key and value both separately from this attachStat as:
		//					Debug.Log (f_pair.Key + ":" + f_pair.Value);
		//				}
		//
		//				Debug.Log ("Awake!");

	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.S) && Input.GetKeyDown (KeyCode.LeftCommand)) {
			OnButtonSave ();
		}


		//				if (Input.GetKeyDown (KeyCode.L) && Input.GetKeyDown (KeyCode.LeftCommand)) {
		//					
		//				}
	}

	public void OnButtonLoad () {
		myLevel = CS_AnyLevelSave.LoadFile (GetFileName ());
		if (myLevel != null) {
			LoadLevelToScene ();
		}
	}

	public void OnButtonDelete () {
		myLevel = null;
		ClearScene ();

		CS_AnyLevelSave.DeleteFile (GetFileName ());

		myFileName = FILENAME_DEFAULT;
		//				myInputField_FileName.text = FILENAME_DEFAULT;
	}

	public void OnButtonSave () {
		myLevel = StoreSceneToLevel ();
		Debug.Log (myLevel.anyObjects.Length);
		CS_AnyLevelSave.DeleteFile (myFileName);
		myFileName = System.DateTime.UtcNow.ToString ("HH:mm-dd-MMMM-yyyy");
		CS_AnyLevelSave.SaveFile (GetFileName (), myLevel);
	}

	public void OnButtonSaveAs () {
		myLevel = StoreSceneToLevel ();
		Debug.Log (myLevel.anyObjects.Length);
		myFileName = System.DateTime.UtcNow.ToString ("HH:mm-dd-MMMM-yyyy");
		CS_AnyLevelSave.SaveFile (GetFileName (), myLevel);
	}

	private void ClearScene () {
		for (int i = 0; i < myPrimitiveParent.childCount; i++) {
			Destroy (myPrimitiveParent.GetChild (i).gameObject);
		}

		for (int i = 0; i < myObjectParent.childCount; i++) {
			Destroy (myObjectParent.GetChild (i).gameObject);
		}

		for (int i = 0; i < myInvisibleParent.childCount; i++) {
			Destroy (myInvisibleParent.GetChild (i).gameObject);
		}
	}

	private void LoadLevelToScene () {
		// clear the current scene
		ClearScene ();

		foreach (CS_AnyLevelSave_Object f_SaveObject in myLevel.anyObjects) {
			GameObject t_prefab = myDictionary [f_SaveObject.prefabName];
			CS_AnyLevelObject t_anyLevelObject = t_prefab.GetComponent<CS_AnyLevelObject> ();

			GameObject t_GameObject = Instantiate (t_prefab);

			//set name
			t_GameObject.name = f_SaveObject.name;
			t_anyLevelObject.SetMyPrefabName (f_SaveObject.prefabName);

			//set parent
			t_GameObject.transform.SetParent (GetParent(t_anyLevelObject.GetMyCategory ()));

			//set transform
			t_GameObject.transform.localPosition = (Vector3)(f_SaveObject.position);
			t_GameObject.transform.localScale = (Vector3)(f_SaveObject.scale);
			t_GameObject.transform.localRotation = (Quaternion)(f_SaveObject.rotation);
		}
	}

	private CS_AnyLevelSave_Level StoreSceneToLevel () {
		List<CS_AnyLevelSave_Object> t_objects = new List<CS_AnyLevelSave_Object> ();

		for (int i = 0; i < myPrimitiveParent.childCount; i++) {
			GameObject t_gameObject = myPrimitiveParent.GetChild (i).gameObject;

			t_objects.Add (CS_AnyLevelSave.GameObjectToSaveObject (t_gameObject));
		}

		for (int i = 0; i < myObjectParent.childCount; i++) {
			GameObject t_gameObject = myObjectParent.GetChild (i).gameObject;

			t_objects.Add (CS_AnyLevelSave.GameObjectToSaveObject (t_gameObject));
		}

		for (int i = 0; i < myInvisibleParent.childCount; i++) {
			GameObject t_gameObject = myInvisibleParent.GetChild(i).gameObject;

			t_objects.Add(CS_AnyLevelSave.GameObjectToSaveObject(t_gameObject));
		}

		return new CS_AnyLevelSave_Level (t_objects);
	}

	public string GetFileName () {
		return myFileName;
	}

	public void SetFileName () {
		//				myFileName = myInputField_FileName.text;
		Debug.Log ("Now editing: " + myFileName);
	}

	public Transform GetParent (Category g_category) {
		if (g_category == Category.Primitive) {
			return myPrimitiveParent;
		} else if (g_category == Category.Object) {
			return myObjectParent;
		} else if (g_category == Category.Invisible) {
			return myInvisibleParent;
		}
		return this.transform;
	}
}
