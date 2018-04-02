using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hang.JellyJoystick;
using AnyBall.Property;
using Hang.AiryAudio;
using Global;

public class CS_PlayerControl : MonoBehaviour {
	[System.Serializable]
	public class ArmLerpSetup {
		
		[SerializeField] Transform myTransform;
		[SerializeField] int myDirection;
		[SerializeField] Transform myEndTransform;
		[SerializeField] float myEndPositionRatio = 1;
		private Quaternion myRotation;
		private Vector3 myEndLocalPosition;

		public void InitEndLocalPosition () {
			myEndLocalPosition = myEndTransform.localPosition;
		}

		public void SetEndBack () {
			myEndTransform.localPosition = myEndLocalPosition;
		}

		public void LerpTo (Vector3 g_position, float g_time) {

			// rotation

			if (myRotation != null)
				myTransform.localRotation = myRotation;

			myTransform.right = Vector3.Lerp (
				myTransform.right, 
				(g_position - myTransform.position) * myDirection,
				g_time
			);

			myRotation = myTransform.localRotation;

			// position

			if (myEndLocalPosition != null) {
				myEndTransform.position = 
					(g_position - myTransform.position) * myEndPositionRatio + myTransform.position;

//				myEndTransform.position = Vector3.Lerp (
//					myEndTransform.position, 
//					(g_position - myTransform.position) * myEndPositionRatio + myTransform.position, 
//					g_time
//				);
			}

		}
	}

    private Rigidbody myRigidbody;
    [SerializeField] CapsuleCollider myCapsuleCollider;
    [SerializeField] SO_PlayerSettings myPlayerSettings;

    [SerializeField] Renderer myMeshRenderer;
    [SerializeField] int myColorMaterialIndex;

    [SerializeField] ParticleSystem myParticleDash;

    [SerializeField] Animator myAnimator;
    private PlayerAnimationState myAnimationState = PlayerAnimationState.Idle;
    [SerializeField] float myAnimationSpeedModifier = 0.015f;

    private bool doDash = false;
    private bool isOnGround = false;
    private float myDashEndTime;

    private bool onKnockedOut = false;
    private float myKnockedOutEndTime;

    //	[SerializeField] SpriteRenderer mySpriteRenderer;
    private CS_Prop_Wearable myWearing = null;
    private SpringJoint myWearingJoint;

    private CS_Prop_Pickup myPickup = null;
    private SpringJoint myPickupJoint;
    private bool onPickup = false;
    private bool onThrowCharge = false;
    private float myThrowChargeTimer = 0;

    private bool onJumpCharge = false;
    private float myJumpChargeTimer = 0;

    [SerializeField] int myJoystickNumber = 1;
    [SerializeField] int myTeamNumber = 0;

    private Vector2 myMovingDirection = Vector2.up;
    private Vector2 myLastMovingDirection = Vector2.up;

    // hand attatching to object
	[SerializeField] List<ArmLerpSetup> myArms = new List<ArmLerpSetup>();

	void Awake () {
		DontDestroyOnLoad (this.gameObject);
		myRigidbody = this.GetComponent<Rigidbody> ();

		for (int i = 0; i < myArms.Count; i++) {
			myArms [i].InitEndLocalPosition ();
		}
	}

	public void Init (int g_index) {
		myJoystickNumber = g_index + 1;
		myTeamNumber = g_index % Global.Constants.NUMBER_MAX_TEAM;
		SetColor (CS_PlayerManager.Instance.GetTeamColor (myTeamNumber));
	}

