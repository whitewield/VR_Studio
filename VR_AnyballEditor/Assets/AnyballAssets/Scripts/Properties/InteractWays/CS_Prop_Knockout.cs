using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Property;

namespace AnyBall {
	namespace Property {

		public enum KnockoutType {
			Stun,
			Drop
		}

		public class CS_Prop_Knockout : CS_Prop_Base {
			[SerializeField] KnockoutType myKnockoutType = KnockoutType.Stun;

			/// <summary>
			/// My scale for sqrImpulse. 0 -> never knockout. 1000 -> super easy to knockout 
			/// </summary>
			[Range (0, 1000)]
			[SerializeField] float mySqrImpulseScale = 1;
			public float GetSqrImpulseScale () {
				return mySqrImpulseScale;
			}

			public KnockoutType GetKnockoutType () {
				return myKnockoutType;
			}
		}
	}
}
