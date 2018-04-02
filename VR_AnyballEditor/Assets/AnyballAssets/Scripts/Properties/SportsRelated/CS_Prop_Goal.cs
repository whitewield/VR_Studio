using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Property;
using AnyBall.Rule;

namespace AnyBall {
	namespace Property {
		public class CS_Prop_Goal : CS_Prop_Base {
			private List<CS_Rule> myRules = new List<CS_Rule> ();

			public void AddRule (CS_Rule g_rule){
				myRules.Add (g_rule);
			}

			void OnTriggerEnter (Collider g_collider) {
				foreach (CS_Rule f_rule in myRules) {
					f_rule.Enter (g_collider.gameObject, this.gameObject);
				}
			}

			void OnTriggerExit (Collider g_collider) {
				foreach (CS_Rule f_rule in myRules) {
					f_rule.Exit (g_collider.gameObject, this.gameObject);
				}
			}
		}
	}
}
