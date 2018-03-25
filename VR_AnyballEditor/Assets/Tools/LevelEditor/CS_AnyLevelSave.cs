using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AnyBall {
	namespace Editor {

		public static class CS_AnyLevelSave {

			public const string FOLDERNAME = "Maps";

			public const string FILENAME_EXTENSION = ".am";

			public static CS_AnyLevelSave_Level LoadFile (string g_fileName) {
				
				string t_filePath = GetFilePath (g_fileName);

				Debug.Log ("LoadFile");

				if (File.Exists (t_filePath)) {
					string t_dataAsJson = File.ReadAllText (t_filePath);
					return JsonUtility.FromJson<CS_AnyLevelSave_Level> (t_dataAsJson);
				} else {
					Debug.LogWarning ("cannot load any level! : " + t_filePath);
				}

				return null;
			}

			public static void SaveFile (string g_fileName, CS_AnyLevelSave_Level g_level) {
				
				string t_filePath = GetFilePath (g_fileName);

				Debug.Log ("SaveFile");

				if (!File.Exists (t_filePath))
					File.Create (t_filePath).Dispose ();

				string t_dataAsJson = JsonUtility.ToJson(g_level);
				File.WriteAllText (t_filePath, t_dataAsJson);
			}

			public static void DeleteFile (string g_fileName) {
				
				string t_filePath = GetFilePath (g_fileName);

				Debug.Log ("DeleteFile");

				if (File.Exists (t_filePath))
					File.Delete (t_filePath);
			}

			private static string GetFilePath (string g_fileName) {
				string t_directoryPath = Application.dataPath + Path.DirectorySeparatorChar + FOLDERNAME;
				if (!Directory.Exists (t_directoryPath))
					Directory.CreateDirectory (t_directoryPath);

				return t_directoryPath + Path.DirectorySeparatorChar + g_fileName + FILENAME_EXTENSION;
			}

			public static CS_AnyLevelSave_Object GameObjectToSaveObject (GameObject g_gameObject) {
				CS_AnyLevelSave_Object t_saveObject = new CS_AnyLevelSave_Object ();
				t_saveObject.prefabName = g_gameObject.GetComponent<CS_AnyLevelObject> ().GetMyPrefabName ();
				t_saveObject.name = g_gameObject.name;
				t_saveObject.position = new CS_AnyLevelSave_Vector3 (g_gameObject.transform.localPosition);
				t_saveObject.scale = new CS_AnyLevelSave_Vector3 (g_gameObject.transform.localScale);
				t_saveObject.rotation = new CS_AnyLevelSave_Vector4 (g_gameObject.transform.localRotation);

				return t_saveObject;
			}
				
		}

		[System.Serializable]
		public class CS_AnyLevelSave_Level {
			public CS_AnyLevelSave_Object[] anyObjects;

			public CS_AnyLevelSave_Level (List<CS_AnyLevelSave_Object> g_list) {
				anyObjects = g_list.ToArray ();
			}
		}

		[System.Serializable]
		public class CS_AnyLevelSave_Object {
			public string prefabName;
			public string name;
			public CS_AnyLevelSave_Vector3 position;
			public CS_AnyLevelSave_Vector3 scale;
			public CS_AnyLevelSave_Vector4 rotation;
		}

		[System.Serializable]
		public class CS_AnyLevelSave_Vector3 {
			public float x;
			public float y;
			public float z;

			public CS_AnyLevelSave_Vector3 (float g_x, float g_y, float g_z) {
				x = g_x;
				y = g_y;
				z = g_z;
			}

			public CS_AnyLevelSave_Vector3 (Vector3 g_vector3) {
				x = g_vector3.x;
				y = g_vector3.y;
				z = g_vector3.z;
			}

			public static implicit operator Vector3 (CS_AnyLevelSave_Vector3 g_value) {
				return new Vector3 (g_value.x, g_value.y, g_value.z);
			}
		}

		[System.Serializable]
		public class CS_AnyLevelSave_Vector4 {
			public float x;
			public float y;
			public float z;
			public float w;

			public CS_AnyLevelSave_Vector4 (float g_x, float g_y, float g_z, float g_w) {
				x = g_x;
				y = g_y;
				z = g_z;
				w = g_w;
			}

			public CS_AnyLevelSave_Vector4 (Vector4 g_vector4) {
				x = g_vector4.x;
				y = g_vector4.y;
				z = g_vector4.z;
				w = g_vector4.w;
			}

			public CS_AnyLevelSave_Vector4 (Quaternion g_quaternion) {
				x = g_quaternion.x;
				y = g_quaternion.y;
				z = g_quaternion.z;
				w = g_quaternion.w;
			}

			public static implicit operator Vector4 (CS_AnyLevelSave_Vector4 g_value) {
				return new Vector4 (g_value.x, g_value.y, g_value.z, g_value.w);
			}

			public static implicit operator Quaternion (CS_AnyLevelSave_Vector4 g_value) {
				return new Quaternion (g_value.x, g_value.y, g_value.z, g_value.w);
			}
		}
	}
}
