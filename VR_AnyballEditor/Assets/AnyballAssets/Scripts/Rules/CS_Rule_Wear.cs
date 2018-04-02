using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using AnyBall.Property;
using Hang.PoppingParticlePool;
using Hang.AiryAudio;

namespace AnyBall {
	namespace Rule {
		public class CS_Rule_Wear : CS_Rule {

			[SerializeField] float myWearTime = 5;
			[SerializeField] float myWearDistance = 10;
			private GameObject myWearPrefab;
			private List<GameObject> myWears = new List<GameObject> ();
			private List<bool> isPlayerWearing = new List<bool> ();
			private List<float> myPlayerWearingTime = new List<float> ();

			public override bool Init (CS_RuleSet g_ruleSet) {
				if (!base.Init (g_ruleSet))
					return false;

				if (myRuleInfo.myScoreType != ScoreType.Duration)
					Debug.LogError (this.gameObject.name + " need to be Duration!");


				//find goal
				myWearPrefab = CS_EverythingManager.Instance.GetRandomPrefab (Constants.NAME_PROP_WEARABLE);

				for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
					isPlayerWearing.Add (false);
					myPlayerWearingTime.Add (0);
				}

				if (myRuleInfo.isTeamBased) {
					//create goals accoring to team count
					for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
						GameObject f_goal = Instantiate (myWearPrefab, CS_EverythingManager.Instance.transform) as GameObject;

						CS_Prop_Wearable t_wear = f_goal.GetComponent<CS_Prop_Wearable> ();
						t_wear.AddRule (this);
						t_wear.SetTeamNumber (i);
						//add goal to the list
						myWears.Add (f_goal);
					}

					//calculate the angle for each goal
					float t_baseAngle = Random.Range (0, 360f);
					float t_deltaAngle = 360f / CS_PlayerManager.Instance.GetTeamCount ();

					//move the goal and look at center
					for (int i = 0; i < myWears.Count; i++) {
						myWears [i].transform.position = 
							Quaternion.AngleAxis (t_baseAngle + t_deltaAngle * i, Vector3.up) * Vector3.forward * myWearDistance +
						Vector3.up * Constants.DISTANCE_INIT_HEIGHT;
					}
				} else {
					GameObject f_goal = Instantiate (myWearPrefab, CS_EverythingManager.Instance.transform) as GameObject;
					Vector3 f_startPos = Random.insideUnitSphere * myWearDistance;
					f_startPos.y = Constants.DISTANCE_INIT_HEIGHT;
					f_goal.transform.position = f_startPos;

					CS_Prop_Wearable t_wear = f_goal.GetComponent<CS_Prop_Wearable> ();
					t_wear.AddRule (this);

					myWears.Add (f_goal);

				}

				return true;
			}

			public override void Active (int g_index) {
				isActive [g_index] = true;
				if (myRuleInfo.isTeamBased) {
					myWears [g_index].GetComponent<CS_Prop_Color> ().SetColor (
						CS_PlayerManager.Instance.GetTeamColorFromIndex (g_index)
					);
				} else {
					myWears [0].GetComponent<CS_Prop_Color> ().SetColorBack ();
				}
			}

			public override void Inactive (int g_index) {
				isActive [g_index] = false;
				if (myRuleInfo.isTeamBased) {
					myWears [g_index].GetComponent<CS_Prop_Color> ().SetColor (
						CS_PlayerManager.Instance.GetInactiveColor ()
					);
					isPlayerWearing [g_index] = false;
					myPlayerWearingTime [g_index] = 0;
//					isOn [g_index] = false;
				} else {
					bool t_allFalse = true;
					foreach (bool f_isActive in isActive) {
						if(f_isActive == true){
							t_allFalse = false;
							break;
						}
					}
					if (t_allFalse)
						myWears [0].GetComponent<CS_Prop_Color> ().SetColor (
							CS_PlayerManager.Instance.GetInactiveColor ()
						);
				}
			}

			public override void Enter (GameObject g_player, GameObject g_wear) {
				Debug.Log ("Enter!");

				CS_PlayerControl t_playerControl = g_player.GetComponent<CS_PlayerControl> ();
				if (t_playerControl == null) {
					return;
				}

				int t_index = CS_PlayerManager.Instance.GetIndexNumber (t_playerControl.GetTeam ());

				if (isActive [t_index] == false)
					return;

				if (myRuleInfo.isTeamBased) {
					if (myWears.IndexOf (g_wear) == t_index) {
						isPlayerWearing [t_index] = true;
						isOn [t_index] = true;
					}
				} else {
					isPlayerWearing [t_index] = true;
					isOn [t_index] = true;
				}
			}

			public override void Exit (GameObject g_player, GameObject g_wear) {
				Debug.Log ("Exit!");

				CS_PlayerControl t_playerControl = g_player.GetComponent<CS_PlayerControl> ();
				if (t_playerControl == null) {
					return;
				}

				int t_index = CS_PlayerManager.Instance.GetIndexNumber (t_playerControl.GetTeam ());

				if (myRuleInfo.isTeamBased) {
					if (myWears.IndexOf (g_wear) == t_index) {
						isPlayerWearing [t_index] = false;
						isOn [t_index] = false;
					}
				} else {
					isPlayerWearing [t_index] = false;
					isOn [t_index] = false;
				}
			}

			protected override void Update () {
				for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
//					Debug.Log (isPlayerWearing.Count);
					if (isPlayerWearing [i] == true) {
						//increase
						myPlayerWearingTime [i] += Time.deltaTime;
						if (myPlayerWearingTime [i] > myWearTime) {
							myPlayerWearingTime [i] -= myWearTime;



							if (CheckGoal (i)) {

								//play particle
                                //play sound
								ParticleSystem t_particle =
									PoppingParticlePoolManager.Instance.GetFromPool(Hang.PoppingParticlePool.ParticleType.Score);
                                if (myRuleInfo.isTeamBased) {
                                    t_particle.transform.position = myWears[i].transform.position;
                                    AiryAudioManager.Instance.GetAudioData("ScoreSounds").Play(myWears[i].transform.position);
                                    } else {
                                        t_particle.transform.position = myWears[0].transform.position;
                                        ParticleActions.SetColor(t_particle, CS_PlayerManager.Instance.GetTeamColorFromIndex(i));
                                        t_particle.Play();
                                        AiryAudioManager.Instance.GetAudioData("ScoreSounds").Play(myWears[0].transform.position);
                                    }



								Goal (i);
							}
						}
					} else if (myPlayerWearingTime [i] > 0) {
						//drain
						myPlayerWearingTime [i] = 0;
					}

					//show the progress
//					myAreas [i].GetComponent<CS_Prop_Area> ().SetProgress (myPlayerOnHillTime [i] / myAreaTime);
				}

			}
		}
	}
}
