using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ScoreManager : MonoBehaviour {

	private static CS_ScoreManager instance = null;

	//========================================================================
	public static CS_ScoreManager Instance {
		get { 
			return instance;
		}
	}

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
//		DontDestroyOnLoad(this.gameObject);
	}
	//========================================================================

	[SerializeField] CS_ScoreboardNumber[] team0Scores;
    [SerializeField] CS_ScoreboardNumber[] team1Scores;
	[SerializeField] CS_ScoreboardNumber[] team2Scores;
	[SerializeField] CS_ScoreboardNumber[] team3Scores;
	private List <CS_ScoreboardNumber[]> teamScores = new List<CS_ScoreboardNumber[]>();


	[SerializeField] GameObject[] wordTeam;
	[SerializeField] Vector3 wordTeam_startPos = new Vector3 (-10, 10, 5);
	[SerializeField] Vector3 wordTeam_posDifference = new Vector3 (3, 0, 0);

	[SerializeField] GameObject[] wordNumber;
	[SerializeField] Vector3 wordNumbers_pos = new Vector3 (10, 10, 5);

	[SerializeField] GameObject[] wordWin;
	[SerializeField] Vector3 wordWin_startPos = new Vector3 (-3, 10, 5);
	[SerializeField] Vector3 wordWin_posDifference = new Vector3 (3, 0, 0);

	[SerializeField] GameObject[] wordTie;
	[SerializeField] Vector3 wordTie_startPos = new Vector3 (-3, 10, 0);
	[SerializeField] Vector3 wordTie_posDifference = new Vector3 (3, 0, 0);

	private bool stopScore = false;


	// Use this for initialization
	void Start () {
//		ShowWords (0);

		teamScores.Add (team0Scores);
		teamScores.Add (team1Scores);
		teamScores.Add (team2Scores);
		teamScores.Add (team3Scores);

		for (int i = 0; i < teamScores.Count; i++) {
			foreach (CS_ScoreboardNumber scoreboard in teamScores[i]) {
				scoreboard.GetComponent<SpriteRenderer>().color = CS_PlayerManager.Instance.GetTeamColor(i);
			}

		}
			
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddScore (int g_teamNumber) {
		if (stopScore)
			return;
        
		foreach(CS_ScoreboardNumber scoreboard in teamScores[g_teamNumber]) {
                scoreboard.AddScore();
            }
       
	}

	public void ShowWinningTeam () {
		stopScore = true;

		int maxScore = 0;
		int winnerTeamNr = 0;

		for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount(); i++) {

			if (teamScores [i] [0].GetScore () > maxScore) {

				maxScore = teamScores [i] [0].GetScore ();
				winnerTeamNr = i;

			} else if (teamScores [i] [0].GetScore () == maxScore) {

				winnerTeamNr = -1;
			}

		}

		if (winnerTeamNr == -1) {
			ShowTie ();

		} else {
			ShowWords (winnerTeamNr);
		}
			
	}

	void ShowTie () {
		Color t_color = Color.grey;

		for (int i = 0; i < wordTie.Length; i++) {
			GameObject f_word = Instantiate (wordTie [i], wordTie_startPos + wordTie_posDifference * i, Quaternion.Euler (0, 180, 0));
			f_word.GetComponent<MeshRenderer> ().material.color = t_color;
		}
	}


	void ShowWords (int g_teamNum) {
		Color t_color = CS_PlayerManager.Instance.GetTeamColor (g_teamNum);

		for (int i = 0; i < wordTeam.Length; i++) {
			GameObject f_word = Instantiate (wordTeam [i], wordTeam_startPos + wordTeam_posDifference * i, Quaternion.Euler (0, 180, 0));
			f_word.GetComponent<MeshRenderer> ().material.color = t_color;
		}

		GameObject t_word = Instantiate (wordNumber [g_teamNum], wordNumbers_pos, Quaternion.Euler (0, 180, 0));
		t_word.GetComponent<MeshRenderer> ().material.color = t_color;

		for (int i = 0; i < wordWin.Length; i++) {
			GameObject f_word = Instantiate (wordWin [i], wordWin_startPos + wordWin_posDifference * i, Quaternion.Euler (0, 180, 0));
			f_word.GetComponent<MeshRenderer> ().material.color = t_color;
		}
	}

		
}
