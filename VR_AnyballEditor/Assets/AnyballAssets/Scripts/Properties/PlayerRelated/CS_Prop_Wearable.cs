using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Property;
using AnyBall.Rule;

namespace AnyBall {
	namespace Property {
		public class CS_Prop_Wearable : CS_Prop_Base {
			private List<CS_Rule> myRules = new List<CS_Rule> ();

			private CS_PlayerControl myHolder;

			public void AddRule (CS_Rule g_rule){
				myRules.Add (g_rule);
			}

			public virtual void SetTeamNumber (int g_teamNumber) {

			}

			public void SetMyHolder (CS_PlayerControl g_holder) {
				if (myHolder != null)
					myHolder.DropWearing ();
				myHolder = g_holder;
				foreach (CS_Rule f_rule in myRules) {
					f_rule.Enter (g_holder.gameObject, this.gameObject);
				}
			}

			public void Drop (CS_PlayerControl g_holder) {
				myHolder = null;
				foreach (CS_Rule f_rule in myRules) {
					f_rule.Exit (g_holder.gameObject, this.gameObject);
				}
			}
		}
	}
}
