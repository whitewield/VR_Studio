using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using UnityEngine.UI;

namespace AnyBall {
	namespace Editor {

		public class CS_AnyLevelEditor : MonoBehaviour {

			private const string FILENAME_DEFAULT = "new any map";

			private string myFileName = FILENAME_DEFAULT;
			private CS_AnyLevelSave_Level myLevel;

			[SerializeField] Transform myPrimitiveParent;
			[SerializeField] Transform myObjectParent;
			[SerializeField] Transform myInvisibleParent;
			private Dictionary<string,GameObject> myDictionary = new Dictionary<string, GameObject> ();

			[Header ("UI")]
			[SerializeField] InputField myInputField_FileName;

			void Awake () {
				Time.timeScale = 0;

				myDictionary = Functions.GetAnyLevelDictionary ();

				myInputField_FileName.text = myFileName;
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
				myInputField_FileName.text = FILENAME_DEFAULT;
			}

			public void OnButtonSave () {
				myLevel = StoreSceneToLevel ();
				Debug.Log (myLevel.anyObjects.Length);
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
					if (t_anyLevelObject.GetMyCategory () == Category.Primitive) {
						t_GameObject.transform.SetParent (myPrimitiveParent);
					} else if (t_anyLevelObject.GetMyCategory () == Category.Object) {
						t_GameObject.transform.SetParent (myObjectParent);
					} else if (t_anyLevelObject.GetMyCategory () == Category.Invisible) {
						t_GameObject.transform.SetParent (myInvisibleParent);
					}

					//set transform
					t_GameObject.transform.position = (Vector3)(f_SaveObject.position);
					t_GameObject.transform.localScale = (Vector3)(f_SaveObject.scale);
					t_GameObject.transform.rotation = (Quaternion)(f_SaveObject.rotation);
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
				myFileName = myInputField_FileName.text;
				Debug.Log ("Now editing: " + myFileName);
			}
		}


	}
}
