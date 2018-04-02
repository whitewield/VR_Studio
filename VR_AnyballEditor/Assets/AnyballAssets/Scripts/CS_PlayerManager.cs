using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using Hang.JellyJoystick;

public class CS_PlayerManager : MonoBehaviour {

	private static CS_PlayerManager instance = null;
	public static CS_PlayerManager Instance { get { return instance; } }

	[SerializeField] int myPlayerCount = 2;
	[SerializeField] GameObject myPlayerPrefab;
	private List<GameObject> myPlayersAll = new List<GameObject> ();
	public List<GameObject> MyPlayersAll { get { return myPlayersAll; } }
	private List<GameObject> myPlayersInUse = new List<GameObject> ();
	public List<GameObject> MyPlayersInUse { get { return myPlayersInUse; } }
	[SerializeField] float myPlayerDistanceToCenter = 5;

	private int myTeamCount;
	private List<int> myIndexToTeamNumber = new List<int> ();
	[SerializeField] Color myInactiveColor = Color.gray;
	[SerializeField] Color[] myTeamColors;
	[SerializeField] Color[] myTeamColors_Dark;

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}

		DontDestroyOnLoad (this.gameObject);

		for (int i = 0; i < myPlayerCount; i++) {
			myPlayersAll.Add (Instantiate (myPlayerPrefab, Vector3.zero, Quaternion.identity) as GameObject);
			myPlayersAll [i].GetComponent<CS_PlayerControl> ().Init (i);
		}

	}

	// Use this for initialization
	void Start () {
		
		CheckTeam ();
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}

	public Color GetTeamColor (int g_teamNumber, bool g_isDark = false) {
		switch (g_isDark) {
		case false:
			if (myTeamColors.Length > g_teamNumber)
				return myTeamColors [g_teamNumber];
			break;
		case true:
			if (myTeamColors_Dark.Length > g_teamNumber)
				return myTeamColors_Dark [g_teamNumber];
			break;
		}

		Debug.LogError ("cannot find the color");
		return Color.black;
	}

	public Color GetTeamColorFromIndex (int g_index, bool g_isDark = false) {
		return GetTeamColor (GetTeamNumber (g_index), g_isDark);
	}

	public Color GetInactiveColor () {
		return myInactiveColor;
	}

	public void CheckTeam () {
		List<int> t_teamNumbers = new List<int> ();
		for (int i = 0; i < myPlayersInUse.Count; i++) {
			int f_teamNumber = myPlayersInUse [i].GetComponent<CS_PlayerControl> ().GetTeam ();
			if (t_teamNumbers.Contains (f_teamNumber) == false) {
				t_teamNumbers.Add (f_teamNumber);
			}
		}

		myTeamCount = t_teamNumbers.Count;

		myIndexToTeamNumber.Clear ();
		for (int i = 0; i < Constants.NUMBER_MAX_TEAM; i++) {
			if (t_teamNumbers.Contains (i))
				myIndexToTeamNumber.Add (i);
		}
	}

	public bool IsTeamExist (int g_teamNumber) {
		return myIndexToTeamNumber.Contains (g_teamNumber);
	}

	public int GetTeamCount () {
		return myTeamCount;
	}

	public int GetTeamNumber (int g_index) {
		return myIndexToTeamNumber [g_index];
	}

	public int GetIndexNumber (int g_teamNumber) {
		return myIndexToTeamNumber.IndexOf (g_teamNumber);
	}

	public int GetPlayerCount () {
		return myPlayerCount;
	}


	public void InitPlayersInUse () {
		myPlayersInUse.Clear ();
	}

	public void AddPlayerInUse (GameObject g_player) {
		if (myPlayersInUse.Contains (g_player) == false)
			myPlayersInUse.Add (g_player);
	}

	public void RemovePlayerInUse (GameObject g_player) {
		if (myPlayersInUse.Contains (g_player) == true)
			myPlayersInUse.Remove (g_player);
	}

	public void HidePlayerNotInUse () {
		foreach (GameObject f_player in myPlayersAll) {
			if (myPlayersInUse.Contains (f_player) == false) {
				f_player.SetActive (false);
			}
		}
	}

	public void ShowPlayerNotInUse () {
		foreach (GameObject f_player in myPlayersAll) {
			if (myPlayersInUse.Contains (f_player) == false) {
				f_player.SetActive (true);
			}
		}
	}
}