	// Update is called once per frame
	void Update () {
		//this.transform.localScale = new Vector3 (1, 1 - (myJumpChargeTimer / myPlayerSettings.myJumpChargeTime) * 0.5f, 1);

		UpdateOnGround ();

		if (onKnockedOut) {
			UpdateKnockedOut ();
			return;
		}

		//get the move vecter from input left stick
		myMovingDirection = new Vector2 (
            JellyJoystickManager.Instance.GetAxis (AxisMethodName.Normal, myJoystickNumber, JoystickAxis.LS_X), 
            JellyJoystickManager.Instance.GetAxis (AxisMethodName.Normal, myJoystickNumber, JoystickAxis.LS_Y)
		);

		if (JellyJoystickManager.Instance.GetButton (ButtonMethodName.Down, myJoystickNumber, JoystickButton.B)) {
			PickUp ();
		}

		if (JellyJoystickManager.Instance.GetButton (ButtonMethodName.Down, myJoystickNumber, JoystickButton.B)) {
			ThrowDone ();
		}

		if (JellyJoystickManager.Instance.GetButton (ButtonMethodName.Down, myJoystickNumber, JoystickButton.A))
			JumpCharge ();
		
		if (isOnGround) {
			if (JellyJoystickManager.Instance.GetButton (ButtonMethodName.Down, myJoystickNumber, JoystickButton.Y)) {
				Dash ();
			}
			if (JellyJoystickManager.Instance.GetButton (ButtonMethodName.Up, myJoystickNumber, JoystickButton.A))
				Jump ();
		} else {
			if (JellyJoystickManager.Instance.GetButton (ButtonMethodName.Up, myJoystickNumber, JoystickButton.A))
				JumpChargeCancel ();
		}


		if (myPickup == null && myPickupJoint != null)
			Destroy (myPickupJoint);
		if (myWearing == null && myWearingJoint != null)
			Destroy (myWearingJoint);
	}

	void FixedUpdate () {

        // TEMP hand attatching to object - - - - - - - - - -
        //if(myPickup!=null) {
        //    Debug.Log("holding something");
        //    leftHand.transform.position = myPickup.transform.position;
        //    rightHand.transform.position = myPickup.transform.position;
        //}
        // TEMP hand attatching to object - - - - - - - - - -

		UpdateGroundFriction ();

		if (onKnockedOut) {
			return;
		}

		UpdateThrow ();

		if (isOnGround)
			UpdateJump ();

		if (onJumpCharge == false)
			UpdateMove ();
		UpdateRotation ();
	}

	void LateUpdate () {
		

		//ANIMATION
		if (onJumpCharge){
			myAnimationState = PlayerAnimationState.Crouching;
		} else if (isOnGround) {
			if (myMovingDirection.sqrMagnitude > 0f) {
				myAnimationState = PlayerAnimationState.Running;
			} else {
				myAnimationState = PlayerAnimationState.Idle;
			}

		} else {
			myAnimationState = PlayerAnimationState.Flying;
		}
		myAnimator.SetInteger("state", (int)myAnimationState);
		myAnimator.SetFloat("yVelocity", myRigidbody.velocity.y);

		if (myAnimationState == PlayerAnimationState.Running) {
			myAnimator.speed = myRigidbody.velocity.magnitude * myAnimationSpeedModifier;
		} else {
			myAnimator.speed = 1f;
		}

		myAnimator.Update (Time.deltaTime);

		//arm

		if (myPickup != null) {
//			myRShoulder.transform.right = (myPickup.transform.position - myRShoulder.transform.position) * -1;
//			myLShoulder.transform.right = (myPickup.transform.position - myLShoulder.transform.position);

			float t_lerpTime = Time.deltaTime * myPlayerSettings.myArmLerpSpeed;

			for (int i = 0; i < myArms.Count; i++) {
				myArms [i].LerpTo (myPickup.transform.TransformPoint (myPickupJoint.connectedAnchor), t_lerpTime);
//				Debug.Log ("Arm" + i);
			}

//			myRShoulder.rotation = myRShoulder_Rotation;
//			myLShoulder.rotation = myLShoulder_Rotation;
//			myRElbow.rotation = myRElbow_Rotation;
//			myLElbow.rotation = myLElbow_Rotation;
//
//			myRShoulder.right = Vector3.Lerp (
//				myRShoulder.right, 
//				(myPickup.transform.position - myRShoulder.position) * -1,
//				t_lerpTime
//			);
//
//			myLShoulder.right = Vector3.Lerp (
//				myLShoulder.right, 
//				(myPickup.transform.position - myLShoulder.position), 
//				t_lerpTime
//			);
//
//			myRElbow.right = Vector3.Lerp (
//				myRElbow.right, 
//				(myPickup.transform.position - myRShoulder.position) * -1, 
//				t_lerpTime
//			);
//
//			myLElbow.right = Vector3.Lerp (
//				myLElbow.right, 
//				(myPickup.transform.position - myLShoulder.position), 
//				t_lerpTime
//			);
//
//			myRShoulder_Rotation = myRShoulder.rotation;
//			myLShoulder_Rotation = myLShoulder.rotation;
//			myRElbow_Rotation = myRElbow.rotation;
//			myLElbow_Rotation = myLElbow.rotation;

		} else {
			for (int i = 0; i < myArms.Count; i++) {
				myArms [i].SetEndBack ();
			}
		}

	}

