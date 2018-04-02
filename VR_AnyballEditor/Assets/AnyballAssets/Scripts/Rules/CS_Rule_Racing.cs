using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Global;
using AnyBall.Property;
using Hang.PoppingParticlePool;
using Hang.AiryAudio;

namespace AnyBall {
	namespace Rule {
		public class CS_Rule_Racing : CS_Rule {

			[SerializeField] int myFlagCount = 3;

			private GameObject myGoalPrefab;
			private GameObject myGoal;
			private GameObject myFlagPrefab;
			private List<Dictionary<GameObject, bool>> myFlags = new List<Dictionary<GameObject, bool>> ();

			// Use this for initialization
			public override bool Init (CS_RuleSet g_ruleSet) {
				if (!base.Init (g_ruleSet))
					return false;

				if (myRuleInfo.isTeamBased == false)
					Debug.LogError (this.gameObject.name + " need to be TeamBased!");

				if (myRuleInfo.myScoreType != ScoreType.Trigger)
					Debug.LogError (this.gameObject.name + " need to be Trigger!");
				
				//find goal
				myGoalPrefab = CS_EverythingManager.Instance.GetRandomPrefab (Constants.NAME_PROP_GOAL_REACH);

				//create goal 
				myGoal = Instantiate (myGoalPrefab, CS_EverythingManager.Instance.transform) as GameObject;
				myGoal.transform.position = 
					CS_GameManager.Instance.GetRandomSpawnArea ().GetRandomPoint ();
				myGoal.GetComponent<CS_Prop_Goal> ().AddRule (this);

				//find flag
				myFlagPrefab = CS_EverythingManager.Instance.GetRandomPrefab (Constants.NAME_PROP_FLAG);

				//create flags accoring to team count
				for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
					myFlags.Add (new Dictionary<GameObject, bool> ());

					for (int j = 0; j < myFlagCount; j++) {
						GameObject f_flag = Instantiate (myFlagPrefab, CS_EverythingManager.Instance.transform) as GameObject;
//						f_flag.GetComponent<CS_Prop_Flag> ().SetColor (CS_GameManager.Instance.GetTeamColor (i));
						f_flag.GetComponent<CS_Prop_Flag> ().AddRule (this);
						f_flag.transform.position = 
							CS_GameManager.Instance.GetRandomSpawnArea ().GetRandomPoint ();
						//add goal to the list
						myFlags[i].Add (f_flag, false);
					}
				}

				return true;
			}

			public override void Active (int g_index) {
				Debug.Log ("racing active");

				isActive [g_index] = true;

				foreach (GameObject key in myFlags [g_index].Keys.ToList()) {
					key.GetComponent<CS_Prop_Color> ().SetColor (
						CS_PlayerManager.Instance.GetTeamColorFromIndex (g_index)
					);

					myFlags [g_index] [key] = false;
				}

				myGoal.GetComponent<CS_Prop_Color> ().SetColorBack ();
			}

			public override void Inactive (int g_index) {
				isActive [g_index] = false;

				foreach (GameObject key in myFlags [g_index].Keys.ToList()) {
					key.GetComponent<CS_Prop_Color> ().SetColor (
						CS_PlayerManager.Instance.GetInactiveColor ()
					);

					myFlags [g_index] [key] = false;
				}

				bool t_noActive = true;
				foreach (bool f_isActive in isActive) {
					if (f_isActive == true) {
						t_noActive = false;
						break;
					}
				}

				if (t_noActive == true)
					myGoal.GetComponent<CS_Prop_Color> ().SetColor (
						CS_PlayerManager.Instance.GetInactiveColor ()
					);
			}

			public override void Enter (GameObject g_player, GameObject g_goal) {
//				Debug.Log ("Enter!");

				CS_PlayerControl t_playerControl = g_player.GetComponent<CS_PlayerControl> ();
				if (t_playerControl == null) {
					return;
				}

				int t_index = CS_PlayerManager.Instance.GetIndexNumber (t_playerControl.GetTeam ());
				if (isActive [t_index] == false)
					return;

				if (myFlags [t_index].ContainsKey (g_goal)) {
					if (myFlags [t_index] [g_goal] == false) {
						
						//active flag

						//play particle
						ParticleSystem t_particle =
							PoppingParticlePoolManager.Instance.GetFromPool(Hang.PoppingParticlePool.ParticleType.ScoreSub);
						t_particle.transform.position = g_goal.transform.position + new Vector3(0,1.5f,0);
						ParticleActions.SetColor (t_particle, CS_PlayerManager.Instance.GetTeamColorFromIndex (t_index));
						t_particle.Play();

						//change flag color
						g_goal.GetComponent<CS_Prop_Color> ().SetColor (
							CS_PlayerManager.Instance.GetTeamColorFromIndex (t_index, true)
						);

						//play sound
                        AiryAudioManager.Instance.GetAudioData("ScoreSmallSounds").Play(g_goal.transform.position);

						myFlags [t_index] [g_goal] = true;
						Debug.Log ("flag");


					}
				} else if (g_goal == myGoal && CheckGoal (t_index)) {
					//check all touch

					bool t_allTouched = true;
					foreach (bool f_flagStatus in myFlags [t_index].Values) {
						if (f_flagStatus == false) {
							t_allTouched = false;
							break;
						}
					}

					if (t_allTouched) {

						//reset flags, using System.Linq;
						foreach (GameObject key in myFlags[t_index].Keys.ToList()) {
							myFlags [t_index] [key] = false;
						}

						//play particle
						ParticleSystem t_particle =
							PoppingParticlePoolManager.Instance.GetFromPool(Hang.PoppingParticlePool.ParticleType.Score);
						t_particle.transform.position = g_player.transform.position;
						ParticleActions.SetColor (t_particle, CS_PlayerManager.Instance.GetTeamColorFromIndex (t_index));
						t_particle.Play();

						//play sound
                        AiryAudioManager.Instance.GetAudioData("ScoreSounds").Play(g_goal.transform.position);

						Debug.Log ("PLAYEED");

						Goal (t_index);

					} else {
						Debug.Log ("Missing Flag");
					}
				}
			}
		}
	}
}
