//http://plyoung.appspot.com/blog/manipulating-input-manager-in-script.html

#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]

public class JellyJoystickManipulatingAxis : MonoBehaviour {
	
	[SerializeField] bool setupAxis = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (setupAxis == true) {
			SetupInputManager ();
			setupAxis = false;
		}
	}

	private static SerializedProperty GetChildProperty(SerializedProperty parent, string name) {
		SerializedProperty child = parent.Copy ();
		child.Next (true);
		do {
			if (child.name == name)
				return child;
		} while (child.Next (false));
		return null;
	}

	private static bool AxisDefined(string axisName)
	{
		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

		axesProperty.Next(true);
		axesProperty.Next(true);
		while (axesProperty.Next(false))
		{
			SerializedProperty axis = axesProperty.Copy();
			axis.Next(true);
			if (axis.stringValue == axisName) return true;
		}
		return false;
	}

	public enum AxisType
	{
		KeyOrMouseButton = 0,
		MouseMovement = 1,
		JoystickAxis = 2
	};

	public class InputAxis
	{
		public string name;
		public string descriptiveName;
		public string descriptiveNegativeName;
		public string negativeButton;
		public string positiveButton;
		public string altNegativeButton;
		public string altPositiveButton;

		public float gravity;
		public float dead;
		public float sensitivity;

		public bool snap = false;
		public bool invert = false;

		public AxisType type;

		public int axis;
		public int joyNum;
	}

	private static void AddAxis(InputAxis axis)
	{
		if (AxisDefined(axis.name)) return;

		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

		axesProperty.arraySize++;
		serializedObject.ApplyModifiedProperties();

		SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

		GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
		GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
		GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
		GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
		GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
		GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
		GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
		GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
		GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
		GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
		GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
		GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
		GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
		GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
		GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

		serializedObject.ApplyModifiedProperties();
	}

	public static void SetupInputManager()
	{
		// Add mouse definitions
//		AddAxis(new InputAxis() { name = "myMouseX",        sensitivity = 1f, type = AxisType.MouseMovement, axis = 1 });
//		AddAxis(new InputAxis() { name = "myMouseY",        sensitivity = 1f, type = AxisType.MouseMovement, axis = 2 });
//		AddAxis(new InputAxis() { name = "myScrollWheel", sensitivity = 1f, type = AxisType.MouseMovement, axis = 3 });

		// Add gamepad definitions
		int i = 1;
		for (i = 1; i <= 8; i++) {
			for (int j = 0; j <= 10; j++) {
				AddAxis(new InputAxis() 
					{ 
						name = "Joystick" + i + "Axis" + (j + 1).ToString(), 
						dead = 0.19f,
						sensitivity = 1f,
						type = AxisType.JoystickAxis,
						axis = (j + 1),
						joyNum = i,
					});
			}
		}
	}
}

#endif