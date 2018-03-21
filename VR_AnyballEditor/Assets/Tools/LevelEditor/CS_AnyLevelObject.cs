using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyBall {
	namespace Editor {
		public enum Category {
			Primitive,
			Object,
			Invisible,
		}

		public class CS_AnyLevelObject : MonoBehaviour {

			private string myPrefabName = "";
			[SerializeField] protected Category myCategory;

			// Use this for initialization
			protected virtual void Start () {
				if (myPrefabName == "") {
					string[] t_substrings = this.name.Split (' ');
					myPrefabName = t_substrings [0];
				}
			}

			public void SetMyPrefabName (string g_name) {
				myPrefabName = g_name;
			}

			public string GetMyPrefabName () {
				return myPrefabName;
			}

			public Category GetMyCategory () {
				return myCategory;
			}
//			public CS_AnyObject ToAnyObject
		}
	}
}
