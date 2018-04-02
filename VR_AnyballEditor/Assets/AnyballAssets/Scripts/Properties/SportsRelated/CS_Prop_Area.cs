using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using AnyBall.Property;
using AnyBall.Rule;

namespace AnyBall {
	namespace Property {
		public class CS_Prop_Area : CS_Prop_Base {
			private List<CS_Rule> myRules = new List<CS_Rule> ();

			protected Collider myTriggerCollider;
			[SerializeField] protected Transform myColliderTransform;

			[SerializeField] protected SpriteRenderer myOutLine_SpriteRenderer;
			[SerializeField] protected SpriteRenderer myFill_SpriteRenderer;
			[Range (0,1)]
			[SerializeField] protected float myFill_Alpha = 0.5f;

			protected override void Start () {
				Init ();

				base.Start ();
			}

			protected override void Update () {
				myColliderTransform.rotation = 
					myColliderTransform.rotation * Quaternion.FromToRotation (this.transform.up, Vector3.up);
				this.transform.localPosition = Vector3.zero;
				this.transform.rotation = myColliderTransform.rotation;
			}

			protected virtual void Init () {
				myTriggerCollider = GetComponent<BoxCollider> ();

				((BoxCollider)myTriggerCollider).size = new Vector3 (
					myOutLine_SpriteRenderer.size.x, 
					Constants.AREA_TRIGGER_HEIGHT / Constants.AREA_COLLIDER_HEIGHT, 
					myOutLine_SpriteRenderer.size.y
				);

				myColliderTransform.GetComponent<BoxCollider> ().size = new Vector3 (
					myOutLine_SpriteRenderer.size.x, 
					Constants.AREA_COLLIDER_HEIGHT, 
					myOutLine_SpriteRenderer.size.y
				);

			}

			public void AddRule (CS_Rule g_rule){
				myRules.Add (g_rule);
			}

			void OnTriggerEnter (Collider g_collider) {
				foreach (CS_Rule f_rule in myRules) {
					f_rule.Enter (g_collider.gameObject, this.gameObject);
				}
			}

			void OnTriggerExit (Collider g_collider) {
				foreach (CS_Rule f_rule in myRules) {
					f_rule.Exit (g_collider.gameObject, this.gameObject);
				}
			}

			public virtual void SetColor (Color g_color) {
				Color t_fillColor = g_color;
				t_fillColor.a = myFill_Alpha;
				myFill_SpriteRenderer.color = t_fillColor;
				myOutLine_SpriteRenderer.color = g_color;
			}

			public virtual void SetTeamNumber (int g_teamNumber) {
				
			}

			public virtual void SetProgress (float g_percent) {
				myFill_SpriteRenderer.transform.localScale = new Vector3 (
					myOutLine_SpriteRenderer.size.x * g_percent, 
					myOutLine_SpriteRenderer.size.y * g_percent,
					1
				);
			}
		}
	}
}