	private void UpdateOnGround () {
		// raycast for the on ground check
		float t_onGroundDistance = myCapsuleCollider.height / 2 + 0.1f;
		Ray ray = new Ray (this.transform.position, Vector3.down);
		RaycastHit hit;
		//			int t_layerMask = (int) Mathf.Pow (2, 8); //for the layer you want to do the raycast
		//			if (Physics.Raycast (ray, out hit, myJumpDistance, t_layerMask))
		if (Physics.Raycast (ray, out hit, t_onGroundDistance))
			isOnGround = true;
		else
			isOnGround = false;
	}

	private void UpdateGroundFriction () {
		myRigidbody.AddForce (myRigidbody.velocity.normalized * myRigidbody.velocity.sqrMagnitude * myPlayerSettings.myAirDrag * -1);
		if (isOnGround) {
			//add drag
			myRigidbody.AddForce (myRigidbody.velocity.normalized * myRigidbody.velocity.magnitude * myPlayerSettings.myMoveDrag * -1);
		}
	}

	private void UpdateMove () {
		if (myMovingDirection.sqrMagnitude > myPlayerSettings.myLeftStickThreshold) {
			myLastMovingDirection = myMovingDirection;
		}

        if(myMovingDirection.sqrMagnitude > 1){
            myMovingDirection = myMovingDirection.normalized;
        }

        //Vector3 t_moveVecter = myMovingDirection.normalized * myPlayerSettings.myMoveForce * myRigidbody.mass;
        Vector3 t_moveVecter = myMovingDirection * myPlayerSettings.myMoveForce * myRigidbody.mass;

//        Debug.Log(t_moveVecter.magnitude);

		UpdateDash ();

		if (!isOnGround) {
			t_moveVecter *= myPlayerSettings.myAirControlRatio;
		}

		//add move force
		myRigidbody.AddForce (t_moveVecter.x, 0, t_moveVecter.y);

	}

	private void UpdateRotation () {
		//get the face vecter from input right stick
//		Vector2 t_faceVecter = new Vector2 (
//			JellyJoystickManager.Instance.GetAxis (AxisMethodName.Raw, myJoystickNumber, JoystickAxis.RS_X), 
//			JellyJoystickManager.Instance.GetAxis (AxisMethodName.Raw, myJoystickNumber, JoystickAxis.RS_Y)
//		);
//
//		if (t_faceVecter.sqrMagnitude > myPlayerSettings.myRightStickThreshold) {
//			this.transform.LookAt (new Vector3 (t_faceVecter.x, 0, t_faceVecter.y) + this.transform.position);
//
//			if (myPickup != null) {
//				SpringJoint t_springJoint = this.gameObject.GetComponent<SpringJoint> ();
//				t_springJoint.anchor = Vector3.forward;
//			}
//		} else if (myRigidbody.velocity.sqrMagnitude > 0) {
//			this.transform.LookAt (new Vector3 (myRigidbody.velocity.x, 0, myRigidbody.velocity.z) + this.transform.position);
//
//			if (myPickup != null) {
//				SpringJoint t_springJoint = this.gameObject.GetComponent<SpringJoint> ();
//				t_springJoint.anchor = Vector3.up;
//			}
//		}
		if (myPickup != null) {
			if (myMovingDirection.sqrMagnitude > myPlayerSettings.myRightStickThreshold) {
				myPickupJoint.anchor = Vector3.forward;
			} else {
				myPickupJoint.anchor = Vector3.up;
			}
		}

		myRigidbody.angularVelocity = Vector3.zero;
		this.transform.forward = Vector3.Lerp (
			new Vector3 (this.transform.forward.x, 0, this.transform.forward.z), 
			new Vector3 (myLastMovingDirection.x, 0, myLastMovingDirection.y), 
			myPlayerSettings.myRotateForce * Time.fixedDeltaTime
		);
	}

	private void UpdateKnockedOut () {
		if (Time.timeSinceLevelLoad > myKnockedOutEndTime) {
			onKnockedOut = false;
			return;
		}
	}


