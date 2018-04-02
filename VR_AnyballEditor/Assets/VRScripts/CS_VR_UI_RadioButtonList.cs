using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_VR_UI_RadioButtonList : MonoBehaviour {

	[SerializeField] List<CS_VR_UI_RadioButton> myButtons;
	[SerializeField] int myDefaultIndex;

	[SerializeField] GameObject myTarget;
	[SerializeField] string myMethodName;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < myButtons.Count; i++) {
			myButtons [i].Init (this, i);
		}

		Press (myDefaultIndex);
	}

	public void Press (int g_index) {

		//do method
		myTarget.SendMessage ("SetSnappingPosition", g_index);

		//update button display
		for (int i = 0; i < myButtons.Count; i++) {
			if (i == g_index)
				myButtons [i].Show (true);
			else
				myButtons [i].Show (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
