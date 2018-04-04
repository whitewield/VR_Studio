using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnyBall.Editor;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class CS_VR_Object : MonoBehaviour {

	[SerializeField] bool isDestroyable = true;
	[SerializeField] bool isFreeScale = true;
	[SerializeField] bool isUseSnapping = true;
	
	[SerializeField] CS_AnyLevelObject myAnyLevelObjectScript;

	[SerializeField] Hand myHoldingHand;
	[SerializeField] Hand myScalingHand;

	private bool onScale = false;
	private Vector3 myScaling_InitHandDistance;
	private Vector3 myScaling_Default;

	private Material myDefaultMaterial;

	private Vector3 myRealLocalPosition;
	private Quaternion myRealLocalRotation;

	void Awake () {
		myAnyLevelObjectScript = this.GetComponent<CS_AnyLevelObject> ();
		myDefaultMaterial = this.GetComponent<Renderer> ().material;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (myHoldingHand != null &&
		    myScalingHand != null &&
			(myScalingHand.currentAttachedObject == null ||
				myScalingHand.currentAttachedObject.GetComponent<CS_AnyLevelObject>() == null) &&
		    myScalingHand.GetStandardInteractionButtonDown ()) {
			// create scaling
			onScale = true;
			Debug.Log ("true" + myScalingHand.currentAttachedObject);

			myScaling_InitHandDistance = LocalVector3 (myScalingHand.transform.position - myHoldingHand.transform.position, this.transform);

			myScaling_Default = this.transform.localScale;
		}
		*/

		if (onScale) {
			if (myHoldingHand == null ||
				myScalingHand == null ||
				(myScalingHand.currentAttachedObject != null &&
					myScalingHand.currentAttachedObject.GetComponent<CS_AnyLevelObject>() != null) ||
				myScalingHand.GetStandardInteractionButton () == false) {
				onScale = false;
				Debug.Log ("false");
				myScalingHand.HoverUnlock (null);
				return;
			}

			Vector3 t_handDistance = LocalVector3 (myScalingHand.transform.position - myHoldingHand.transform.position, this.transform);

//			Vector3 t_handDistance = 
//				this.transform.TransformPoint (myScalingHand.transform.position - myHoldingHand.transform.position);
			Vector3 t_scale = Vector3.one;
			if (isFreeScale) {
				Vector3 t_deltaPos = t_handDistance - myScaling_InitHandDistance;
				t_scale = new Vector3 (
					Mathf.Abs (myScaling_Default.x + t_deltaPos.x),
					Mathf.Abs (myScaling_Default.y + t_deltaPos.y),
					Mathf.Abs (myScaling_Default.z + t_deltaPos.z)
				);
			} else {
				t_scale = myScaling_Default + Vector3.one * (t_handDistance.magnitude - myScaling_InitHandDistance.magnitude);
				t_scale = new Vector3 (
					Mathf.Abs (t_scale.x),
					Mathf.Abs (t_scale.y),
					Mathf.Abs (t_scale.z)
				);
			}
				
			//snapping scale

			float t_snapping = CS_VR_Settings.Instance.GetSnappingScale ();

			if (isUseSnapping && t_snapping != 0) {
				t_scale = new Vector3 (
					t_scale.x - t_scale.x % t_snapping,
					t_scale.y - t_scale.y % t_snapping,
					t_scale.z - t_scale.z % t_snapping
				);
			}

			this.transform.localScale = t_scale;


		}

	}

	void OnHandHoverBegin (Hand g_hand) {
		if (onScale)
			return;
		this.GetComponent<Renderer> ().material = CS_VR_LevelManager.Instance.EmissionMaterial;
	}

	void OnHandHoverEnd (Hand g_hand) {
		this.GetComponent<Renderer> ().material = myDefaultMaterial;
	}

	//need name space "Valve.VR.InteractionSystem"
	//fire every frame as long as a hand is next to it
	//hand = mouse / VR controller
	public void HandHoverUpdate (Hand g_hand) {
		//mouse click or trigger
		if (g_hand.GetStandardInteractionButtonDown ()) {
			if (myHoldingHand == null) {

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

				if (myAnyLevelObjectScript != null)
					myAnyLevelObjectScript.enabled = false;

				myHoldingHand.HoverLock (null);

				myRealLocalPosition = this.transform.localPosition;

				myRealLocalRotation = this.transform.localRotation;

			} else {
				//scale

				if (myHoldingHand.otherHand != null &&
					(myHoldingHand.otherHand.currentAttachedObject == null ||
						myHoldingHand.otherHand.currentAttachedObject.GetComponent<CS_AnyLevelObject>() == null) &&
					myHoldingHand.otherHand.GetStandardInteractionButtonDown ()) {

					myScalingHand = myHoldingHand.otherHand;

					this.GetComponent<Renderer> ().material = myDefaultMaterial;

					// create scaling
					onScale = true;
					Debug.Log ("true" + myScalingHand.currentAttachedObject);

//					myScaling_InitHandDistance = 
//						this.transform.TransformPoint (myScalingHand.transform.position - myHoldingHand.transform.position);

					myScaling_InitHandDistance = LocalVector3 (myScalingHand.transform.position - myHoldingHand.transform.position, this.transform);

					myScaling_Default = this.transform.localScale;

					myScalingHand.HoverLock (null);
				}
			}
		}
	}

	void HandAttachedUpdate (Hand g_hand) {

		UpdateSnappingPosition ();

		UpdateSnappingRotation ();
		
		if (g_hand.GetStandardInteractionButton () == false) {
			Debug.Log ("player released on me!");
			g_hand.DetachObject (this.gameObject);

			myHoldingHand.HoverUnlock (null);
			myHoldingHand = null;

			if (myAnyLevelObjectScript != null)
				myAnyLevelObjectScript.enabled = true;

		}
	}

	private void UpdateSnappingRotation () {
		if (isUseSnapping == false)
			return;

		float t_snapping = CS_VR_Settings.Instance.GetSnappingRotation ();

		if (t_snapping == 0)
			return;

		Quaternion t_realWorldRotation = myHoldingHand.transform.rotation * myRealLocalRotation;

		Quaternion t_toHandleRotation = Quaternion.Inverse (CS_VR_Settings.Instance.HandleTransform.rotation) * t_realWorldRotation;

		Vector3 t_toHandleEuler = t_toHandleRotation.eulerAngles;

		t_toHandleEuler = new Vector3 (
			t_toHandleEuler.x - t_toHandleEuler.x % t_snapping,
			t_toHandleEuler.y - t_toHandleEuler.y % t_snapping,
			t_toHandleEuler.z - t_toHandleEuler.z % t_snapping
		);

		t_toHandleRotation = Quaternion.Euler (t_toHandleEuler);

		this.transform.rotation = CS_VR_Settings.Instance.HandleTransform.rotation * t_toHandleRotation;
	}

	private void UpdateSnappingPosition () {
		if (isUseSnapping == false)
			return;

		float t_snapping = CS_VR_Settings.Instance.GetSnappingPosition ();

		if (t_snapping == 0)
			return;

		Vector3 t_realWorldPosition = myHoldingHand.transform.TransformPoint (myRealLocalPosition);

		Vector3 t_toHandlePosition = CS_VR_Settings.Instance.HandleTransform.InverseTransformPoint (t_realWorldPosition);

		t_toHandlePosition = new Vector3 (
			t_toHandlePosition.x - t_toHandlePosition.x % t_snapping,
			t_toHandlePosition.y - t_toHandlePosition.y % t_snapping,
			t_toHandlePosition.z - t_toHandlePosition.z % t_snapping
		);
		
		this.transform.position = CS_VR_Settings.Instance.HandleTransform.TransformPoint (t_toHandlePosition);
	}

	public void Delete () {
		if (!isDestroyable)
			return;
		if (myHoldingHand != null) {
			myHoldingHand.DetachObject (myHoldingHand.currentAttachedObject);
			myHoldingHand.HoverUnlock (null);
		}
		if (myScalingHand != null) {
			myScalingHand.HoverUnlock (null);
		}
		Destroy (this.gameObject);
	}

	private Vector3 LocalVector3 (Vector3 g_original, Transform g_localTransform) {
		return new Vector3 (
			Mathf.Abs (Vector3.Dot (g_original, g_localTransform.right)),
			Mathf.Abs (Vector3.Dot (g_original, g_localTransform.up)),
			Mathf.Abs (Vector3.Dot (g_original, g_localTransform.forward))
		);

	}
}
