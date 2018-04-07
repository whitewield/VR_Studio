using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_CheckArea : MonoBehaviour {

//	LineRenderer lineRend;
	[SerializeField] float rectangleX =5f;
	[SerializeField] float rectangleZ =10f;
	[SerializeField] float width = 0.3f;

	[SerializeField] GameObject linePrefab;
	List<GameObject> lines = new List<GameObject>(); 

	BoxCollider triggerCol;
	[SerializeField] BoxCollider col;



	void Awake () {
//		lineRend = GetComponent<LineRenderer> ();
		triggerCol = GetComponent<BoxCollider> ();

		triggerCol.size = new Vector3 (rectangleX, 100f, rectangleZ);
		col.size = new Vector3 (rectangleX, 0.05f, rectangleZ);

		for (int i = 0; i < 4; i++) {
			lines.Add(Instantiate(linePrefab, this.transform));
		}

		lines [0].transform.localPosition = Vector3.left * rectangleX * 0.5f;
		lines [1].transform.localPosition = Vector3.right * rectangleX * 0.5f;
		lines [2].transform.localPosition = Vector3.forward * rectangleZ * 0.5f;
		lines [3].transform.localPosition = Vector3.back * rectangleZ * 0.5f;

		lines [0].transform.localScale = new Vector3 (width, 1, rectangleZ + width);
		lines [1].transform.localScale = new Vector3 (width, 1, rectangleZ + width);
		lines [2].transform.localScale = new Vector3 (rectangleX + width, 1, width);
		lines [3].transform.localScale = new Vector3 (rectangleX + width, 1, width);

	}

	void Start () {
		

	}

	public void SetColor(Color g_col) {
		foreach (GameObject f_line in lines) {
			f_line.GetComponentInChildren<MeshRenderer> ().material.color = g_col;
		}
	}


}
