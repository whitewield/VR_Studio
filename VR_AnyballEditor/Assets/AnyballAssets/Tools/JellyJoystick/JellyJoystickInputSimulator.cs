using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hang.JellyJoystick;

namespace Hang {
	namespace JellyJoystick {

		[CreateAssetMenu(fileName = "JellyJoystickInputSimulator", menuName = "Hang/JellyJoystick/InputSimulator", order = 3)]
		public class JellyJoystickInputSimulator : ScriptableObject {
			
			[System.Serializable]
			public struct Button {
				public JoystickButton myJoystickButton;
				public KeyCode myKeyCode;
			}; 

			[System.Serializable]
			public struct Axis {
				public JoystickAxis myJoystickAxis;
				public KeyCode myKeyCodeNegative;
				public KeyCode myKeyCodePositive;
			}; 

			public int myJoystickNumber = 0;

			public Button[] myButtons;
			private Dictionary<JoystickButton, Button> myButtonDictionary;
			public Axis[] myAxes;
			private Dictionary<JoystickAxis, Axis> myAxisDictionary;

			void OnEnable () {
				myAxisDictionary = new Dictionary<JoystickAxis, Axis> ();
				foreach (Axis f_axis in myAxes) {
					myAxisDictionary.Add (f_axis.myJoystickAxis, f_axis);
				}

				myButtonDictionary = new Dictionary<JoystickButton, Button> ();
				foreach (Button f_button in myButtons) {
					myButtonDictionary.Add (f_button.myJoystickButton, f_button);
				}
			}

			public bool GetButton (ButtonMethodName g_input, JoystickButton g_button) {
				if (myButtonDictionary == null || !myButtonDictionary.ContainsKey (g_button))
					return false;

				if (g_input == ButtonMethodName.Down)
					return Input.GetKeyDown (myButtonDictionary [g_button].myKeyCode);
				if (g_input == ButtonMethodName.Hold)
					return Input.GetKey (myButtonDictionary [g_button].myKeyCode);
				if (g_input == ButtonMethodName.Up)
					return Input.GetKeyUp (myButtonDictionary [g_button].myKeyCode);	
				return false;
			}

			public float GetAxis (JoystickAxis g_axis) {
				if (myAxisDictionary == null || !myAxisDictionary.ContainsKey (g_axis))
					return 0;

				float t_value = 0;
				if (Input.GetKey (myAxisDictionary [g_axis].myKeyCodeNegative))
					t_value -= 1;
				if (Input.GetKey (myAxisDictionary [g_axis].myKeyCodePositive))
					t_value += 1;
				return t_value;
			}

		}
			
	}
}