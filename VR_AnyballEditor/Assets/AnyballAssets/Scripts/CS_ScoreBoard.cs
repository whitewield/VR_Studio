using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Rule;

public class CS_ScoreBoard : MonoBehaviour {
	private int myTeamNumber;

	[SerializeField] int mySize;
	private float myLastSize;

	[SerializeField] Transform myPart_Middle;
	[SerializeField] float myDifference_Middle;
	[SerializeField] float myRatio_Middle;

	[SerializeField] Transform myPart_Top;
	[SerializeField] float myDifference_Top;
	[SerializeField] float myRatio_Top;

	[SerializeField] Transform myScoreParentTransform;
	[SerializeField] GameObject myScorePrefab;
	[SerializeField] float myScoreScale = 0.6f;
	[SerializeField] float myScoreOffsetZ;
	[SerializeField] float myDifference_Score;
	[SerializeField] float myRatio_Score;
	private List<TextMesh> myScoreList = new List<TextMesh> ();

	private Color myColor = Color.white;

	// Use this for initialization
	void Start () {
		myLastSize = 0;
	}

	public void Init (int g_teamNumber, Color g_color, Vector3 g_position) {
		myTeamNumber = g_teamNumber;
		SetColor (g_color);
		this.transform.position = g_position;
	}

	// Update is called once per frame
	void Update () {
		Update_Change ();
		Update_ShowScore ();
	}

	private void Update_Change () {
		if (myLastSize != mySize) {
			ChangeSize (mySize);
		}
	}

	private void Update_Change_Size () {
		myPart_Middle.localScale = new Vector3 (1, mySize, 1);

		myPart_Middle.localPosition = new Vector3 (0, myDifference_Middle + mySize * myRatio_Middle, 0);
		myPart_Top.localPosition = new Vector3 (0, myDifference_Top + mySize * myRatio_Top, 0);

	}

	private void Update_Change_Score () {
		int t_newAmount = mySize * 2 - myScoreList.Count;
		if (t_newAmount > 0) {
			for (int i = 0; i < t_newAmount; i++) {
				GameObject t_score = Instantiate (myScorePrefab, myScoreParentTransform) as GameObject;
				myScoreList.Add (t_score.GetComponent<TextMesh> ());

				int f_index = myScoreList.Count - 1;

				Vector3 f_localPosition = new Vector3 (0, myDifference_Score + (f_index / 2 + 1) * myRatio_Score, 0);

				if (f_index % 2 == 0) {
					f_localPosition.z = myScoreOffsetZ;
					t_score.transform.localRotation = Quaternion.Euler (0, 0, 0);
				} else {
					f_localPosition.z = myScoreOffsetZ * -1;
					t_score.transform.localRotation = Quaternion.Euler (0, 180, 0);
				}

				t_score.transform.localPosition = f_localPosition;


				t_score.transform.localScale = Vector3.one * myScoreScale;

				myScoreList [f_index].color = myColor;
			}
		}

		for (int j = 0; j < myScoreList.Count; j++) {
			if (j < mySize * 2) {
				//should display
				myScoreList [j].gameObject.SetActive (true);
			} else {
				//should hide
				myScoreList [j].gameObject.SetActive (false);
			}
		}

//		ShowScore (0, 0);
	}

	private void Update_ShowScore () {
		int t_sequenceCount = CS_RuleManager.Instance.GetSequenceCount ();
		if (t_sequenceCount == 0) {
			return;
		}

		ChangeSize (t_sequenceCount);

		for (int i = 0; i < t_sequenceCount; i++) {
			int f_score = CS_RuleManager.Instance.GetScore (i, CS_PlayerManager.Instance.GetIndexNumber (myTeamNumber));
//			Debug.Log ("seq:" + i + "::" + f_score);
			ShowScore (i, f_score);
		}
	}

	private void SetColor (Color g_color) {
		myColor = g_color;

		for (int i = 0; i < myScoreList.Count; i++) {
			myScoreList [i].color = g_color;
		}
	}

	private void ShowScore (int g_sequenceNumber, int g_score) {
		for (int i = 0; i < 2; i++) {
			myScoreList [g_sequenceNumber + i].text = g_score.ToString ("0");
		}
	}

	private void ChangeSize (int g_size) {
		mySize = g_size;
		Update_Change_Size ();
		Update_Change_Score ();
		myLastSize = mySize;
	}
}
