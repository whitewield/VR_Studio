using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using AnyBall.Property;

namespace AnyBall {
	namespace Property {
		public class CS_Prop_Base : MonoBehaviour {

			protected virtual void Awake () {
			}

			protected virtual void Start () {
			}

			protected virtual void Update () {
				if (this.transform.position.y < Constants.DISTANCE_RESPAWN_HEIGHT) {
					if (CS_GameManager.Instance != null)
						this.transform.position = CS_GameManager.Instance.GetRandomSpawnArea ().GetRandomPoint ();
				}
			}

			protected virtual void FixedUpdate () {
			}
		}
	}
}
