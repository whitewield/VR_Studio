using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Rule;

public class CS_SingleScoreBoard : MonoBehaviour {

	private static CS_SingleScoreBoard instance = null;
	public static CS_SingleScoreBoard Instance { get { return instance; } }

	[SerializeField] Transform[] myParts;
	/// <summary>
	/// 0,1,2
	/// 3,4,5
	/// 6,7,8
	/// </summary>

	[SerializeField] int myDefaultTeamCount = 4;
	[SerializeField] int myDefaultSequenceCount = 3;

	[SerializeField] Vector2 mySpacing = new Vector2 (2, 1);

	private int myLastTeamCount = 0;
	private int myLastSequenceCount = 0;

	[SerializeField] Transform myScoreParentTransform;
	[SerializeField] GameObject myScorePrefab;
	private List<TextMesh> myScoreList = new List<TextMesh> ();

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

	}
	
	// Update is called once per frame
	void Update () {
		Update_ShowScore ();
	}

	private void Update_ShowScore () {
		if (CS_RuleManager.Instance == null)
			return;
		int t_sequenceCount = CS_RuleManager.Instance.GetSequenceCount ();
		if (t_sequenceCount == 0)
			return;

		int t_teamCount = CS_PlayerManager.Instance.GetTeamCount ();
		if (t_teamCount == 0)
			return;

		if (t_sequenceCount != myLastSequenceCount || t_teamCount != myLastTeamCount) {
			myLastSequenceCount = t_sequenceCount;
			myLastTeamCount = t_teamCount;
			ChangeSize ();
			ChangeScoreDisplay ();
		}

		int t_count = myLastTeamCount * myLastSequenceCount;
		for (int i = 0; i < t_count; i++) {
			// x -> sequence cnumber; y -> team number
			Vector2 f_index = new Vector2 (i % myLastSequenceCount, i / myLastSequenceCount); 

			int f_score = CS_RuleManager.Instance.GetScore ((int)(f_index.x), (int)(f_index.y));
						//			Debug.Log ("seq:" + i + "::" + f_score);

			// show score

			myScoreList [i].text = f_score.ToString ("0");
		}

//		for (int i = 0; i < t_sequenceCount; i++) {
//			int f_score = CS_RuleManager.Instance.GetScore (i, CS_PlayerManager.Instance.GetIndexNumber (myTeamNumber));
//			//			Debug.Log ("seq:" + i + "::" + f_score);
//			ShowScore (i, f_score);
//		}
	}

	private void ChangeSize () {

		for (int i = 0; i < myParts.Length; i++) {
			int f_r = i / 3;
			int f_c = i % 3;

			Vector2 f_pos = Vector2.zero;
			Vector2 f_size = Vector2.one;

			if (f_r == 0) {
				f_pos.y = (myLastTeamCount * mySpacing.y + 1) * 0.5f;
			} else if (f_r == 2) {
				f_pos.y = (myLastTeamCount * mySpacing.y + 1) * -0.5f;
			} else {
				f_size.y = mySpacing.y * myLastTeamCount;
			}

			if (f_c == 0) {
				f_pos.x = (myLastSequenceCount * mySpacing.x + 1) * -0.5f;
			} else if (f_c == 2) {
				f_pos.x = (myLastSequenceCount * mySpacing.x + 1) * 0.5f;
			} else {
				f_size.x = mySpacing.x * myLastSequenceCount;
			}

			myParts [i].localPosition = f_pos;

			myParts [i].localScale = new Vector3 (f_size.x, f_size.y, 1);

//			Debug.Log (f_r + ":" + f_c + " - " + f_pos + " - " + f_size);
		}
	}

	private void ChangeScoreDisplay () {
		int t_size = myLastTeamCount * myLastSequenceCount;
		int t_newAmount = t_size - myScoreList.Count;
		if (t_newAmount > 0) {
			for (int i = 0; i < t_newAmount; i++) {
				GameObject t_score = Instantiate (myScorePrefab, myScoreParentTransform) as GameObject;
				myScoreList.Add (t_score.GetComponent<TextMesh> ());

				int f_index = myScoreList.Count - 1;
			}
		}

		Vector2 t_topLeft = new Vector2 (
			                    -(myLastSequenceCount - 1) * mySpacing.x / 2, 
			                    (myLastTeamCount - 1) * mySpacing.y / 2
		                    );

		for (int j = 0; j < myScoreList.Count; j++) {
			if (j < t_size) {
				//should display
				myScoreList [j].gameObject.SetActive (true);

				Vector2 f_index = new Vector2 (j % myLastSequenceCount, j / myLastSequenceCount);

				//update position
				myScoreList [j].transform.localPosition = new Vector3 (
					f_index.x * mySpacing.x + t_topLeft.x, 
					-f_index.y * mySpacing.y + t_topLeft.y,
					0
				);

				//update color
				myScoreList [j].color = CS_PlayerManager.Instance.GetTeamColorFromIndex ((int)(f_index.y));

			} else {
				//should hide
				myScoreList [j].gameObject.SetActive (false);
			}
		}
	}
}
