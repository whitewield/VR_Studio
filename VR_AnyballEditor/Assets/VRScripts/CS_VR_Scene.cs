using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class CS_VR_Scene : MonoBehaviour {

	private Hand[] myAllHands;

	// scene data
	private float mySceneScale;

	private GameObject mySceneParent;

	private Hand myOneHand;
	private Hand myOtherHand;
	private float myHandDistance;


	void Start () {
		myAllHands = FindObjectOfType<Player> ().GetComponentsInChildren<Hand> (true);
		mySceneParent = new GameObject ();
	}

	void Update () {
		foreach (Hand f_hand in myAllHands) {
			if (f_hand.GetStandardInteractionButton () == false && f_hand.hoverLocked) {
				f_hand.HoverUnlock (null);
			}
		}

		if (CS_VR_Settings.Instance.GetSelectionMode () == CS_VR_Settings.SelectionMode.Object) {
			myOneHand = null;
			myOtherHand = null;
			mySceneParent.transform.DetachChildren ();
			return;
		}

		Update_Hands ();
	}

	void Init_OneHand (Hand g_hand) {
		mySceneParent.transform.DetachChildren ();

		myOneHand = g_hand;

		mySceneParent.transform.position = myOneHand.transform.position;
		mySceneParent.transform.rotation = myOneHand.transform.rotation;

		this.transform.SetParent (mySceneParent.transform);
	}

	void Init_TwoHands (Hand g_hand) {
		mySceneParent.transform.DetachChildren ();

		myOtherHand = g_hand;

		myHandDistance = Vector3.Distance (myOtherHand.transform.position, myOneHand.transform.position);

		mySceneParent.transform.position = (myOtherHand.transform.position + myOneHand.transform.position) / 2;
		mySceneParent.transform.rotation = Quaternion.LookRotation (
			myOtherHand.transform.position - myOneHand.transform.position);

		this.transform.SetParent (mySceneParent.transform);

		mySceneScale = this.transform.localScale.x;
	}

	void Update_Hands () {
		// set one hand
		if (myOneHand == null) {
			foreach (Hand f_hand in myAllHands) {
				if (f_hand.GetStandardInteractionButtonDown ()) {
					Init_OneHand (f_hand);
					break;
				}
			}
		} else {
			if (myOneHand.GetStandardInteractionButton () == false) {
				myOneHand = null;
				mySceneParent.transform.DetachChildren ();

				if (myOtherHand != null) {
					Init_OneHand (myOtherHand);
					myOtherHand = null;
				}
			}
		}

		if (myOneHand == null)
			return;

		// set other hand
		if (myOtherHand == null) {
			if (myOneHand.otherHand != null && myOneHand.otherHand.GetStandardInteractionButtonDown ()) {
				Init_TwoHands (myOneHand.otherHand);
			}
		} else {
			if (myOtherHand.GetStandardInteractionButton () == false) {
				myOtherHand = null;
				Init_OneHand (myOneHand);
			}
		}

		if (myOtherHand == null) {
			Update_OneHandControl ();
		} else {
			Update_TwoHandsControl ();
		}
	}

	void Update_OneHandControl () {
		mySceneParent.transform.position = myOneHand.transform.position;
		mySceneParent.transform.rotation = myOneHand.transform.rotation;
	}

	void Update_TwoHandsControl () {

		mySceneParent.transform.position = (myOtherHand.transform.position + myOneHand.transform.position) / 2;
		mySceneParent.transform.rotation = Quaternion.LookRotation (
			myOtherHand.transform.position - myOneHand.transform.position);

		float t_newHandDistance = Vector3.Distance (myOtherHand.transform.position, myOneHand.transform.position);

		float t_scale = t_newHandDistance / myHandDistance;
		this.transform.localScale = t_scale * mySceneScale * Vector3.one;

	}

}
