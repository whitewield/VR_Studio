using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_FollowZoomCamera : MonoBehaviour {

	private static CS_FollowZoomCamera instance = null;
	public static CS_FollowZoomCamera Instance { get { return instance; } }

	[SerializeField] Camera myCamera;
	[SerializeField] List<Transform> myFollowTransforms = new List<Transform>();
	[SerializeField] float myLerpSpeed = 10;
	[SerializeField] float myZoomLerpSpeed = 10;
	[SerializeField] float myPositionMultiplier = 3;
	[SerializeField] float myPositionOffset = 3;
	[SerializeField] float myPlayerVision = 10;

	private bool isInGame = false;
	[SerializeField] bool followScoreboard;

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}

		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (followScoreboard)
			Update_FollowScoreboard ();
		else
			Update_NotFollowScoreboard ();
	}

	private void Update_NotFollowScoreboard () {
		if (CS_PlayerManager.Instance == null)
			return;

		List<GameObject> targetList = CS_PlayerManager.Instance.MyPlayersAll;

		if (CS_GameManager.Instance != null)
			targetList = CS_PlayerManager.Instance.MyPlayersInUse;


		if (myFollowTransforms.Count != targetList.Count) {
			myFollowTransforms.Clear ();
			for (int i = 0; i < targetList.Count; i++) {
				myFollowTransforms.Add (targetList [i].transform);
			}
		} else {
			this.transform.position = Vector3.Lerp (this.transform.position, GetCenter (), Time.deltaTime * myLerpSpeed);
			UpdateDistance ();
		}
	}

	private void Update_FollowScoreboard () {
		if (CS_PlayerManager.Instance == null)
			return;

		List<GameObject> targetList = CS_PlayerManager.Instance.MyPlayersAll;

		if (CS_GameManager.Instance != null)
			targetList = CS_PlayerManager.Instance.MyPlayersInUse;

		bool t_doUpdate = false;

		if (!isInGame && myFollowTransforms.Count != targetList.Count) {
			t_doUpdate = true;
		}

		if (isInGame && myFollowTransforms.Count != (targetList.Count + 1)) {
			t_doUpdate = true;
		}

		if ((CS_GameManager.Instance != null) != isInGame) {
			isInGame = (CS_GameManager.Instance != null);
			t_doUpdate = true;

			Debug.Log ("isInGame");
		}

		if (t_doUpdate) {
//			Debug.Log ("t_doUpdate");

			myFollowTransforms.Clear ();
			for (int i = 0; i < targetList.Count; i++) {
				myFollowTransforms.Add (targetList [i].transform);
			}
			if (isInGame && CS_SingleScoreBoard.Instance != null)
				myFollowTransforms.Add (CS_SingleScoreBoard.Instance.transform);
		} else {
//			Debug.Log ("not t_doUpdate");

			this.transform.position = Vector3.Lerp (this.transform.position, GetCenter (), Time.deltaTime * myLerpSpeed);
			UpdateDistance ();
		}
	}

	private Vector3 GetCenter () {
		Vector3 t_center = Vector3.zero;

		for (int i = 0; i < myFollowTransforms.Count; i++) {
			t_center += myFollowTransforms [i].position + myFollowTransforms [i].forward * myPlayerVision;
//			t_center += myFollowTransforms [i].position + myFollowTransforms [i].forward * myPlayerVision;
		}

		return t_center / myFollowTransforms.Count;
	}

	private void UpdateDistance () {
		float t_radius = 0;
		Vector3 t_center = GetCenter ();

		for (int i = 0; i < myFollowTransforms.Count; i++) {
			t_radius = Mathf.Max (t_radius, Vector3.Distance (t_center, myFollowTransforms [i].position));
		}

//		Debug.Log (t_radius);

		myCamera.transform.localPosition = 
			Vector3.Lerp (myCamera.transform.localPosition, 
				new Vector3 (0, (t_radius + myPositionOffset) * myPositionMultiplier, -(t_radius + myPositionOffset) * myPositionMultiplier), 
			Time.deltaTime * myZoomLerpSpeed
		);

		if (myCamera.orthographic) {
			myCamera.orthographicSize = Mathf.Lerp (myCamera.orthographicSize, 
				(t_radius + myPositionOffset) * myPositionMultiplier / 4, 
				Time.deltaTime * myZoomLerpSpeed
			);
		}
	}
}
