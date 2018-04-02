using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using AnyBall.Property;

namespace AnyBall {
	namespace Rule {
		public class CS_Rule_SoccerOld : CS_Rule {

			[SerializeField] int myBallTypeCount = 3;
			[SerializeField] float myBallDistanceToCenter = 5;
			[SerializeField] float myGoalDistanceToCenter = 10;
			[SerializeField] float myGoalSize = 3;
			[SerializeField] SpecialGoalSize[] mySpecialGoalSize;
			private List<GameObject> myBallPrefabList = new List<GameObject> ();
			private GameObject myGoalPrefab;
			private List<GameObject> myGoals = new List<GameObject> ();



			// Use this for initialization
			protected override void Start () {
				//find goal
				myGoalPrefab = CS_EverythingManager.Instance.GetRandomPrefab (Constants.NAME_PROP_GOAL);

				//check if is special goal size
				float t_specialSize = -1;
				foreach (SpecialGoalSize f_setting in mySpecialGoalSize) {
					if (f_setting.prefab == myGoalPrefab) {
						t_specialSize = f_setting.size;
					}
				}

				//create goals accoring to team count
				for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
					GameObject f_goal = Instantiate (myGoalPrefab, CS_EverythingManager.Instance.transform) as GameObject;
//					f_goal.GetComponent<CS_Prop_Goal> ().SetColor (CS_GameManager.Instance.GetTeamColor (i));
					f_goal.GetComponent<CS_Prop_Goal> ().AddRule (this);
					if (t_specialSize == -1) {
						f_goal.transform.localScale = f_goal.transform.localScale * myGoalSize;
					} else {
						f_goal.transform.localScale = f_goal.transform.localScale * t_specialSize;
					}
					//add goal to the list
					myGoals.Add (f_goal);
				}

				//calculate the angle for each goal
				float t_baseAngle = Random.Range (0, 360f);
				float t_deltaAngle = 360f / CS_PlayerManager.Instance.GetTeamCount ();

				//move the goal and look at center
				for (int i = 0; i < myGoals.Count; i++) {
					myGoals [i].transform.position = 
						Quaternion.AngleAxis (t_baseAngle + t_deltaAngle * i, Vector3.up) * Vector3.forward * myGoalDistanceToCenter +
						Vector3.up * Constants.DISTANCE_INIT_HEIGHT;
					myGoals [i].transform.LookAt (Vector3.up * Constants.DISTANCE_INIT_HEIGHT);
				}

				//find balls
				myBallPrefabList = CS_EverythingManager.Instance.GetRandomPrefabs (Constants.NAME_PROP_BALL, myBallTypeCount);

				AddBall ();
			}

			// Update is called once per frame
			protected override void Update () {

			}

			private void AddBall () {
				GameObject t_prefab = myBallPrefabList [Random.Range (0, myBallPrefabList.Count)];
				GameObject t_ball = 
					Instantiate (
						t_prefab,
						CS_EverythingManager.Instance.transform
					) as GameObject;

				t_ball.name = t_prefab.name;
				t_ball.transform.position = 
					(Vector3)(Random.insideUnitCircle * myBallDistanceToCenter) +
					Vector3.up * Constants.DISTANCE_INIT_HEIGHT;
			}

			public override void Enter (GameObject g_ball, GameObject g_goal) {
				Debug.Log ("Enter!");
				foreach (GameObject f_ball in myBallPrefabList) {
					if (g_ball.name == f_ball.name) {
						//add score

						CS_ScoreManager.Instance.AddScore (myGoals.IndexOf (g_goal));

						Destroy (g_ball);
						AddBall ();
					}
				}
			}
		}
	}
}
