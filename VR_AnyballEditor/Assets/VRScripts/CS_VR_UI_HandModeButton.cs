using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Valve.VR.InteractionSystem;

public class CS_VR_UI_HandModeButton : MonoBehaviour {

	[SerializeField] Renderer[] myRenderers;
	[SerializeField] Color myColor_Highlight;
	[SerializeField] Color myColor_Normal;

	[SerializeField] CS_VR_Settings.HandMode myHandMode;

	// Use this for initialization
	void Awake () {
		if (myRenderers == null || myRenderers.Length == 0) {
			myRenderers = this.GetComponents<Renderer> ();
		}

		foreach (Renderer f_renderer in myRenderers) {
			foreach (Material f_material in f_renderer.materials) {
				f_material.color = myColor_Normal;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnHandHoverBegin (Hand g_hand) {
		foreach (Renderer f_renderer in myRenderers) {
			foreach (Material f_material in f_renderer.materials) {
				f_material.color = myColor_Highlight;
			}
		}
	}

	void OnHandHoverEnd (Hand g_hand) {
		foreach (Renderer f_renderer in myRenderers) {
			foreach (Material f_material in f_renderer.materials) {
				f_material.color = myColor_Normal;
			}
		}
	}

	public void HandHoverUpdate (Hand g_hand) {
		//mouse click or trigger
		if (g_hand.GetStandardInteractionButtonDown ()) {
			CS_VR_Settings.Instance.OnButtonHandMode (g_hand, myHandMode);
		}
	}
}
