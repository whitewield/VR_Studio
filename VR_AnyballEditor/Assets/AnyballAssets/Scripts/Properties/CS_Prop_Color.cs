using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyBall.Property;

namespace AnyBall {
	namespace Property {
		public class CS_Prop_Color : CS_Prop_Base {
			[SerializeField] Renderer myRenderer;
			[SerializeField] int myColorMaterialIndex;
			private Color myOriginalColor;

			protected override void Awake () {
				if (myRenderer == null) {
					myRenderer = this.GetComponent<Renderer> ();
				}
					
				if (myRenderer == null) {
					Debug.LogError ("cannot find MeshRenderer");
				}

				myOriginalColor = myRenderer.materials [myColorMaterialIndex].color;

				base.Awake ();
			}

			public virtual void SetColor (Color g_color) {
				myRenderer.materials [myColorMaterialIndex].color = g_color;
			}

			public virtual void SetColorBack () {
				myRenderer.materials [myColorMaterialIndex].color = myOriginalColor;
			}
		}
	}
}
