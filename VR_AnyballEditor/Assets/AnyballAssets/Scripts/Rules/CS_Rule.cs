using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using Hang.PoppingParticlePool;

namespace AnyBall {
	namespace Rule {
		public enum ScoreType {
			Duration = 0,
			Trigger = 1,
			Keeping = 2
		}

		[System.Serializable]
		public struct RuleInfo {
			public bool isTeamBased;
			public ScoreType myScoreType;
		}

		public class CS_Rule : MonoBehaviour {
			[System.Serializable]
			public struct SpecialGoalSize {
				public GameObject prefab;
				public float size;
			}

			protected bool isInitialized = false;

			[SerializeField] protected RuleInfo myRuleInfo;

			protected CS_RuleSet myRuleSet;

			protected List<bool> isActive = new List<bool> (); // used for sequence to active and inactive
			protected List<bool> isOn = new List<bool> (); // used for combine

			// Use this for initialization
			protected virtual void Start () {

			}

			// Update is called once per frame
			protected virtual void Update () {

			}

			/// <summary>
			/// Init the rule. create all objects in this rule
			/// </summary>
			/// <param name="g_ruleSet">rule set.</param>
			public virtual bool Init (CS_RuleSet g_ruleSet) {
				if (isInitialized) {
					return false;
				}

				isInitialized = true;

				myRuleSet = g_ruleSet;
				for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
					isOn.Add (false);
				}

				for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
					isActive.Add (false);
				}

				return true;
			}

			/// <summary>
			/// Acitve the specified team-based object.
			/// </summary>
			/// <param name="g_index">the team number.</param>
			public virtual void Active (int g_index) {
				// TODO: set this team's team-based object active to true
			}

			/// <summary>
			/// Inacitve the specified team-based object.
			/// </summary>
			/// <param name="g_index">the team number.</param>
			public virtual void Inactive (int g_index) {
				// TODO: set this team's team-based object active to false
			}

			public virtual void ActiveAll () {
				// TODO: set all team-based object active to true
				for (int i = 0; i < isActive.Count ;i++) {
					Active (i);
				}
			}

			/// <summary>
			/// Inactives all the team-based objects in for this rule.
			/// </summary>
			public virtual void InactiveAll () {
				// TODO: set all team-based object active to false
				for (int i = 0; i < isActive.Count ;i++) {
					Inactive (i);
				}
			}

			public virtual void Enter (GameObject g_ball, GameObject g_goal) {

			}

			public virtual void Exit (GameObject g_ball, GameObject g_goal) {

			}

			public bool CheckGoal (int g_index) {
				return myRuleSet.CheckGoal (this, g_index);
			}

			public void Goal ( int g_index) {
				// TODO: before call this function, need to regenerate the ball

				myRuleSet.Goal (g_index);
			}

			public RuleInfo GetInfo () {
				return myRuleInfo;
			}

			public bool GetIsOn (int g_index) {
				if (myRuleInfo.myScoreType != ScoreType.Duration)
					return false;
				return isOn [g_index];
			}
		}
	}
}
