using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class CS_VR_UI_RadioButton : MonoBehaviour {

	[SerializeField] Renderer[] myRenderers;
	[SerializeField] Color myColor_Highlight;
	[SerializeField] Color myColor_Normal;

	private CS_VR_UI_RadioButtonList myList;
	private int myIndex;


	void Awake () {
		if (myRenderers == null || myRenderers.Length == 0) {
			myRenderers = new Renderer[1];
			myRenderers [0] = this.GetComponent<Renderer> ();
		}
			
	}

	public void Init (CS_VR_UI_RadioButtonList g_list, int g_index) {
		myList = g_list;
		myIndex = g_index;
	}

	public void Show (bool g_isHighlighted) {
		if (g_isHighlighted) {
			foreach (Renderer f_renderer in myRenderers) {
				f_renderer.material.color = myColor_Highlight;
			}
		} else {
			foreach (Renderer f_renderer in myRenderers) {
				f_renderer.material.color = myColor_Normal;
			}
		}
	}

	public void HandHoverUpdate (Hand g_hand) {
		//mouse click or trigger
		if (g_hand.GetStandardInteractionButtonDown ()) {
			myList.Press (myIndex);
		}
	}
}
