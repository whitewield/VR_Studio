using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyBall {
	namespace Editor {

		public class CS_AnyLevelObject_Invisible : CS_AnyLevelObject {

			private Renderer[] myRenderers;

			void Awake () {
				myCategory = Category.Invisible;

				myRenderers = this.GetComponentsInChildren<Renderer> ();

			}

			public virtual void Hide () {
				foreach (Renderer t_renderer in myRenderers) {
					t_renderer.enabled = false;
				}
			}

			public virtual void Show () {
				foreach (Renderer t_renderer in myRenderers) {
					t_renderer.enabled = true;
				}
			}
		}
	}
}
