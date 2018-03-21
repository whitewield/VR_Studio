using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyBall {
	namespace Editor {

		public class CS_AnyLevelObject_Base : CS_AnyLevelObject {

			void Awake () {
				myCategory = Category.Primitive;
			}

			void LateUpdate () {
				this.transform.localPosition = new Vector3 (this.transform.localPosition.x, -0.5f, this.transform.localPosition.z);
				this.transform.localScale = new Vector3 (this.transform.localScale.x, 1f, this.transform.localScale.z);
				this.transform.localRotation = Quaternion.Euler (0, this.transform.localRotation.eulerAngles.y, 0);
			}

		}
	}
}

