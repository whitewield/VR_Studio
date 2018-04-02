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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
