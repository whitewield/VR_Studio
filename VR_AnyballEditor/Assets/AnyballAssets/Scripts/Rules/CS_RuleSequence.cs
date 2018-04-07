using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

namespace AnyBall {
	namespace Rule {

		public class CS_RuleSequence {
			
			public List<CS_RuleSet> mySequence; // a sequence of rule sets
			private List<int> mySequenceIndexList = new List<int>();
			private List<int> myScoreList = new List<int> ();

			private bool extendable = true; // if the sequence is extandable, if all games in the sequence is team based, it should be extentable
			private bool hasKeeping = false; // if the sequence has keeping at the end

			/// <summary>
			/// Initializes a new instance of the <see cref="AnyBall.Rule.CS_RuleSequence"/> class.
			/// </summary>
			/// <param name="g_ruleSet">the rule set to initialize with.</param>
			public CS_RuleSequence (CS_RuleSet g_ruleSet) {
				// create a new sequence of rule set
				mySequence = new List<CS_RuleSet> ();

				//add the rule set in the sequence
				mySequence.Add (g_ruleSet);

				// is teamBased is extandable
				if (g_ruleSet.GetInfo ().isTeamBased)
					extendable = true;
				else
					extendable = false;

				// if the new rule is keeping, set keeping to true
				if (g_ruleSet.GetInfo ().myScoreType == ScoreType.Keeping)
					hasKeeping = true;
				else
					hasKeeping = false;
			}

			/// <summary>
			/// Adds the rule set to the sequence
			/// </summary>
			/// <returns><c>true</c>, if rule set was added, <c>false</c> otherwise.</returns>
			/// <param name="g_ruleSet">rule set you want to add.</param>
			public bool AddRuleSet (CS_RuleSet g_ruleSet) {
				if (CheckSequence (g_ruleSet) == false)
					return false;

				if (hasKeeping == true)
					// if have keeping, insert before keeping
					mySequence.Insert (mySequence.Count - 1, g_ruleSet);
				else
					// add the rule set to the end
					mySequence.Add (g_ruleSet);

				// if the new rule is keeping, set keeping to true
				if (g_ruleSet.GetInfo ().myScoreType == ScoreType.Keeping)
					hasKeeping = true;

				return true;
			}

			public bool CheckSequence (CS_RuleSet g_ruleSet) {
				// if is not extendable or the rule you want to add is not team base, return false
				if (extendable == false || g_ruleSet.GetInfo ().isTeamBased == false)
					return false;

				// if already have keeing and trying to add another keeping, return false
				if (hasKeeping == true && g_ruleSet.GetInfo ().myScoreType == ScoreType.Keeping)
					return false;

				return true;
			}

			public void Init (int g_teamCount) {
				mySequenceIndexList.Clear ();
				myScoreList.Clear ();

				for (int i = 0; i < g_teamCount; i++) {
					// init index for each team
					mySequenceIndexList.Add (0);
					// init score for each team
					myScoreList.Add (0);

//					CS_GameManager.Instance.myScoreBoards [CS_GameManager.Instance.GetTeamNumber (i)].ShowScore (
//						CS_RuleManager.Instance.GetSequenceIndex (this), 
//						0
//					);
				}
				// init the all rule set in the sequence
				for (int i = 0; i < mySequence.Count; i++) {
					mySequence [i].Init (this);

					// if its not the first ruleSet, inactive the rule set
					if (i == 0)
						mySequence [i].ActiveAll ();
					else
						mySequence [i].InactiveAll ();
				}
			}

			public void Goal (int g_index) {
				//t_index is the index of the team number;

				int t_indexSequence = mySequenceIndexList [g_index];

				mySequenceIndexList [g_index]++;
				if (mySequence.Count == mySequenceIndexList [g_index]) {
					mySequenceIndexList [g_index] = 0;
					myScoreList [g_index]++;

//					CS_RoundScore.Instance.AddRoundScore (g_index);

//					CS_GameManager.Instance.myScoreBoards [CS_GameManager.Instance.GetTeamNumber (g_index)].ShowScore (
//						CS_RuleManager.Instance.GetSequenceIndex (this), 
//						myScoreList [g_index]
//					);
				}

				if (t_indexSequence != mySequenceIndexList [g_index]) {
					// inactive the rule set for the team
					mySequence [t_indexSequence].Inactive (g_index);
					// active the next rule set for the team
					mySequence [mySequenceIndexList [g_index]].Active (g_index);
				} else {
					mySequence [t_indexSequence].Active (g_index);
				}
			}

			public void UpdateExtendable () {
				foreach (CS_RuleSet f_ruleSet in mySequence) {
					if (f_ruleSet.GetInfo ().isTeamBased == false) {
						extendable = false;
						return;
					}
				}
				extendable = true;
			}

			public int GetScore (int g_index) {
				return myScoreList [g_index];
			}
		}
	}
}
