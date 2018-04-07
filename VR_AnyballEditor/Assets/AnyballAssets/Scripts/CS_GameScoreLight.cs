using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_GameScoreLight : MonoBehaviour {

	[SerializeField] GameObject myLight;

	public void SetLightOn () {
		myLight.SetActive (true);
	}

	public void SetLightOff () {
		myLight.SetActive (false);
	}
}
