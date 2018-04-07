using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

namespace AnyBall {
	namespace Rule {
		public class CS_RuleManager : MonoBehaviour {

			private static CS_RuleManager instance = null;

			public static CS_RuleManager Instance {
				get { 
					return instance;
				}
			}

			private GameObject[] myRulePrefabs;

//			[SerializeField] bool test = false;
//			[SerializeField] GameObject myTestRule;

			List<CS_RuleSequence> myAllRules = new List<CS_RuleSequence> ();

			void Awake () {
				if (instance != null && instance != this) {
					Destroy(this.gameObject);
					return;
				} else {
					instance = this;
				}

				myRulePrefabs = Resources.LoadAll<GameObject> (Constants.PATH_RULES);
			}

			// Use this for initialization
			void Start () {
//				if (test && myTestRule != null) {
//					Instantiate (myTestRule, this.transform);
//				} else {
//					Instantiate (myRulePrefabs [Random.Range (0, myRulePrefabs.Length)], this.transform);
//				}

//				AddRule ();
//
//				InitRules ();
			}

			// Update is called once per frame
			void Update () {
				
			}

			/// <summary>
			/// Adds a new rule.
			/// </summary>
			public void AddRule () {

				// create new rule
				int t_num = Random.Range (0, myRulePrefabs.Length);
				CS_Rule t_rule = Instantiate (myRulePrefabs [t_num], this.transform).GetComponent<CS_Rule> ();
				t_rule.gameObject.name = myRulePrefabs [t_num].name;

				// a list of rule set that can add the rule
				List<CS_RuleSet> t_combineList = new List<CS_RuleSet> ();
				for (int i = 0; i < myAllRules.Count; i++) {
					for (int j = 0; j < myAllRules [i].mySequence.Count; j++) {
						if (myAllRules [i].mySequence [j].CheckCombine (t_rule))
							t_combineList.Add (myAllRules [i].mySequence [j]);
					}
				}

				// if the rule can be combined
				if (t_combineList.Count > 0 && Random.Range(0f, 1f) > 0.2f) {
					// pick a random rule set from the list and add the rule into the rule set
					t_combineList [Random.Range (0, t_combineList.Count)].AddRule (t_rule);
					t_combineList.Clear ();
					return;
				}
				t_combineList.Clear ();

				// create a new rule set with the rule
				CS_RuleSet t_ruleSet = new CS_RuleSet (t_rule);
				// a list of rule sequence that can add the rule set
				List<CS_RuleSequence> t_sequenceList = new List<CS_RuleSequence> ();
				for (int i = 0; i < myAllRules.Count; i++) {
					if (myAllRules [i].CheckSequence (t_ruleSet)) {
						t_sequenceList.Add (myAllRules [i]);
					}
				}

				// if the rule set can be part of the sequence
				if (t_sequenceList.Count > 0 && Random.Range(0f, 1f) > 0.3f) {
					// pick a random rule sequence from the list and add the rule set into the sequence
					t_sequenceList [Random.Range (0, t_combineList.Count)].AddRuleSet (t_ruleSet);
					t_sequenceList.Clear ();
					return;
				}
				t_sequenceList.Clear ();

				// if didn't combine/put into sequence, add a new sequence with the rule set
				CS_RuleSequence t_ruleSequence = new CS_RuleSequence (t_ruleSet);
				myAllRules.Add (t_ruleSequence);
			}

			public void InitRules () {
				// init all sequences
				foreach (CS_RuleSequence f_sequence in myAllRules) {
					f_sequence.Init (CS_PlayerManager.Instance.GetTeamCount ());
				}


			}

			public void NextRound () {
				AddRule ();
				InitRules ();
			}

			public int GetSequenceCount () {
				return myAllRules.Count;
			}

			public int GetScore (int g_sequenceIndex, int g_index) {
				return myAllRules [g_sequenceIndex].GetScore (g_index);
			}

			public List<int> GetRoundScore () {
				int t_teamCount = CS_PlayerManager.Instance.GetTeamCount ();

				List<int> t_roundScore = new List<int> ();
				for (int i = 0; i < t_teamCount; i++) {
					t_roundScore.Add (0);
				}

				for (int i = 0; i < myAllRules.Count; i++) {
					List<int> f_winnerIndexList = new List<int> ();
					int f_winningScore = 0;
					for (int j = 0; j < t_teamCount; j++) {
						int f_score = myAllRules [i].GetScore (j);
						if (f_score > f_winningScore) {
							f_winningScore = f_score;
							f_winnerIndexList.Clear ();
							f_winnerIndexList.Add (j);
						} else if (f_score == f_winningScore) {
							f_winnerIndexList.Add (j);
						}
					}

					for (int j = 0; j < f_winnerIndexList.Count; j++) {
						int f_winnerIndex = f_winnerIndexList [j];
						t_roundScore [f_winnerIndex]++;
					}
				}

				return t_roundScore;
			}
		}
	}
}
