using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Wield/PlayerSettings", order = 1)]
public class SO_PlayerSettings : ScriptableObject {
	public float myMoveForce = 5;
	public float myMoveDrag = 5;
	public float myRotateForce = 10;
	public float myAirDrag = 0.01f;
	public float myJumpImpulse = 5;
	public float myJumpChargeTime = 0.1f;
	public float myJumpChargeImpulse = 5;
	public float myDashImpulse = 100;
	public float myDashTime = 0.2f;
	public float myAirControlRatio = 0.3f;
	public float myPickUpRadius = 0.5f;
	public float myThrowChargeTime = 0.3f;
	public float myThrowMaxYImpulse = 5f;
	public float myThrowXZImpulseMultiplier = 5f;
	public float myKnockedOutSqrImpulse = 100f;
	public float myKnockedOutTime = 3f;

	public float myLeftStickThreshold = 0.01f;
	public float myRightStickThreshold = 0.3f;


	public float myArmLerpSpeed = 10;
}