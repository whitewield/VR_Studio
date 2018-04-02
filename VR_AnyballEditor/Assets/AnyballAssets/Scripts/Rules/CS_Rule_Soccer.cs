using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using AnyBall.Property;
using Hang.PoppingParticlePool;
using Hang.AiryAudio;

namespace AnyBall {
	namespace Rule {
		public class CS_Rule_Soccer : CS_Rule {

			private CS_Prop_SpawnArea mySpawnArea;

			[SerializeField] float myGoalSize = 3;
			[SerializeField] SpecialGoalSize[] mySpecialGoalSize;
			private GameObject myBallPrefab;
			private GameObject myBall;
			private GameObject myGoalPrefab;
			private List<GameObject> myGoals = new List<GameObject> ();

			public override bool Init (CS_RuleSet g_ruleSet) {
				if (!base.Init (g_ruleSet))
					return false;

				if (myRuleInfo.isTeamBased == false)
					Debug.LogError (this.gameObject.name + " need to be TeamBased!");

				if (myRuleInfo.myScoreType != ScoreType.Trigger)
					Debug.LogError (this.gameObject.name + " need to be Trigger!");

				//find spawn area
				mySpawnArea = CS_GameManager.Instance.GetRandomSpawnArea ();

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
					f_goal.GetComponent<CS_Prop_Goal> ().AddRule (this);
					if (t_specialSize == -1) {
						f_goal.transform.localScale = f_goal.transform.localScale * myGoalSize;
					} else {
						f_goal.transform.localScale = f_goal.transform.localScale * t_specialSize;
					}
					//add goal to the list
					myGoals.Add (f_goal);
				}


				Vector3[] t_positions = mySpawnArea.GetRandomPoints (myGoals.Count);

				//move the goal and look at center
				for (int i = 0; i < myGoals.Count; i++) {
					myGoals [i].transform.position = t_positions [i];
					myGoals [i].transform.LookAt (mySpawnArea.transform.position);
				}

//				if (CS_Map.Instance != null && CS_Map.Instance.GetSpawnPositions ().Count >= CS_GameManager.Instance.GetTeamCount ()) {
//					Debug.Log ("using CS_Map!");
//					for (int i = 0; i < myGoals.Count; i++) {
//						Vector3 t_position = CS_Map.Instance.GetSpawnPositions () [i].position;
//						t_position = new Vector3 (t_position.x, 0, t_position.z);
//						myGoals [i].transform.position = t_position + Vector3.up * Constants.DISTANCE_INIT_HEIGHT;
//						myGoals [i].transform.LookAt (Vector3.up * Constants.DISTANCE_INIT_HEIGHT);
//					}
//				} else {
//					//calculate the angle for each goal
//					float t_baseAngle = Random.Range (0, 360f);
//					float t_deltaAngle = 360f / CS_GameManager.Instance.GetTeamCount ();
//
//					//move the goal and look at center
//					for (int i = 0; i < myGoals.Count; i++) {
//						myGoals [i].transform.position = 
//						Quaternion.AngleAxis (t_baseAngle + t_deltaAngle * i, Vector3.up) * Vector3.forward * myGoalDistanceToCenter +
//						Vector3.up * Constants.DISTANCE_INIT_HEIGHT;
//						myGoals [i].transform.LookAt (Vector3.up * Constants.DISTANCE_INIT_HEIGHT);
//					}
//				}

				//find balls
				myBallPrefab = CS_EverythingManager.Instance.GetRandomPrefab (Constants.NAME_PROP_BALL);
				InitBall ();

				return true;
			}

			public override void Active (int g_index) {
				isActive [g_index] = true;
				myGoals [g_index].GetComponent<CS_Prop_Color> ().SetColor (
					CS_PlayerManager.Instance.GetTeamColorFromIndex (g_index)
				);

				myBall.GetComponent<CS_Prop_Color> ().SetColorBack ();
			}

			public override void Inactive (int g_index) {
				isActive [g_index] = false;
				myGoals [g_index].GetComponent<CS_Prop_Color> ().SetColor (
					CS_PlayerManager.Instance.GetInactiveColor ()
				);

				bool t_noActive = true;
				foreach (bool f_isActive in isActive) {
					if (f_isActive == true) {
						t_noActive = false;
						break;
					}
				}

				if (t_noActive == true)
					myBall.GetComponent<CS_Prop_Color> ().SetColor (
						CS_PlayerManager.Instance.GetInactiveColor ()
					);
			}

			private void InitBall () {
				myBall = Instantiate (myBallPrefab, CS_EverythingManager.Instance.transform) as GameObject;
				myBall.name = myBallPrefab.name;
				myBall.transform.position = mySpawnArea.GetRandomPoint ();
			}


			public override void Enter (GameObject g_ball, GameObject g_goal) {
				if (g_ball.name != myBallPrefab.name)
					return;

				int t_index = myGoals.IndexOf (g_goal);
				if (isActive [t_index] == false)
					return;
				
				if (CheckGoal (t_index)) {
					//play particle
					ParticleSystem t_particle =
						PoppingParticlePoolManager.Instance.GetFromPool(Hang.PoppingParticlePool.ParticleType.Score);
					t_particle.transform.position = g_ball.transform.position;
					ParticleActions.SetColor (t_particle, CS_PlayerManager.Instance.GetTeamColorFromIndex (t_index));
					t_particle.Play();

                    //play sound
                    AiryAudioManager.Instance.GetAudioData("ScoreSounds").Play(g_goal.transform.position);

					
					Debug.Log ("PLAYEED");

					Destroy (g_ball);

					InitBall ();

					Goal (t_index);
				}
			}
		}
	}
}
