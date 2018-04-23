using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CS_VR_Global {
	public static Vector3 LocalVector3 (Vector3 g_original, Transform g_localTransform) {
		return new Vector3 (
			Mathf.Abs (Vector3.Dot (g_original, g_localTransform.right)),
			Mathf.Abs (Vector3.Dot (g_original, g_localTransform.up)),
			Mathf.Abs (Vector3.Dot (g_original, g_localTransform.forward))
		);

	}
}
