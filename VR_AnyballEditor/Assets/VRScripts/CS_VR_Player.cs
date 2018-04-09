using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnyBall.Editor;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class CS_VR_Player : MonoBehaviour {

	private Hand myHoldingHand;

	private Quaternion myLocalRotation;

	[SerializeField] Renderer myTargetRenderer;
	private Material myDefaultMaterial;

	void Awake () {
		myDefaultMaterial = myTargetRenderer.material;

		myLocalRotation = this.transform.localRotation;
	}

	// Use this for initialization
	void Start () {
		
	}

	void Update () {
		this.transform.rotation = CS_VR_Settings.Instance.HandleTransform.rotation * myLocalRotation;
	}

	void OnHandHoverBegin (Hand g_hand) {
		myTargetRenderer.material = CS_VR_LevelManager.Instance.EmissionMaterial;
	}

	void OnHandHoverEnd (Hand g_hand) {
		myTargetRenderer.material = myDefaultMaterial;
	}

	//need name space "Valve.VR.InteractionSystem"
	//fire every frame as long as a hand is next to it
	//hand = mouse / VR controller
	public void HandHoverUpdate (Hand g_hand) {
		//mouse click or trigger
		if (g_hand.GetStandardInteractionButtonDown ()) {
			//grab

			Debug.Log ("player clicked on me!");

			if (g_hand.currentAttachedObject != null) {
				g_hand.DetachObject (g_hand.currentAttachedObject);
			}

			Quaternion t_lastRotation = this.transform.rotation;
			Vector3 t_lastPosition = this.transform.position;

			g_hand.AttachObject (this.gameObject);

			this.transform.rotation = t_lastRotation;
			this.transform.position = t_lastPosition;

			myHoldingHand = g_hand;

			myHoldingHand.HoverLock (null);

		}
	}

	void HandAttachedUpdate (Hand g_hand) {

//		UpdateFixedRotation ();
		
		if (g_hand.GetStandardInteractionButton () == false) {
			Debug.Log ("player released on me!");
			g_hand.DetachObject (this.gameObject);

			myHoldingHand.HoverUnlock (null);
			myHoldingHand = null;

		}
	}

	private void UpdateFixedRotation () {
		this.transform.localRotation = myLocalRotation;
	}

}
