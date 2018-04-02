using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AnyBall {
	namespace Property {
		public class CS_Prop_Button : CS_Prop_Base {
			public enum ButtonType {
				Quit,
				Continue,
			}

			[SerializeField] ButtonType myButtonType;
			public ButtonType MyButtonType { get { return myButtonType; } }
		}
	}
}
