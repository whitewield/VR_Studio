using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Property;

public class CS_GameScoreBoard : MonoBehaviour {

	private static CS_GameScoreBoard instance = null;

	public static CS_GameScoreBoard Instance {
		get { 
			return instance;
		}
	}

	[SerializeField] Transform myBoardTransform;
	[SerializeField] GameObject myLightPrefab;
	private List<GameObject> myLightList = new List<GameObject> ();
	[SerializeField] Vector2 myLightCorner;

	[SerializeField] Transform myBoardHangerTransform;
	[SerializeField] float myBoardHangerOutY = 20;


//	[SerializeField] int myTeamCount;
	private int myWinnerScore;

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}

		//		DontDestroyOnLoad (this.gameObject);
	}

//	// Use this for initialization
//	void Start () {
//		
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		
//	}

	public void Show () {
		Debug.Log ("show scoreboard");
		myBoardHangerTransform.localPosition = Vector3.zero;
	}

	public void Hide () {
		Debug.Log ("hide scoreboard");
		myBoardHangerTransform.localPosition = new Vector3 (0, myBoardHangerOutY, 0);
	}

	public void UpdateScore (List<int> g_score) {
		for (int teamIndex = 0; teamIndex < g_score.Count; teamIndex++) {
			for (int j = 0; j < myWinnerScore; j++) {
				CS_GameScoreLight f_light = GetLight (teamIndex, j).GetComponent<CS_GameScoreLight> ();

				if (j < g_score [teamIndex]) {
					f_light.SetLightOn ();
				} else {
					f_light.SetLightOff ();
				}
			}
		}
	}

	public void InitScoreBoard (int g_teamCount, int g_winnerScore) {
		myWinnerScore = g_winnerScore;

		int t_totalLights = g_teamCount * g_winnerScore;

		//create new lights
		int t_newAmount = t_totalLights - myLightList.Count;
		if (t_newAmount > 0) {
			for (int i = 0; i < t_newAmount; i++) {
				myLightList.Add (Instantiate (myLightPrefab, myBoardTransform));
			}
		}

		//set active for all lights
		for (int i = 0; i < myLightList.Count; i++) {
			if (i < t_totalLights)
				myLightList [i].SetActive (true);
			else
				myLightList [i].SetActive (false);
		}

		//calculate the start pos and interval for lights
		Vector2 t_startPos = new Vector2 (0, 0);
		Vector2 t_interval = new Vector2 (0, 0);

		if (g_teamCount > 1){
			t_interval.y = Mathf.Abs (myLightCorner.y) * 2 / (g_teamCount - 1);
			t_startPos.y = Mathf.Abs (myLightCorner.y);
		}
		
		if (g_winnerScore > 1) {
			t_interval.x = Mathf.Abs (myLightCorner.x) * 2 / (g_winnerScore - 1);
			t_startPos.x = -Mathf.Abs (myLightCorner.x);
		}

		//move lights and color lights
		for (int y = 0; y < g_teamCount; y++) {
			
			Color f_teamColor = CS_PlayerManager.Instance.GetTeamColorFromIndex (y);

			for (int x = 0; x < g_winnerScore; x++) {
				GameObject f_light = GetLight (y, x);

				f_light.transform.localPosition = 
					new Vector3 (t_startPos.x + x * t_interval.x, t_startPos.y - y * t_interval.y, 0);

				f_light.GetComponent<CS_Prop_Color> ().SetColor (f_teamColor);

				f_light.GetComponent<CS_GameScoreLight> ().SetLightOff ();
			}
		}

	}

	private GameObject GetLight (int g_teamIndex, int g_lightIndex) {
		return myLightList [myWinnerScore * g_teamIndex + g_lightIndex];
	}

}
