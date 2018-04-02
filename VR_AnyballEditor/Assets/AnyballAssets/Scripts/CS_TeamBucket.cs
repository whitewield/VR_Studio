using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Property;

public class CS_TeamBucket : MonoBehaviour {
	[SerializeField] int myTeamNumber;
	// Use this for initialization
	void Start () {
		this.GetComponent<CS_Prop_Color> ().SetColor (CS_PlayerManager.Instance.GetTeamColor (myTeamNumber));
	}
	
//	// Update is called once per frame
//	void Update () {
//		
//	}

	void OnTriggerEnter (Collider g_collider) {
		CS_PlayerControl t_playerControl = g_collider.gameObject.GetComponent<CS_PlayerControl> ();
		if (t_playerControl != null) {
			t_playerControl.SetTeam (myTeamNumber);
		}

		CS_PlayerManager.Instance.CheckTeam ();
	}
}
