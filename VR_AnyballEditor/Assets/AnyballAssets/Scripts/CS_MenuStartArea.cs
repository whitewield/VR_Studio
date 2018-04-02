using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class CS_MenuStartArea : MonoBehaviour {

	protected Collider myTriggerCollider;

	[SerializeField] protected SpriteRenderer myOutLine_SpriteRenderer;
	[SerializeField] protected SpriteRenderer myFill_SpriteRenderer;

	private int myPlayerAmount = 0;

	[SerializeField] float myGameStartTime = 5;
	private float myTimer = 0;

	void Start () {
		Init ();
	}

	protected virtual void Init () {
		myTriggerCollider = GetComponent<BoxCollider> ();

		((BoxCollider)myTriggerCollider).size = new Vector3 (
			myOutLine_SpriteRenderer.size.x, 
			Constants.AREA_TRIGGER_HEIGHT, 
			myOutLine_SpriteRenderer.size.y
		);

	}

	void Update () {
		if (myPlayerAmount > 1) {
			myTimer += Time.deltaTime;
			if (myTimer > myGameStartTime) {
				// game start

				myTimer = myGameStartTime;

				CS_MenuManager.Instance.OnButtonStart ();
			}
		}

		// update display

		SetProgress (myTimer / myGameStartTime);
	}

	void OnTriggerEnter (Collider g_collider) {
		if (g_collider.GetComponent<CS_PlayerControl> ()) {
			myPlayerAmount++;
			myTimer = 0;
			CS_PlayerManager.Instance.AddPlayerInUse (g_collider.gameObject);
		}
	}

	void OnTriggerExit (Collider g_collider) {
		if (g_collider.GetComponent<CS_PlayerControl> ()) {
			myPlayerAmount--;
			myTimer = 0;
			CS_PlayerManager.Instance.RemovePlayerInUse (g_collider.gameObject);
		}
	}

	private void SetProgress (float g_percent) {
		myFill_SpriteRenderer.transform.localScale = new Vector3 (
			myOutLine_SpriteRenderer.size.x * g_percent, 
			myOutLine_SpriteRenderer.size.y * g_percent,
			1
		);
	}
}
