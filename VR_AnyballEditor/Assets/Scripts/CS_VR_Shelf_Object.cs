using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnyBall.Editor;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class CS_VR_Shelf_Object : MonoBehaviour {

	private GameObject myPrefab;
	public GameObject Prefab { set { myPrefab = value; } get { return myPrefab; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void HandHoverUpdate (Hand g_hand) {
		//mouse click or trigger
		if (g_hand.GetStandardInteractionButtonDown ()) {
			Debug.Log ("player clicked on me!");

			GameObject t_gameObject = 
				Instantiate (
					myPrefab, 
					CS_VR_LevelManager.Instance.GetParent (myPrefab.GetComponent<CS_AnyLevelObject> ().GetMyCategory ())
				);

			t_gameObject.GetComponent<CS_VR_Shelf_Object> ().enabled = false;
			Destroy (t_gameObject.GetComponent<CS_VR_Shelf_Object> ());

			t_gameObject.GetComponent<CS_AnyLevelObject> ().enabled = false;

			if (g_hand.currentAttachedObject != null) {
				g_hand.DetachObject (g_hand.currentAttachedObject);
			}

			g_hand.AttachObject (t_gameObject);
		}
	}

}
