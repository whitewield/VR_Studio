using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Property;

namespace AnyBall {
	namespace Property {
		public class CS_Prop_LerpUp : CS_Prop_Base {
			[SerializeField] float myLerpSpeed = 20;

			protected override void FixedUpdate () {
				this.transform.rotation = Quaternion.Lerp (this.transform.rotation,
					this.transform.rotation * Quaternion.FromToRotation (this.transform.up, Vector3.up),
					myLerpSpeed * Time.fixedDeltaTime);

				base.FixedUpdate ();
//				this.transform.up = Vector3.Lerp (this.transform.up, Vector3.up, myLerpSpeed * Time.fixedDeltaTime);
			}
		}
	}
}
