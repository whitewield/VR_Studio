using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hang.JellyJoystick;
using AnyBall.Property;

public class CS_MenuManager : MonoBehaviour {

	private static CS_MenuManager instance = null;
	public static CS_MenuManager Instance { get { return instance; } }

	[SerializeField] GameObject myTeamBucket;

	[SerializeField] CS_Prop_SpawnArea myPlayerSpawnArea;

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
	}

	// Use this for initialization
	void Start () {

		CS_PlayerManager.Instance.ShowPlayerNotInUse ();
		CS_PlayerManager.Instance.InitPlayersInUse ();

		//init player position
		List<GameObject> t_players = CS_PlayerManager.Instance.MyPlayersAll;
		for (int i = 0; i < t_players.Count; i++) {
			t_players [i].transform.position = myPlayerSpawnArea.GetRandomPoint ();
		}
	}
	
//	// Update is called once per frame
//	void Update () {
//		
//	}

	public void OnButtonStart () {
		CS_PlayerManager.Instance.CheckTeam ();
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Game");
	}
}
