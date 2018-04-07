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

	[SerializeField] Renderer myRenderer;
	private Material myDefaultMaterial;

	private GameObject myReference;

	void Awake () {
		myAnyLevelObjectScript = this.GetComponent<CS_AnyLevelObject> ();
		if (myRenderer == null)
			myRenderer = this.GetComponent<Renderer> (); 
		myDefaultMaterial = myRenderer.material;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (onScale) {
			if (myHoldingHand == null ||
				myScalingHand == null ||
				(myScalingHand.currentAttachedObject != null &&
					myScalingHand.currentAttachedObject.GetComponent<CS_AnyLevelObject>() != null) ||
				myScalingHand.GetStandardInteractionButton () == false) {
				onScale = false;
				Debug.Log ("false");
				myScalingHand.HoverUnlock (null);

				//SnapScale (this.transform);

				return;
			}

			Vector3 t_handDistance = LocalVector3 (myScalingHand.transform.position - myHoldingHand.transform.position, this.transform);

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

			this.transform.localScale = t_scale;
		}
	}

	void UpdateReference () {
		if (myReference == null || isUseSnapping == false)
			return;

		myReference.transform.position = this.transform.position;
		myReference.transform.rotation = this.transform.rotation;
		myReference.transform.localScale = this.transform.localScale / CS_VR_Settings.Instance.HandleScale;

		SnapPosition (myReference.transform);
		SnapRotation (myReference.transform);
		SnapScale (myReference.transform);
	}

	void OnHandHoverBegin (Hand g_hand) {
		if (onScale)
			return;
		myRenderer.material = CS_VR_LevelManager.Instance.EmissionMaterial;
	}

	void OnHandHoverEnd (Hand g_hand) {
		myRenderer.material = myDefaultMaterial;
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

				if (isUseSnapping)
					myReference = CS_VR_ReferenceManager.Instance.GetIdleReference ();

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

			} else {
				//scale

				if (myHoldingHand.otherHand != null &&
					(myHoldingHand.otherHand.currentAttachedObject == null ||
						myHoldingHand.otherHand.currentAttachedObject.GetComponent<CS_AnyLevelObject>() == null) &&
					myHoldingHand.otherHand.GetStandardInteractionButtonDown ()) {

					myScalingHand = myHoldingHand.otherHand;

					myRenderer.material = myDefaultMaterial;

					// create scaling
					onScale = true;
					Debug.Log ("true" + myScalingHand.currentAttachedObject);

					myScaling_InitHandDistance = LocalVector3 (myScalingHand.transform.position - myHoldingHand.transform.position, this.transform);

					myScaling_Default = this.transform.localScale;

					myScalingHand.HoverLock (null);
				}
			}
		}
	}

	void HandAttachedUpdate (Hand g_hand) {

		//UpdateSnappingPosition ();

		//UpdateSnappingRotation ();
		UpdateReference ();

		if (g_hand.GetStandardInteractionButton () == false) {
			Debug.Log ("player released on me!");
			g_hand.DetachObject (this.gameObject);

			myHoldingHand.HoverUnlock (null);
			myHoldingHand = null;

			if (myAnyLevelObjectScript != null)
				myAnyLevelObjectScript.enabled = true;

			if (isUseSnapping) {
				myReference.SetActive (false);
				myReference = null;
			}

			SnapPosition (this.transform);
			SnapRotation (this.transform);
			SnapScale (this.transform);
		}
	}

	public void Delete () {
		if (!isDestroyable)
			return;

		if (myReference != null)
			myReference.SetActive (false);

		if (myHoldingHand != null) {
			myHoldingHand.DetachObject (myHoldingHand.currentAttachedObject);
			myHoldingHand.HoverUnlock (null);
		}
		if (myScalingHand != null) {
			myScalingHand.HoverUnlock (null);
		}
		Destroy (this.gameObject);
	}

	private void SnapPosition (Transform g_transform) {
		if (isUseSnapping == false)
			return;

		float t_snapping = CS_VR_Settings.Instance.GetSnappingPosition ();

		if (t_snapping == 0)
			return;

		Vector3 t_position = g_transform.localPosition;

		Vector3 t_positionTimes = new Vector3 (
			Mathf.Round (t_position.x / t_snapping), 
			Mathf.Round (t_position.y / t_snapping), 
			Mathf.Round (t_position.z / t_snapping)
		);

		t_position = t_positionTimes * t_snapping;

		g_transform.localPosition = t_position;
		//g_transform.localPosition = Vector3.Lerp(g_transform.localPosition, t_position, Time.deltaTime * CS_VR_Settings.Instance.SnappingSpeed);
	}

	private void SnapRotation (Transform g_transform) {
		if (isUseSnapping == false)
			return;

		float t_snapping = CS_VR_Settings.Instance.GetSnappingRotation ();

		if (t_snapping == 0)
			return;

		Vector3 t_euler = g_transform.localRotation.eulerAngles;

		Vector3 t_eulerTimes = new Vector3 (
			Mathf.Round (t_euler.x / t_snapping), 
			Mathf.Round (t_euler.y / t_snapping), 
			Mathf.Round (t_euler.z / t_snapping)
		);

		t_euler = t_eulerTimes * t_snapping;

		g_transform.localRotation = Quaternion.Euler (t_euler);
		//g_transform.localRotation = Quaternion.Lerp(g_transform.localRotation, Quaternion.Euler (t_euler), Time.deltaTime * CS_VR_Settings.Instance.SnappingSpeed);
	}

	private void SnapScale (Transform g_transform) {
		if (isUseSnapping == false)
			return;

		float t_snapping = CS_VR_Settings.Instance.GetSnappingScale ();

		if (t_snapping == 0)
			return;
		
		Vector3 t_scale = g_transform.localScale;

		Vector3 t_scaleTimes = new Vector3 (
			                       Mathf.Round (t_scale.x / t_snapping), 
			                       Mathf.Round (t_scale.y / t_snapping), 
			                       Mathf.Round (t_scale.z / t_snapping)
		                       );

		t_scaleTimes.x = Mathf.Clamp (t_scaleTimes.x, 1, t_scaleTimes.x);
		t_scaleTimes.y = Mathf.Clamp (t_scaleTimes.y, 1, t_scaleTimes.y);
		t_scaleTimes.z = Mathf.Clamp (t_scaleTimes.z, 1, t_scaleTimes.z);

		t_scale = t_scaleTimes * t_snapping;

		g_transform.localScale = t_scale;
		//g_transform.localScale = Vector3.Lerp(g_transform.localScale, t_scale, Time.deltaTime * CS_VR_Settings.Instance.SnappingSpeed);
	}

	private Vector3 LocalVector3 (Vector3 g_original, Transform g_localTransform) {
		return new Vector3 (
			Mathf.Abs (Vector3.Dot (g_original, g_localTransform.right)),
			Mathf.Abs (Vector3.Dot (g_original, g_localTransform.up)),
			Mathf.Abs (Vector3.Dot (g_original, g_localTransform.forward))
		);

	}
}
