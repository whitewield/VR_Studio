using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hang.JellyJoystick;

namespace Hang {
	namespace JellyJoystick {

		[CreateAssetMenu(fileName = "JellyJoystickInputLayout", menuName = "Hang/JellyJoystick/InputLayout", order = 2)]
		public class JellyJoystickInputLayout : ScriptableObject {
			public MapType myMapType;

			public AxisEntry[] myAxes;
			private Dictionary<JoystickAxis, int> myAxisDictionary;
			private Dictionary<JoystickAxis, int> myAxisMultiplierDictionary;

			public ButtonEntry[] myButtons;
			private Dictionary<JoystickButton, int> myButtonDictionary;

			void OnEnable () {
				myAxisDictionary = new Dictionary<JoystickAxis, int> ();
				foreach (AxisEntry f_axisEntry in myAxes) {
					myAxisDictionary.Add (f_axisEntry.joystickAxis, f_axisEntry.axisNumber);
				}

				myAxisMultiplierDictionary = new Dictionary<JoystickAxis, int> ();
				foreach (AxisEntry f_axisEntry in myAxes) {
					if (f_axisEntry.invert)
						myAxisMultiplierDictionary.Add (f_axisEntry.joystickAxis, -1);
					else
						myAxisMultiplierDictionary.Add (f_axisEntry.joystickAxis, 1);
				}

				myButtonDictionary = new Dictionary<JoystickButton, int> ();
				foreach (ButtonEntry f_buttonEntry in myButtons) {
					myButtonDictionary.Add (f_buttonEntry.joystickButton, f_buttonEntry.buttonNumber);
				}
			}

			public int GetAxisNumber (JoystickAxis g_axis) {
				if (myAxisDictionary == null || !myAxisDictionary.ContainsKey (g_axis))
					return 0;
				return myAxisDictionary [g_axis];
			}

			public int GetAxisMultiplier (JoystickAxis g_axis) {
				if (myAxisMultiplierDictionary == null || !myAxisMultiplierDictionary.ContainsKey (g_axis))
					return 0;
				return myAxisMultiplierDictionary [g_axis];
			}

			public int GetButtonNumber (JoystickButton g_button) {
				if (myButtonDictionary == null || !myButtonDictionary.ContainsKey (g_button))
					return -1;
				return myButtonDictionary [g_button];
			}
		}

		[System.Serializable]
		public struct AxisEntry {
			public JoystickAxis joystickAxis;
			public int axisNumber;
			public bool invert;
		}

		[System.Serializable]
		public struct ButtonEntry {
			public JoystickButton joystickButton;
			public int buttonNumber;
		}
			
	}
}