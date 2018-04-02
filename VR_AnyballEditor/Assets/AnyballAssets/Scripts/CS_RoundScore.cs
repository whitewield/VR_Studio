using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Rule;

public class CS_RoundScore : MonoBehaviour {

	private static CS_RoundScore instance = null;

	public static CS_RoundScore Instance {
		get { 
			return instance;
		}
	}

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
	}

	[SerializeField] TextMesh[] roundTexts;
	[SerializeField] TextMesh[] scoresText1;
	[SerializeField] TextMesh[] scoresText2;
	[SerializeField] TextMesh[] scoresText3;
	[SerializeField] TextMesh[] scoresText4;
	private List<TextMesh[]> scoreTextList = new List<TextMesh[]>();

	int[] teamScores = {0,0,0,0};


	int roundNr = 1;


	void Start () {
		scoreTextList.Add (scoresText1);
		scoreTextList.Add (scoresText2);
		scoreTextList.Add (scoresText3);
		scoreTextList.Add (scoresText4);

		for (int i = 0; i < scoreTextList.Count; i++) {
			foreach (TextMesh t in scoreTextList[i]) {
				t.color = CS_PlayerManager.Instance.GetTeamColor (i);
			}
		
		}

	}
	
	public void AddRoundScore (int teamNr) {
		teamScores [teamNr]++;

		foreach (TextMesh t in scoreTextList[teamNr]) {
			t.text = teamScores[teamNr].ToString ("0");
		}
	}

	public void NextRound () {
		// reset
		for (int i = 0; i < teamScores.Length; i++) {
			teamScores [i] = 0;
		}

		for (int i = 0; i < scoreTextList.Count; i++) {
			foreach (TextMesh t in scoreTextList[i]) {
				t.text = teamScores[i].ToString ("0");
			}
		}

		roundNr++;

		foreach(TextMesh roundText in roundTexts) {
			roundText.text = "ROUND :" + roundNr.ToString ("0");
		}
	}

	public void EndRound() {

		int maxScore = 0;
		List<int> winnerTeamNums = new List<int> ();

		for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount(); i++) {

			if (teamScores [i] > maxScore) {

				maxScore = teamScores [i];
				winnerTeamNums.Clear ();
				winnerTeamNums.Add (i);

			} else if (teamScores [i] == maxScore) {

				winnerTeamNums.Add (i);
//				Debug.Log("add:" i);
			}

		}

		foreach (int f_num in winnerTeamNums) {
			CS_ScoreManager.Instance.AddScore (f_num);
		}
		Debug.Log ("NextRound:" + roundNr);

		if (roundNr == 3) {
			CS_ScoreManager.Instance.ShowWinningTeam ();
			return;
		}




//		CS_DigitalClock.Instance.ResetClock ();

	}
}
