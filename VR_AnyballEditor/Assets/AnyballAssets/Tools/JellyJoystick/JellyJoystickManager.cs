/// <summary>
/// JellyJoystick Ver.0.2 
/// made by Hang Ruan
/// special thanks to Tim
/// =====
/// 20180208 Update, Ver.0.2
///  + add input layout
///  + simplify adding new controller process
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hang.JellyJoystick;

namespace Hang {
	namespace JellyJoystick {

		public enum ButtonMethodName {
			Down,
			Hold,
			Up
		}

		public enum JoystickButton {
			A,
			B,
			X,
			Y,
			LB,
			RB,
			BACK,
			START,
			LS,
			RS
		}

		public enum AxisMethodName {
			Normal,
			Raw
		}

		public enum JoystickAxis {
			LS_X,
			LS_Y,
			RS_X,
			RS_Y,
			LT,
			RT,
		}

		public enum Platform {
			OSX,
			WIN
		}

		public enum MapType {
			NONE,
			OSX_XBOX,
			WIN_XBOX,
			PS4,
			PS3,
			SNES,
		}



		public class JellyJoystickManager : MonoBehaviour {

			private static JellyJoystickManager instance = null;

			//========================================================================
			public static JellyJoystickManager Instance {
				get { 
					return instance;
				}
			}

			void Awake () {
				if (instance != null && instance != this) {
					Destroy(this.gameObject);
				} else {
					instance = this;
				}
				DontDestroyOnLoad(this.gameObject);
			}
			//========================================================================

			public const int NUMBER_MAX_JOYSTICK = 8;

			private delegate bool ButtonMethod (KeyCode t_keyCode);
			private delegate float AxisMethod (string t_axis);

			private Platform myPlatform;
			private MapType[] myMapTypes = new MapType[NUMBER_MAX_JOYSTICK];

			[SerializeField] JellyJoystickInputLayout[] myInputLayoutBank;
			private Dictionary<MapType,JellyJoystickInputLayout> myInputLayoutDictionary;

			[SerializeField] bool useSimulator = false;
			[SerializeField] JellyJoystickInputSimulator[] mySimulators;
			private Dictionary<int,JellyJoystickInputSimulator> mySimulatorDictionary;


			void Start () {
				InitLayoutDictionary ();
				InitSimulators ();
				InitPlatform ();
				UpdateMyControllersType ();
			}

			private void InitSimulators () {
				mySimulatorDictionary = new Dictionary<int, JellyJoystickInputSimulator> ();
				foreach (JellyJoystickInputSimulator f_simulator in mySimulators) {
					mySimulatorDictionary.Add (f_simulator.myJoystickNumber, f_simulator);
				}
			}

			private void InitLayoutDictionary () {
				myInputLayoutDictionary = new Dictionary<MapType, JellyJoystickInputLayout> ();
				foreach (JellyJoystickInputLayout f_layout in myInputLayoutBank) {
					myInputLayoutDictionary.Add (f_layout.myMapType, f_layout);
				}
			}

			private void InitPlatform () {
				if (Application.platform == RuntimePlatform.OSXEditor ||
					Application.platform == RuntimePlatform.OSXPlayer) {
					myPlatform = Platform.OSX;
				} else if (Application.platform == RuntimePlatform.WindowsEditor ||
					Application.platform == RuntimePlatform.WindowsPlayer) {
					myPlatform = Platform.WIN;
				}
			}

			void Update () {

				//		for (int i = 0; i < 20; i++) {
				//			if (Input.GetKeyDown (GetKeyCode (i, 0))) {
				//				Debug.Log ("button " + i + " is down");
				//			}
				//		}
				//
				//		for (int i = 1; i < 12; i++) {
				//			Debug.Log (i + ": " + Input.GetAxis ("Joystick1Axis" + i));
				//		}

			}

			public void UpdateMyControllersType () {
				string[] t_names = Input.GetJoystickNames();

				int number = Mathf.Clamp (t_names.Length, 0, NUMBER_MAX_JOYSTICK);

				for (int i = 0; i < number; i++) {
					myMapTypes [i] = GetControllerType (t_names [i]);
				}
			}

