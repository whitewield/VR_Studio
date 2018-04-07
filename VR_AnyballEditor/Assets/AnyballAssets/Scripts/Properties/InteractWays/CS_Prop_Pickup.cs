using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Property;

namespace AnyBall {
	namespace Property {
		public class CS_Prop_Pickup : CS_Prop_Base {

			[SerializeField] bool isBig = false;
			public bool IsBig { get { return isBig; } }

			private GameObject myHolder;
			public GameObject MyHolder { 
				get {
					return myHolder;
				}
				set { 
					myHolder = value;
					myHolderEndTime = Time.timeSinceLevelLoad + myHolderTime; 
				} 
			}

			private float myHolderTime = 1;
			private float myHolderEndTime;

			private bool onHold;
			public bool OnHold { 
				get { 
					return onHold;
				} 
				set {
					onHold = value;
				}
			}

			protected override void Update () {
				if (!onHold && Time.timeSinceLevelLoad > myHolderEndTime) {
					myHolder = null;
				}

				base.Update ();
			}

		}
	}
}
