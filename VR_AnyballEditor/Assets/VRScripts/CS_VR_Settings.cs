using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_VR_Settings : MonoBehaviour {

	private static CS_VR_Settings instance = null;
	public static CS_VR_Settings Instance { get { return instance; } }

	[SerializeField] Transform myHandleTransform;
	public float HandleScale { get { return myHandleTransform.localScale.x; } }
	public Transform HandleTransform { get { return myHandleTransform; } }

	private float mySnappingPosition;
	private float mySnappingScale;
	private float mySnappingRotation;

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
	}

	public float GetSnappingPosition () {
		return mySnappingPosition;
	}

	public void SetSnappingPosition (int g_index) {
		switch (g_index) {
		case 0:
			mySnappingPosition = 0f;
			break;
		case 1:
			mySnappingPosition = 0.25f;
			break;
		case 2:
			mySnappingPosition = 0.5f;
			break;
		case 3:
			mySnappingPosition = 1f;
			break;
		default:
			mySnappingPosition = 0f;
			break;
		}
	}

	public float GetSnappingRotation () {
		return mySnappingRotation;
	}

	public void SetSnappingRotation (int g_index) {

		Debug.Log ("SetSnappingRotation");

		switch (g_index) {
		case 0:
			mySnappingRotation = 0f;
			break;
		case 1:
			mySnappingRotation = 22.5f;
			break;
		case 2:
			mySnappingRotation = 45f;
			break;
		case 3:
			mySnappingRotation = 90f;
			break;
		default:
			mySnappingRotation = 0f;
			break;
		}
	}

	public float GetSnappingScale () {
		return mySnappingScale;
	}

	public void SetSnappingScale (int g_index) {
		switch (g_index) {
		case 0:
			mySnappingScale = 0f;
			break;
		case 1:
			mySnappingScale = 0.25f;
			break;
		case 2:
			mySnappingScale = 0.5f;
			break;
		case 3:
			mySnappingScale = 1f;
			break;
		default:
			mySnappingScale = 0f;
			break;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
