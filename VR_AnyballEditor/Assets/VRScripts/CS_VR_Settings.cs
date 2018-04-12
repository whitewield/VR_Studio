﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_VR_Settings : MonoBehaviour {

	private static CS_VR_Settings instance = null;
	public static CS_VR_Settings Instance { get { return instance; } }

	[SerializeField] Transform myHandleTransform;
	public float HandleScale { get { return myHandleTransform.localScale.x; } }
	public Transform HandleTransform { get { return myHandleTransform; } }

	public enum SelectionMode {
		Scene,
		Object,
	}

	private SelectionMode mySelectionMode;
	private float mySnappingPosition;
	private float mySnappingScale;
	private float mySnappingRotation;

	[SerializeField] float mySnappingSpeed = 10;
	public float SnappingSpeed { get { return mySnappingSpeed; } }

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
	}

	public SelectionMode GetSelectionMode () {
		return mySelectionMode;
	}

	public void SetSelectionMode (int g_index) {
		mySelectionMode = (SelectionMode)g_index;
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

//		Debug.Log ("SetSnappingRotation");

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
			mySnappingScale = 0.5f;
			break;
		case 2:
			mySnappingScale = 1f;
			break;
		case 3:
			mySnappingScale = 2f;
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
		if (Input.GetKeyDown (KeyCode.R)) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Game");
		}
	}
}
