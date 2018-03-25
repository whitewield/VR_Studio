using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global {

	public enum GameStatus {
		Play,
		Prepare,
		Stop,
	}

	public static class Functions {
		
		public static Dictionary<string,GameObject> GetAnyLevelDictionary () {
			Dictionary<string,GameObject> t_dictionary = new Dictionary<string, GameObject> ();

			GameObject[] t_primitivePrefabs = Resources.LoadAll<GameObject> (Constants.PATH_MAP_PRIMITIVES);
			foreach (GameObject f_prefab in t_primitivePrefabs) {
				t_dictionary.Add (f_prefab.name, f_prefab);
			}

			GameObject[] t_objectPrefabs = Resources.LoadAll<GameObject> (Constants.PATH_MAP_OBJECTS);
			foreach (GameObject f_prefab in t_objectPrefabs) {
				t_dictionary.Add (f_prefab.name, f_prefab);
			}

			GameObject[] t_invisiblePrefabs = Resources.LoadAll<GameObject> (Constants.PATH_MAP_INVISIBLES);
			foreach (GameObject f_prefab in t_invisiblePrefabs) {
				t_dictionary.Add (f_prefab.name, f_prefab);
			}

			return t_dictionary;
		}
	}

	public class Constants {

		public const string PATH_OBJECTS = "PrefabsLoad/Objects/";
		public const string PATH_RULES = "PrefabsLoad/Rules/";

		public const string PATH_MAP_PRIMITIVES = "Map/Primitives";
		public const string PATH_MAP_OBJECTS = "Map/Objects";
		public const string PATH_MAP_INVISIBLES = "Map/Invisibles";

		public const string NAME_PROP_BASE = "AnyBall.Property.CS_Prop_";
		public const string NAME_PROP_GOAL = "Goal";
		public const string NAME_PROP_GOAL_CONTAINER = "Goal_Container";
		public const string NAME_PROP_GOAL_HOOP = "Goal_Hoop";
		public const string NAME_PROP_GOAL_REACH = "Goal_Reach";
		public const string NAME_PROP_AREA = "Area";
		public const string NAME_PROP_BALL = "Ball";
		public const string NAME_PROP_FLAG = "Flag";
		public const string NAME_PROP_WEARABLE = "Wearable";

		public const float DISTANCE_INIT_HEIGHT = 5;

		public const float AREA_TRIGGER_HEIGHT = 100;
		public const float AREA_COLLIDER_HEIGHT = 0.05f;

		public const float MATERIAL_TILING_CONVEYORBELT = 41.4f;

		public const float TIME_PLAY = 60f;
		public const float TIME_PREPARE = 10f;

		public const int NUMBER_MAX_TEAM = 4;


	}
}
