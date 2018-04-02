using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using AnyBall.Rule;

public class CS_DigitalClock : MonoBehaviour {
		
	[SerializeField] Color myColor_Normal = Color.green;
	[SerializeField] Color myColor_Blink = Color.red;
	[SerializeField] Color myColor_Stop = Color.white;
	[SerializeField] Color myColor_Off = Color.black;
	private Color myCurrentColor = Color.magenta;

	[SerializeField] TextMesh myTextMesh_Minute;
	[SerializeField] TextMesh myTextMesh_Colon;
	[SerializeField] TextMesh myTextMesh_Second;

	private MeshRenderer myMeshRenderer;
	[SerializeField] int[] myMeshRenderer_LightNumber = { 1, 2 };

	private float myCurrentTime = 3599;
	[SerializeField] float myBlinkTime = 5f;
	[SerializeField] float myBlinkCycle = 0.5f;
	[SerializeField] float myBlinkCycle_OffTime = 0.1f;
	private float myBlinkTimer = 0;

	private void Awake(){
		myMeshRenderer = GetComponent<MeshRenderer>();
    }


    void Start () {
		SetColor (myColor_Stop);
		SetTime (0);
	}


	void Update () {
		if (CS_GameManager.Instance == null)
			return;

		float t_time = CS_GameManager.Instance.GetGameTimer ();
		GameStatus t_gameStatus = CS_GameManager.Instance.GetGameStatus ();

		if (t_gameStatus == GameStatus.Stop || t_gameStatus == GameStatus.Prepare || t_gameStatus == GameStatus.End) {
			ChangeColor (myColor_Stop);
		} else if (t_time > myBlinkTime) {
			ChangeColor (myColor_Normal);
		} else {
			myBlinkTimer -= Time.deltaTime;
			if (myBlinkTimer > myBlinkCycle_OffTime) {
				ChangeColor (myColor_Blink);
			} else {
				ChangeColor (myColor_Off);
				if (myBlinkTimer < 0) {
					myBlinkTimer += myBlinkCycle;

				}
			}
		}

		if (t_gameStatus == GameStatus.Stop) {
			ChangeTime (0);
		} else {
			ChangeTime (t_time);
		}
			
	}

	private void SetColor (Color g_color) {
		//set the light color
		foreach (int f_number in myMeshRenderer_LightNumber) {
			myMeshRenderer.materials [f_number].color = g_color;
		}

		//set text color
		myTextMesh_Minute.color = g_color;
		myTextMesh_Colon.color = g_color;
		myTextMesh_Second.color = g_color;
	}

	private void ChangeColor (Color g_color) {
		if (myCurrentColor == g_color)
			return;

		myCurrentColor = g_color;

		SetColor (g_color);
	}

	private void SetTime (float g_time) {
		int t_minute = Mathf.FloorToInt (g_time / 60f);
		int t_second = Mathf.FloorToInt (g_time % 60f);

//		Debug.Log ("Time:" + t_minute.ToString ("00") + ":" + t_second.ToString ("00"));

		myTextMesh_Minute.text = t_minute.ToString ("00");
		myTextMesh_Second.text = t_second.ToString ("00");
	}

	private void ChangeTime (float g_time) {
		if (Mathf.FloorToInt (g_time) == myCurrentTime)
			return;

		myCurrentTime = Mathf.FloorToInt (g_time);

		SetTime (g_time);
	}
}
