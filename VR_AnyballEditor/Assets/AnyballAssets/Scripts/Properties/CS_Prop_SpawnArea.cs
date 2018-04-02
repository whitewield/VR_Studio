using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyBall {
	namespace Property {
		public class CS_Prop_SpawnArea : CS_Prop_Base {

			public enum Type {
				Object,
				Player,
			}

			[SerializeField] Type mySpawnType = Type.Object;
			public Type MySpawnType { get { return mySpawnType; } }

			public float GetSize () {
				return this.transform.localScale.x / 2;
			}

			public Vector3 GetRandomPoint () {
				Vector2 t_v2 = Random.insideUnitCircle * GetSize ();
				return (new Vector3 (t_v2.x, 0, t_v2.y) + this.transform.position + Vector3.up * Random.Range (-1, 1));
			}

			public Vector3[] GetRandomPoints (int g_count) {
				float t_baseAngle = Random.Range (0, 360f);
				float t_deltaAngle = 360f / g_count;

				Vector3[] t_points = new Vector3[g_count];
				//move the goal and look at center
				for (int i = 0; i < g_count; i++) {
					t_points [i] = 
						Quaternion.AngleAxis (t_baseAngle + t_deltaAngle * i, Vector3.up) * Vector3.forward * GetSize () +
						this.transform.position;
				}
				return t_points;
			}
		}
	}
}
