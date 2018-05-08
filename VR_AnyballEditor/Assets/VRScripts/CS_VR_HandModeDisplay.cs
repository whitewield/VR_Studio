using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_VR_HandModeDisplay : MonoBehaviour {

	[SerializeField] SpriteRenderer mySpriteRenderer;

	void Awake () {
		if (mySpriteRenderer == null)
			mySpriteRenderer = this.GetComponentInChildren<SpriteRenderer> ();
	}

	public void ChangeSprite () {
		mySpriteRenderer.sprite = null;
	}

	public void ChangeSprite (Sprite g_sprite) {
		mySpriteRenderer.sprite = g_sprite;
	}
}
