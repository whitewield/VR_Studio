using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class CS_VR_Scene : MonoBehaviour {

	private Hand[] myAllHands;

	private Hand myGrabbingHand;
	private Vector3 myGrabbingPosition;

	private Hand myScalingHand;



	void Start () {
		myAllHands = FindObjectOfType<Player> ().GetComponentsInChildren<Hand> (true);
	}

	void Update () {
		if (CS_VR_Settings.Instance.GetSelectionMode () == CS_VR_Settings.SelectionMode.Object)
			return;
		
		Debug.Log (myGrabbingHand);
		Update_StartGrab ();
		Update_Grabbing ();
	}

	void Update_StartGrab () {
		if (myGrabbingHand != null)
			return;

		foreach (Hand f_hand in myAllHands) {
			if (f_hand.GetStandardInteractionButtonDown ()) {
				myGrabbingHand = f_hand;
				myGrabbingPosition = myGrabbingHand.transform.position;
				return;
			}
		}
	}

	void Update_Grabbing () {
		if (myGrabbingHand == null)
			return;

		if (myGrabbingHand.GetStandardInteractionButton () == false)
			myGrabbingHand = null;

		if (myGrabbingHand == null)
			return;

		Vector3 t_newPosition = myGrabbingHand.transform.position;
		Vector3 t_moveVector = t_newPosition - myGrabbingPosition;
		myGrabbingPosition = t_newPosition;

		CS_VR_Settings.Instance.HandleTransform.position += t_moveVector;
	}
}
