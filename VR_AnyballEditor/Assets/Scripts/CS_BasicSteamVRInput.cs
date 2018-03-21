using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PUT THIS SCRIPT ON A CONTROLLER
//use Prefabs/[CameraRig]

public class CS_BasicSteamVRInput : MonoBehaviour {

	SteamVR_TrackedObject myTrackedObject;

	SteamVR_Controller.Device myDevice {
		get {
			return SteamVR_Controller.Input ((int)myTrackedObject.index);
		}
	}

	// Use this for initialization
	void Start () {
		myTrackedObject = GetComponent<SteamVR_TrackedObject> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (myDevice.GetHairTrigger ()) {
			Debug.Log ("user is holding down the trigger");
		}

		if (myDevice.GetPress (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)) {
			
		}

		if (myDevice.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0).x > 0) {

		}
	}
}
