using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

namespace AnyBall {
	namespace Rule {
		public class CS_RuleSet {

			private List<CS_Rule> myRules; // a list of rules

			[SerializeField] RuleInfo myRuleInfo; // info of the rule set

			private CS_RuleSequence myRuleSequence;

			/// <summary>
			/// Initializes a new instance of the <see cref="AnyBall.Rule.CS_RuleSet"/> class.
			/// </summary>
			/// <param name="g_rule">the rule to initialize with.</param>
			public CS_RuleSet (CS_Rule g_rule) {
				myRules = new List<CS_Rule> ();
				myRules.Add (g_rule);

				myRuleInfo = g_rule.GetInfo ();
			}

			/// <summary>
			/// Adds the rule to rule set.
			/// </summary>
			/// <returns><c>true</c>, if rule was added, <c>false</c> otherwise.</returns>
			/// <param name="g_rule">rule you want to add.</param>
			public bool AddRule (CS_Rule g_rule) {
				if (CheckCombine (g_rule) == false)
					return false;
				
				myRules.Add (g_rule);
				SetInfo (g_rule.GetInfo ());

				//update the extendable in the sequence
				//fix : if hat + football + footbal, the 2nd football should be added to the sequence
				myRuleSequence.UpdateExtendable ();

				return true;
			}


			public bool CheckCombine (CS_Rule g_rule) {
				//cannot combine the same rule
				foreach (CS_Rule f_rule in myRules) {
					if (f_rule.gameObject.name == g_rule.gameObject.name) {
						return false;
					}
				}

				if (myRuleInfo.myScoreType == ScoreType.Keeping || g_rule.GetInfo().myScoreType == ScoreType.Keeping)
					return false;

				if (myRuleInfo.myScoreType == ScoreType.Trigger && g_rule.GetInfo ().myScoreType == ScoreType.Trigger)
					return false;

				return true;
			}

			public void Init (CS_RuleSequence g_sequence) {
				// TODO: init all rules in this rule set
				myRuleSequence = g_sequence;
				foreach (CS_Rule f_rule in myRules) {
					f_rule.Init (this);
				}
			}

			public bool CheckGoal (CS_Rule g_rule, int g_index) {
				foreach (CS_Rule f_rule in myRules) {
					if (g_rule == f_rule)
						continue;

					if (f_rule.GetIsOn (g_index) == false) {
						return false;
					}
				}
				return true;
			}

			public void Goal (int g_index) {
				myRuleSequence.Goal (g_index);
			}

			public void Active (int g_index) {
				foreach (CS_Rule f_rule in myRules) {
					f_rule.Active (g_index);
				}
			}

			public void Inactive (int g_index) {
				foreach (CS_Rule f_rule in myRules) {
					f_rule.Inactive (g_index);
				}
			}

			/// <summary>
			/// Inactives all the team-based objects in for each rule.
			/// </summary>
			public void InactiveAll () {
				foreach (CS_Rule f_rule in myRules) {
					f_rule.InactiveAll ();
				}
			}

			/// <summary>
			/// Actives all the team-based objects in for each rule.
			/// </summary>
			public void ActiveAll () {
				foreach (CS_Rule f_rule in myRules) {
					f_rule.ActiveAll ();
				}
			}

			/// <summary>
			/// Sets the info from the new rule.
			/// </summary>
			/// <param name="g_info">new rule info.</param>
			private void SetInfo (RuleInfo g_info) {
				myRuleInfo.isTeamBased = myRuleInfo.isTeamBased || g_info.isTeamBased;
				myRuleInfo.myScoreType = 
					(ScoreType)Mathf.Max ((int)myRuleInfo.myScoreType, (int)g_info.myScoreType);
			}

			/// <summary>
			/// Gets the info.
			/// </summary>
			/// <returns>The info.</returns>
			public RuleInfo GetInfo () {
				return myRuleInfo;
			}
		}
	}
}