	private void Dash () {
		if (!doDash) {
			doDash = true;
			myDashEndTime = Time.timeSinceLevelLoad + myPlayerSettings.myDashTime;
			myRigidbody.AddForce (myLastMovingDirection.x * myPlayerSettings.myDashImpulse * myRigidbody.mass,
				0,
				myLastMovingDirection.y * myPlayerSettings.myDashImpulse * myRigidbody.mass, 
				ForceMode.Impulse
			);
			//play particle
			ParticleSystem t_particle =
				PoppingParticlePoolManager.Instance.GetFromPool(Hang.PoppingParticlePool.ParticleType.Dust);
			t_particle.transform.position = transform.position;
			t_particle.Play();
			//play sound
            AiryAudioManager.Instance.GetAudioData("DashSounds").Play(this.transform.position);

			myParticleDash.Play ();
		}
	}

	public bool GetOnDash () {
		return doDash;
	}

	private void UpdateDash () {
		if (doDash && Time.timeSinceLevelLoad > myDashEndTime) {
			doDash = false;
			myParticleDash.Stop ();
			return;
		}
	}

	private void JumpCharge () {
		onJumpCharge = true;

		myJumpChargeTimer = 0;

//		Debug.Log ("JumpCharge");
//		myRigidbody.AddForce (0, myPlayerSettings.myJumpImpulse * myRigidbody.mass, 0, ForceMode.Impulse);
	}

	private void JumpChargeCancel () {
		onJumpCharge = false;
		myJumpChargeTimer = 0;

//		Debug.Log ("JumpChargeCancel");
		//		myRigidbody.AddForce (0, myPlayerSettings.myJumpImpulse * myRigidbody.mass, 0, ForceMode.Impulse);
	}

	private void UpdateJump () {
//		Debug.Log ("UpdateJump: " + myJumpChargeTimer);
		if (onJumpCharge == false)
			return;

		if (onKnockedOut) {
			onJumpCharge = false;
			return;
		}

		myJumpChargeTimer += Time.fixedDeltaTime;
		if (myJumpChargeTimer > myPlayerSettings.myJumpChargeTime) {
			myJumpChargeTimer = myPlayerSettings.myJumpChargeTime;
//			Jump ();
		}
	}

	private void Jump () {
		Debug.Log ("Jump");
		if (onJumpCharge == false)
			return;
		onJumpCharge = false;
		float t_chargeImpulse = myPlayerSettings.myJumpChargeImpulse * (myJumpChargeTimer / myPlayerSettings.myJumpChargeTime);
		myRigidbody.AddForce (
			0, 
			myPlayerSettings.myJumpImpulse + t_chargeImpulse,
			0, 
			ForceMode.Impulse
		);
		myJumpChargeTimer = 0;
	}
		