			private MapType GetControllerType (string g_name){
				Debug.Log (g_name);

				if (g_name.Contains ("Microsoft") ||
					g_name.Contains ("Xbox") ||
					g_name.Contains ("XBOX") ||
					g_name.Contains ("360") ||
					g_name.Contains ("one") ||
					g_name.Contains ("ONE")) {
					Debug.Log ("XBOX!");
					if (myPlatform == Platform.OSX)
						return MapType.OSX_XBOX;
					else if (myPlatform == Platform.WIN)
						return MapType.WIN_XBOX;
				}

				if (g_name.Contains ("USB Gamepad")) {
					Debug.Log ("SNES");
					return MapType.SNES;
				}

				if (g_name.Contains ("3")) {
					Debug.Log ("PS3");
					return MapType.PS3;
				}

				if (g_name.Contains ("Wireless Controller")) {
					Debug.Log ("PS4");
					return MapType.PS4;
				}

				return MapType.NONE;
			}

			public JellyJoystickInputLayout GetInputLayout (MapType g_mapType) {
				return myInputLayoutDictionary [g_mapType];
			}

			public JellyJoystickInputLayout GetInputLayout (int g_joystickNumber) {
				return myInputLayoutDictionary [myMapTypes [g_joystickNumber - 1]];
			}

			public float GetAxis (AxisMethodName g_input, int g_joystickNumber, JoystickAxis g_axis) {
				//check if it use simulator
				if (useSimulator) {
					if (mySimulatorDictionary.ContainsKey (g_joystickNumber)) {
						float t_value = mySimulatorDictionary [g_joystickNumber].GetAxis (g_axis);
						if (t_value != 0) {
							return t_value;
						}
					}
				}

				//get the input function
				AxisMethod t_InputFunction;
				if (g_input == AxisMethodName.Normal)
					t_InputFunction = Input.GetAxis;
				else
					t_InputFunction = Input.GetAxisRaw;

				//0 -> all; 1-8 -> joystick1-8 
				g_joystickNumber = Mathf.Clamp (g_joystickNumber, 0, NUMBER_MAX_JOYSTICK);

				if (g_joystickNumber != 0) {

					JellyJoystickInputLayout t_layout = GetInputLayout (g_joystickNumber);

					int t_axisNumber = t_layout.GetAxisNumber (g_axis);
					if (t_axisNumber == 0)
						return 0;

					int t_axisMultiplier = t_layout.GetAxisMultiplier (g_axis);

					return t_InputFunction ("Joystick" + g_joystickNumber + "Axis" + t_axisNumber) * t_axisMultiplier;

				} else {
					for (int i = 1; i <= NUMBER_MAX_JOYSTICK; i++) {
						float t_value = GetAxis (g_input, i, g_axis);
						if (t_value != 0)
							return t_value;
					}
				}
				return 0;
			}

			public bool GetButton (ButtonMethodName g_input, int g_joystickNumber, JoystickButton g_button) {
				//check if it use simulator
				if (useSimulator) {
					if (mySimulatorDictionary.ContainsKey(g_joystickNumber) && 
						mySimulatorDictionary [g_joystickNumber].GetButton (g_input, g_button)) {
						return true;
					}
				}

				//get the input function
				ButtonMethod t_InputFunction;
				if (g_input == ButtonMethodName.Up)
					t_InputFunction = Input.GetKeyUp;
				else if (g_input == ButtonMethodName.Hold)
					t_InputFunction = Input.GetKey;
				else
					t_InputFunction = Input.GetKeyDown;

				//0 -> all; 1-8 -> joystick1-8 
				g_joystickNumber = Mathf.Clamp (g_joystickNumber, 0, NUMBER_MAX_JOYSTICK);

				if (g_joystickNumber != 0) {

					JellyJoystickInputLayout t_layout = GetInputLayout (g_joystickNumber);

					int t_buttonNumber = t_layout.GetButtonNumber (g_button);
					if (t_buttonNumber == -1)
						return false;

					return t_InputFunction (GetKeyCode (t_buttonNumber, g_joystickNumber));

				} else {
					for (int i = 1; i <= NUMBER_MAX_JOYSTICK; i++) {
						if (GetButton (g_input, i, g_button))
							return true;
					}
				}
				return false;
			}

			private KeyCode GetKeyCode (int g_buttonNumber, int g_joystickNumber) {
				return KeyCode.JoystickButton0 + g_buttonNumber + g_joystickNumber * 20;
			}
		}
	}
}
