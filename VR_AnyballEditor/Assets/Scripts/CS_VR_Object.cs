using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnyBall.Editor;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class CS_VR_Object : MonoBehaviour {
	
	[SerializeField] CS_AnyLevelObject myAnyLevelObjectScript;

	[SerializeField] Hand myHoldingHand;

	[SerializeField] GameObject myScaling_Object;
	[SerializeField] Vector3 myScaling_InitLocalPosition;
	[SerializeField] Vector3 myScaling_Default;

	void Awake () {
		myAnyLevelObjectScript = this.GetComponent<CS_AnyLevelObject> ();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (myHoldingHand != null && 
			myHoldingHand.otherHand != null && 
			myHoldingHand.otherHand.GetStandardInteractionButtonDown ()) {
			// create scaling

			myScaling_Object = new GameObject ();
			myScaling_Object.transform.SetParent (this.transform);
			myScaling_Object.transform.position = myHoldingHand.transform.position;

			myScaling_Default = this.transform.localScale;

			myScaling_InitLocalPosition = myScaling_Object.transform.localPosition;
		}

		if (myScaling_Object != null) {
			Vector3 t_deltaPos = myScaling_Object.transform.localPosition - myScaling_InitLocalPosition;
			Vector3 t_scale = new Vector3 (
				                  myScaling_Object.transform.localPosition.x / myScaling_InitLocalPosition.x,
				                  myScaling_Object.transform.localPosition.y / myScaling_InitLocalPosition.y,
				                  myScaling_Object.transform.localPosition.z / myScaling_InitLocalPosition.z
			                  );

			this.transform.localScale = new Vector3 (
				myScaling_Default.x * t_scale.x,
				myScaling_Default.y * t_scale.y,
				myScaling_Default.z * t_scale.z
			);
		}

		if (myHoldingHand == null || 
			myHoldingHand.otherHand == null ||
			myHoldingHand.otherHand.GetStandardInteractionButton () == false) {
			Destroy (myScaling_Object);
		}
	}

	//need name space "Valve.VR.InteractionSystem"
	//fire every frame as long as a hand is next to it
	//hand = mouse / VR controller
	void HandHoverUpdate (Hand g_hand) {
		//mouse click or trigger
		if (g_hand.GetStandardInteractionButtonDown ()) {
			Debug.Log ("player clicked on me!");

			if (g_hand.currentAttachedObject != null) {
				g_hand.DetachObject (g_hand.currentAttachedObject);
			}

			g_hand.AttachObject (this.gameObject);

			myHoldingHand = g_hand;

			if (myAnyLevelObjectScript != null)
				myAnyLevelObjectScript.enabled = false;
		}
	}

	void HandAttachedUpdate (Hand g_hand) {
		if (g_hand.GetStandardInteractionButton () == false) {
			Debug.Log ("player released on me!");
			g_hand.DetachObject (this.gameObject);

			myHoldingHand = null;

			if (myAnyLevelObjectScript != null)
				myAnyLevelObjectScript.enabled = true;
		}
	}
}
