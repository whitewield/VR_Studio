using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyBall {
	namespace Editor {

		public class CS_AnyLevelObject_SpawnArea : CS_AnyLevelObject_Invisible {

			private bool showShadow = true;
			private SpriteRenderer mySpriteRenderer;
			private GameObject myShadow;

			protected override void Start () {
				base.Start ();
				mySpriteRenderer = this.GetComponentInChildren<SpriteRenderer> ();
				CreateShadow ();
			}

			public override void Hide () {
				base.Hide ();
				showShadow = false;
			}

			public override void Show () {
				base.Show ();
				showShadow = true;
			}

			public void CreateShadow () {
				if (myShadow != null)
					return;

				myShadow = new GameObject (this.gameObject.name + "_Shadow");
				myShadow.transform.SetParent (this.transform);
				myShadow.transform.rotation = mySpriteRenderer.transform.rotation;
				myShadow.transform.localPosition = Vector3.zero;
				myShadow.transform.localScale = Vector3.one;

				SpriteRenderer t_shadowSpriteRenderer = myShadow.AddComponent<SpriteRenderer> ();
				t_shadowSpriteRenderer.sprite = mySpriteRenderer.sprite;
				t_shadowSpriteRenderer.color = new Color (0, 0, 0, 0.1f);

				if (showShadow == false) {
					myShadow.SetActive (false);
				}
			}

			void LateUpdate () {
				this.transform.position = new Vector3 (this.transform.position.x, 30, this.transform.position.z);
				this.transform.localScale = new Vector3 (this.transform.localScale.x, 1, this.transform.localScale.x);

				if (myShadow != null) {
					if (showShadow == true) {
						if (myShadow.activeSelf == false)
							myShadow.SetActive (true);

						//move shadow
						float t_distance = 100;
						Ray t_ray = new Ray (this.transform.position, Vector3.down);
						RaycastHit t_hit;
						int t_layerMask = 1 << 9;
						if (Physics.Raycast (t_ray, out t_hit, t_distance, t_layerMask)) {
							myShadow.transform.position = t_hit.point + Vector3.up;
						}
					} else {
						if (myShadow.activeSelf == true)
							myShadow.SetActive (false);
					}
				}
			}



		}
	}
}
