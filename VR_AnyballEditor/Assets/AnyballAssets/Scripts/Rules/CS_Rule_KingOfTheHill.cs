using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using AnyBall.Property;
using Hang.PoppingParticlePool;
using Hang.AiryAudio;

namespace AnyBall {
	namespace Rule {
		public class CS_Rule_KingOfTheHill : CS_Rule {

			[SerializeField] float myAreaTime = 5;
			[SerializeField] float myAreaDrainSpeed = 5;
			[SerializeField] float myAreaSize = 3;
			[SerializeField] SpecialGoalSize[] mySpecialAreaSize;
			private GameObject myAreaPrefab;
			private List<CS_Prop_Area> myAreas = new List<CS_Prop_Area> ();
			private List<int> isPlayerOnHill = new List<int> ();
			private List<float> myPlayerOnHillTime = new List<float> ();

			public override bool Init (CS_RuleSet g_ruleSet) {
				if (!base.Init (g_ruleSet))
					return false;

				if (myRuleInfo.myScoreType != ScoreType.Trigger)
					Debug.LogError (this.gameObject.name + " need to be Trigger!");


				//find goal
				myAreaPrefab = CS_EverythingManager.Instance.GetRandomPrefab (Constants.NAME_PROP_AREA);

				//check if is special goal size
				float t_specialSize = -1;
				foreach (SpecialGoalSize f_setting in mySpecialAreaSize) {
					if (f_setting.prefab == myAreaPrefab) {
						t_specialSize = f_setting.size;
					}
				}

				for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
					isPlayerOnHill.Add (0);
					myPlayerOnHillTime.Add (0);
				}

				if (myRuleInfo.isTeamBased) {
					//create goals accoring to team count
					for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
						GameObject f_goal = Instantiate (myAreaPrefab, CS_EverythingManager.Instance.transform) as GameObject;
						f_goal.transform.rotation = Quaternion.identity;

						CS_Prop_Area t_area = f_goal.GetComponentInChildren<CS_Prop_Area> ();
						t_area.AddRule (this);
						t_area.SetTeamNumber (i);
						if (t_specialSize == -1) {
							f_goal.transform.localScale = f_goal.transform.localScale * myAreaSize;
						} else {
							f_goal.transform.localScale = f_goal.transform.localScale * t_specialSize;
						}
						//add goal to the list
						myAreas.Add (t_area);
					}

					//move the goal and look at center
					for (int i = 0; i < myAreas.Count; i++) {
						CS_Prop_SpawnArea t_area = CS_GameManager.Instance.GetRandomSpawnArea ();
						myAreas [i].transform.position = t_area.GetRandomPoint ();
						myAreas [i].transform.LookAt (t_area.transform.position);
					}
				} else {
					GameObject f_goal = Instantiate (myAreaPrefab, CS_EverythingManager.Instance.transform) as GameObject;
					CS_Prop_Area t_area = f_goal.GetComponentInChildren<CS_Prop_Area> ();
					t_area.AddRule (this);
					if (t_specialSize == -1) {
						f_goal.transform.localScale = f_goal.transform.localScale * myAreaSize;
					} else {
						f_goal.transform.localScale = f_goal.transform.localScale * t_specialSize;
					}
					//add goal to the list
					myAreas.Add (t_area);
				}

				return true;
			}

			public override void Active (int g_index) {
				isActive [g_index] = true;
				if (myRuleInfo.isTeamBased) {
					myAreas [g_index].SetColor (
						CS_PlayerManager.Instance.GetTeamColorFromIndex (g_index)
					);
				}
			}

			public override void Inactive (int g_index) {
				isActive [g_index] = false;
				if (myRuleInfo.isTeamBased) {
					myAreas [g_index].SetColor (
						CS_PlayerManager.Instance.GetInactiveColor ()
					);
//					isPlayerOnHill [g_index] = 0;
					myPlayerOnHillTime [g_index] = 0;
					isOn [g_index] = false;
				}
			}

			public override void Enter (GameObject g_player, GameObject g_goal) {
//				Debug.Log ("Enter!");
//				if (!isInitialized)
//					return;



				CS_PlayerControl t_playerControl = g_player.GetComponent<CS_PlayerControl> ();
				if (t_playerControl == null) {
					return;
				}

				int t_index = CS_PlayerManager.Instance.GetIndexNumber (t_playerControl.GetTeam ());

//				if (isActive [t_index] == false)
//					return;

				if (myRuleInfo.isTeamBased) {
					if (myAreas.IndexOf (g_goal.GetComponentInChildren<CS_Prop_Area> ()) == t_index) {
						isPlayerOnHill [t_index] += 1;
//						Debug.Log ("enter3");
						isOn [t_index] = true;
					}
				} else {
					isPlayerOnHill [t_index] += 1;
				}
			}

			public override void Exit (GameObject g_player, GameObject g_goal) {
				//				Debug.Log ("Exit!");
//				if (!isInitialized)
//					return;

				CS_PlayerControl t_playerControl = g_player.GetComponent<CS_PlayerControl> ();
				if (t_playerControl == null) {
					return;
				}

				int t_index = CS_PlayerManager.Instance.GetIndexNumber (t_playerControl.GetTeam ());

				if (myRuleInfo.isTeamBased) {
					if (myAreas.IndexOf (g_goal.GetComponentInChildren<CS_Prop_Area> ()) == t_index) {
						isPlayerOnHill [t_index] -= 1;
						if (isPlayerOnHill [t_index] < 0) {
							isPlayerOnHill [t_index] = 0;
							Debug.LogWarning ("player number on the hill is less than 0?!");
						}
						isOn [t_index] = false;
					}
				} else {
					isPlayerOnHill [t_index] -= 1;
					if (isPlayerOnHill [t_index] < 0) {
						isPlayerOnHill [t_index] = 0;
						Debug.LogWarning ("player number on the hill is less than 0?!");
					}
				}
			}

			protected override void Update () {
				for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
					if (isActive [i] == true && isPlayerOnHill [i] > 0 && CheckGoal (i)) {
						//increase
						myPlayerOnHillTime [i] += Time.deltaTime;
						if (myPlayerOnHillTime [i] > myAreaTime) {
							myPlayerOnHillTime [i] -= myAreaTime;
							Goal (i);
							//play particle
							ParticleSystem t_particle =
								PoppingParticlePoolManager.Instance.GetFromPool(Hang.PoppingParticlePool.ParticleType.Score);
							t_particle.transform.position = myAreas[i].transform.position;
							ParticleActions.SetColor (t_particle, CS_PlayerManager.Instance.GetTeamColorFromIndex (i));
							t_particle.Play();

                            //play sound
                            AiryAudioManager.Instance.GetAudioData("ScoreSounds").Play(myAreas[i].transform.position);
						}
					} else if (myPlayerOnHillTime [i] > 0) {
						//drain
						myPlayerOnHillTime [i] -= Time.deltaTime * myAreaDrainSpeed;
						if (myPlayerOnHillTime [i] < 0)
							myPlayerOnHillTime [i] = 0;
					}

					//show the progress
					myAreas [i].SetProgress (myPlayerOnHillTime [i] / myAreaTime);
				}

			}


		}
	}
}
