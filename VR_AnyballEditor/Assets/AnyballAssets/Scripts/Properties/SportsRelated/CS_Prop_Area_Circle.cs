using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using AnyBall.Property;
using AnyBall.Rule;

namespace AnyBall {
	namespace Property {
		public class CS_Prop_Area_Circle : CS_Prop_Area {
			[SerializeField] protected SpriteMask myOutLine_SpriteMask;

			protected override void Init () {
				myTriggerCollider = GetComponent<CapsuleCollider> ();

				((CapsuleCollider)myTriggerCollider).height = Constants.AREA_TRIGGER_HEIGHT;
				((CapsuleCollider)myTriggerCollider).radius = myOutLine_SpriteRenderer.transform.localScale.x * 0.5f;

				this.transform.localScale = new Vector3 (
					1 / myOutLine_SpriteRenderer.transform.localScale.x, 
					this.transform.localScale.y / Constants.AREA_COLLIDER_HEIGHT,
					1 / myOutLine_SpriteRenderer.transform.localScale.y
				);

				myColliderTransform.localScale = new Vector3 (
					myOutLine_SpriteRenderer.transform.localScale.x, 
					Constants.AREA_COLLIDER_HEIGHT,
					myOutLine_SpriteRenderer.transform.localScale.y
				);

			}
				
			public override void SetTeamNumber (int f_teamNumber) {
				myOutLine_SpriteMask.frontSortingOrder = -f_teamNumber * 2;
				myOutLine_SpriteMask.backSortingOrder = -f_teamNumber * 2 - 1;
				myOutLine_SpriteRenderer.sortingOrder = -f_teamNumber * 2;
			}

			public override void SetProgress (float g_percent) {
				myFill_SpriteRenderer.transform.localScale = new Vector3 (
					myOutLine_SpriteRenderer.transform.localScale.x * g_percent, 
					myOutLine_SpriteRenderer.transform.localScale.y * g_percent,
					1
				);
			}
		}
	}
}