	private void PickUp () {
		if (myPickup == null) {
			//pick up

			int t_layerMask = (int)Mathf.Pow (2, 8) + (int)Mathf.Pow (2, 10);
			Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, myPlayerSettings.myPickUpRadius, t_layerMask);
			float t_minSqrDistance = myPlayerSettings.myPickUpRadius;
			Collider t_closestBallCollider = null;
			for (int i = 0; i < hitColliders.Length; i++) {
				if (hitColliders [i].gameObject == this.gameObject ||
				    (hitColliders [i].GetComponent<CS_Prop_Pickup> () == null &&
				    hitColliders [i].GetComponent<CS_Prop_Button> () == null &&
				    hitColliders [i].GetComponent<CS_Prop_Wearable> () == null))
					continue;
				float t_sqrDistance = Vector3.SqrMagnitude (this.transform.position - hitColliders [i].ClosestPoint (this.transform.position));
				if (t_sqrDistance < t_minSqrDistance) {
					t_minSqrDistance = t_sqrDistance;
					t_closestBallCollider = hitColliders [i];
				}
			}

			if (t_closestBallCollider != null) {
				Rigidbody t_closestBallRigidbody = t_closestBallCollider.GetComponent<Rigidbody> ();

//				if (!(t_closestBallCollider.GetComponent <CS_Prop_Heavy> ())) {
//					t_closestBallRigidbody.useGravity = false;
//				} else {
//					t_springjoin.spring = 100000;
//				}

//				t_closestBallRigidbody.drag = 2;
				GameObject t_object = t_closestBallCollider.gameObject;

				CS_Prop_Button t_button = t_object.GetComponent<CS_Prop_Button> ();
				if (t_button != null) {
					Debug.Log ("on button");
					CS_GameManager.Instance.OnButton (t_button.MyButtonType);
				}

				CS_Prop_Pickup t_pickUp = t_object.GetComponent <CS_Prop_Pickup> ();
				if (t_pickUp != null) {
					myPickup = t_pickUp;

					Vector3 t_connectedAnchor = Vector3.zero;
					if (t_pickUp.IsBig) {
						t_connectedAnchor = t_object.transform.InverseTransformPoint (
							t_closestBallCollider.ClosestPointOnBounds (this.transform.position)
						);
					}

					myPickupJoint = CreateJoint (
						t_closestBallRigidbody, 
						Vector3.up,
						t_connectedAnchor
					);

					myPickup.OnHold = true;
					myPickup.MyHolder = this.gameObject;

					onPickup = true;

                    // play sound
                    AiryAudioManager.Instance.GetAudioData("GrabSounds").Play(this.transform.position);

				}
					
				CS_Prop_Wearable t_wear = t_object.GetComponent <CS_Prop_Wearable> ();
				if (t_wear != null) {
					if (myWearing != null) {
						DropWearing ();
					}
					myWearing = t_wear;
					myWearingJoint = CreateJoint (t_closestBallRigidbody, Vector3.up * 1.5f, Vector3.zero);
					myWearing.SetMyHolder (this);

					Debug.Log ("Wear!");
                    // play sound
                    AiryAudioManager.Instance.GetAudioData("GrabSounds").Play(this.transform.position);
				}

//				mySpriteRenderer.sprite = myPlayerSettings.myPickUpSprite;
			}
		} else {
			//throw
			onThrowCharge = true;
			myThrowChargeTimer = 0;
			//			mySpriteRenderer.sprite = myPlayerSettings.myNormalSprite;

            // play sound
            AiryAudioManager.Instance.GetAudioData("DropSounds").Play(this.transform.position);
		}
	}

	private void UpdateThrow () {
		if (!onThrowCharge)
			return;

		myThrowChargeTimer += Time.fixedDeltaTime;
        if (myThrowChargeTimer > myPlayerSettings.myThrowChargeTime) {
            //ThrowDone();

            myThrowChargeTimer = myPlayerSettings.myThrowChargeTime;
        }
	}

	private void ThrowDone () {
		onThrowCharge = false;
		if (myPickup == null)
			return;
		if (onPickup == true)
			onPickup = false;
		else {
//			Vector3 t_yImpulse = myPlayerSettings.myThrowMaxYImpulse * myThrowChargeTimer / myPlayerSettings.myThrowChargeTime * Vector3.up;
//			Vector3 t_xzImpulse = myPlayerSettings.myThrowXZImpulseMultiplier * Vector3.ProjectOnPlane (myRigidbody.velocity, Vector3.up);
//			myPickup.GetComponent<Rigidbody> ().AddForce (
//				t_yImpulse + t_xzImpulse,
//				ForceMode.Impulse);

			Vector3 t_yImpulse = Vector3.zero;
			if (isOnGround) {
				t_yImpulse = myPlayerSettings.myThrowMaxYImpulse * Vector3.ProjectOnPlane (myRigidbody.velocity, Vector3.up).normalized;
			} else {
				t_yImpulse = myPlayerSettings.myThrowMaxYImpulse * Vector3.up;
			}
			Vector3 t_xzImpulse = myPlayerSettings.myThrowXZImpulseMultiplier * Vector3.ProjectOnPlane (myRigidbody.velocity, Vector3.up).normalized;
			myPickup.GetComponent<Rigidbody> ().AddForce (
				t_yImpulse + t_xzImpulse,
				ForceMode.Impulse);
			DropItem ();
		}
	}

	private void DropItem () {
		if (myPickup != null) {
			RemoveJoint (myPickupJoint);
			myPickupJoint = null;
//			Rigidbody t_pickupRigidbody = myPickup.GetComponent<Rigidbody> ();
//
//			if (!(t_pickupRigidbody.GetComponent <CS_Prop_Heavy> ())) {
//				t_pickupRigidbody.useGravity = true;
//			}

			myPickup.OnHold = false;
			myPickup.MyHolder = null;

//			t_pickupRigidbody.drag = 0;
			myPickup = null;
		}
	}

	public void DropWearing () {
		if (myWearing != null) {
			RemoveJoint (myWearingJoint);
			myWearingJoint = null;
			myWearing.Drop (this);

			myWearing = null;
		}


	}

	void OnCollisionEnter (Collision g_collision) {

		if (onKnockedOut)
			return;

//		Debug.Log (g_collision.gameObject.name + ": " + g_collision.impulse.sqrMagnitude);
		Rigidbody t_colRigidbody = g_collision.gameObject.GetComponent<Rigidbody> ();
		if (t_colRigidbody == null)
			return;

		CS_Prop_Pickup t_pickUp = g_collision.gameObject.GetComponent<CS_Prop_Pickup> ();
		CS_Prop_Knockout t_knockout = g_collision.gameObject.GetComponent<CS_Prop_Knockout> ();
		if (t_pickUp != null && t_knockout != null) {
			//is a pick up
			if (t_pickUp.MyHolder != null && 
				t_pickUp.MyHolder != this.gameObject) {
				//someone hit me with the object

				Debug.Log ("someone hit me with the object");

				float t_sqrImpulse = g_collision.impulse.sqrMagnitude;

				if (t_sqrImpulse * t_knockout.GetSqrImpulseScale () >
				    myPlayerSettings.myKnockedOutSqrImpulse) {
					Debug.Log ("KnockOut");
					Debug.LogWarning (g_collision.gameObject.name + ": " + t_sqrImpulse * t_knockout.GetSqrImpulseScale ());

					switch (t_knockout.GetKnockoutType ()) {
					case KnockoutType.Drop:
						KnockedOut_Drop (g_collision);
						break;
					case KnockoutType.Stun:
						KnockedOut_Stun (g_collision);
						break;
					}
				}

				if (t_pickUp.OnHold == false) {
					t_pickUp.MyHolder = null;
				}
			}
		}

		CS_PlayerControl t_player = g_collision.gameObject.GetComponent<CS_PlayerControl> ();
		if (t_player != null) {
			//is a player

			if (t_player.GetOnDash ()) {
				KnockedOut_Drop (g_collision);
			}
		}
	}

	private void KnockedOut_Stun (Collision g_collision) {
		// play sound effect
        AiryAudioManager.Instance.GetAudioData("KnockOutSounds").Play(this.transform.position);

		onKnockedOut = true;
		myKnockedOutEndTime = Time.timeSinceLevelLoad + myPlayerSettings.myKnockedOutTime;
		DropItem ();
		DropWearing ();

		//play particle
		ParticleSystem t_particle =
			PoppingParticlePoolManager.Instance.GetFromPool(Hang.PoppingParticlePool.ParticleType.Knockout);
		t_particle.transform.position = g_collision.contacts[0].point;
		t_particle.Play();
	}

	private void KnockedOut_Drop (Collision g_collision) {
		// play sound effect
        AiryAudioManager.Instance.GetAudioData("HitSounds").Play(this.transform.position);

		Debug.Log ("DropItem");
		DropItem ();
		DropWearing ();

		//play particle
		ParticleSystem t_particle =
			PoppingParticlePoolManager.Instance.GetFromPool (Hang.PoppingParticlePool.ParticleType.Hit);
		t_particle.transform.position = g_collision.contacts [0].point;
		t_particle.Play ();
	}

	public void SetColor (Color g_color) {
		myMeshRenderer.materials [myColorMaterialIndex].color = g_color;
	}

	public int GetTeam () {
		return myTeamNumber;
	}

	public void SetTeam (int g_teamNumber) {
		myTeamNumber = g_teamNumber;
		SetColor (CS_PlayerManager.Instance.GetTeamColor (myTeamNumber));
	}

	private SpringJoint CreateJoint (
		Rigidbody g_rigidbody, 
		Vector3 g_anchor,
		Vector3 g_connectedAnchor) {

		SpringJoint t_joint = this.gameObject.AddComponent<SpringJoint> () as SpringJoint;
		t_joint.autoConfigureConnectedAnchor = false;
//        t_joint.anchor = g_anchor;
		t_joint.anchor = g_anchor;
		t_joint.connectedAnchor = g_connectedAnchor;
		t_joint.connectedBody = g_rigidbody;
		t_joint.spring = 1000;
		t_joint.enableCollision = true;
		t_joint.enablePreprocessing = false;
		return t_joint;
	}

	private void RemoveJoint (SpringJoint g_joint) {
		if (g_joint != null) {
			Destroy (g_joint);
		}
	}
}
