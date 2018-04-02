using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using Hang.JellyJoystick;
using AnyBall.Rule;
using AnyBall.Editor;
using AnyBall.Property;

public class CS_GameManager : MonoBehaviour {
	
	private static CS_GameManager instance = null;
	public static CS_GameManager Instance { get { return instance; } }

	private CS_AnyLevelSave_Level myMap;
	[SerializeField] TextAsset[] maps;
	[SerializeField] string myMapName = "HalfPipe";

	[SerializeField] CS_Prop_SpawnArea mySpawnArea_Default;
	private List<CS_Prop_SpawnArea> mySpawnAreaList_Player = new List<CS_Prop_SpawnArea> ();
	private List<CS_Prop_SpawnArea> mySpawnAreaList_Object = new List<CS_Prop_SpawnArea> ();

	//score and time
	[SerializeField] int myWinningScore = 5;

	[SerializeField] List<int> myGameScore;
	[SerializeField] float myGameTimer;
	private GameStatus myGameStatus = GameStatus.Stop;
	[SerializeField] GameObject myTrophyPrefab;

	private List<CS_Prop_Button> myButtons = new List<CS_Prop_Button> ();
	[SerializeField] GameObject myButtonQuitPrefab;
	[SerializeField] GameObject myButtonContinuePrefab;

//	[SerializeField] GameObject myScoreBoardPrefab;
//	public List<CS_ScoreBoard> myScoreBoards = new List<CS_ScoreBoard> ();
//	[SerializeField] float myScoreBoardDistanceToCenter = 8;

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}

//		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () {

		//create buttons
		for (int i = 0; i < 5; i++) {
			CreateButton (CS_Prop_Button.ButtonType.Continue, false);
			CreateButton (CS_Prop_Button.ButtonType.Quit, false);
		}

		//load map
		myMap = CS_AnyLevelSave.LoadFile (maps [Random.Range (0, maps.Length)]);
//		myMap = CS_AnyLevelSave.LoadFile (myMapName);
		if (myMap != null) {
			foreach (CS_AnyLevelSave_Object f_SaveObject in myMap.anyObjects) {
				GameObject t_prefab = CS_EverythingManager.Instance.GetAnyLevelPrefab (f_SaveObject.prefabName);

				GameObject t_GameObject = Instantiate (t_prefab, this.transform);
				CS_AnyLevelObject t_anyLevelObject = t_GameObject.GetComponent<CS_AnyLevelObject> ();

				//set name
				t_GameObject.name = f_SaveObject.name;

				//set transform
				t_GameObject.transform.position = (Vector3)(f_SaveObject.position);
				t_GameObject.transform.localScale = (Vector3)(f_SaveObject.scale);
				t_GameObject.transform.rotation = (Quaternion)(f_SaveObject.rotation);

				//set invisible
				if (t_anyLevelObject.GetMyCategory () == AnyBall.Editor.Category.Invisible) {
					((CS_AnyLevelObject_Invisible)t_anyLevelObject).Hide ();
				}

				CS_Prop_SpawnArea t_spawnArea = t_GameObject.GetComponent<CS_Prop_SpawnArea> ();
				if (t_spawnArea != null) {
					if (t_spawnArea.MySpawnType == CS_Prop_SpawnArea.Type.Player) {
						mySpawnAreaList_Player.Add (t_spawnArea);
					} else {
						mySpawnAreaList_Object.Add (t_spawnArea);
					}
				}

				//remove the any level component
				Destroy (t_anyLevelObject);
			}
		}

//		//create score board
//		for (int i = 0; i < Constants.NUMBER_MAX_TEAM; i++) {
//			CS_ScoreBoard t_scoreBoard = Instantiate (myScoreBoardPrefab).GetComponent<CS_ScoreBoard> ();
//			myScoreBoards.Add (t_scoreBoard);
//
//            Vector3 t_position =
//                GetRandomSpawnArea ().GetRandomPoint ();
//
//			t_scoreBoard.Init (i, CS_PlayerManager.Instance.GetTeamColor (i), t_position);
//		}
//
//		//show and hide score board
//		for (int i = 0; i < myScoreBoards.Count; i++) {
//			if (CS_PlayerManager.Instance.IsTeamExist (i)) {
//				myScoreBoards [i].gameObject.SetActive (true);
//			} else {
//				myScoreBoards [i].gameObject.SetActive (false);
//			}
//		}

		//init game score
		myGameScore.Clear ();
		for (int i = 0; i < CS_PlayerManager.Instance.GetTeamCount (); i++) {
			myGameScore.Add (0);
		}

		//init player position
		CS_PlayerManager.Instance.HidePlayerNotInUse ();

		List<GameObject> t_players = CS_PlayerManager.Instance.MyPlayersInUse;
		for (int i = 0; i < t_players.Count; i++) {
			t_players [i].transform.position = GetRandomSpawnArea (CS_Prop_SpawnArea.Type.Player).GetRandomPoint ();
		}

		CS_GameScoreBoard.Instance.InitScoreBoard (CS_PlayerManager.Instance.GetTeamCount (), myWinningScore);

		CS_GameScoreBoard.Instance.Hide ();

		RoundStart ();
	}
	
	// Update is called once per frame
	void Update () {
		if (JellyJoystickManager.Instance.GetButton (ButtonMethodName.Down, 0, JoystickButton.BACK)) {
//			OnButtonQuit ();
//			Debug.Log ("GO TO MENU!");

			ShowButton (CS_Prop_Button.ButtonType.Continue);
			ShowButton (CS_Prop_Button.ButtonType.Quit);
		}

//		if (JellyJoystickManager.Instance.GetButton (ButtonMethodName.Down, 0, JoystickButton.START)) {
//			Debug.Log ("RoundStart");
//			RoundStart ();
//		}

//		// test restart
//		if (Input.GetKeyDown (KeyCode.Escape)) {
//			UnityEngine.SceneManagement.SceneManager.LoadScene ("Menu");
//		}
//
//		if (Input.GetKeyDown (KeyCode.O)) {
////			myGameStatus = GameStatus.Prepare;
////			myGameTimer = Constants.TIME_PREPARE;
//			RoundStart ();
//
//		}

		Update_Timer ();
	}

	private void CreateButton (CS_Prop_Button.ButtonType g_type, bool g_startActive){
		//create a new one
		GameObject t_buttonObject;
		if (g_type == CS_Prop_Button.ButtonType.Continue) {
			t_buttonObject = Instantiate (myButtonContinuePrefab, this.transform) as GameObject;
		} else {
			t_buttonObject = Instantiate (myButtonQuitPrefab, this.transform) as GameObject;
		}

		if (t_buttonObject == null) {
			Debug.LogError ("cannot create this type of button: " + g_type.ToString ());
			return;
		}

		myButtons.Add (t_buttonObject.GetComponent<CS_Prop_Button> ());
		t_buttonObject.transform.position = GetRandomSpawnArea ().GetRandomPoint ();
		t_buttonObject.SetActive (g_startActive);
	}

	private void ShowButton (CS_Prop_Button.ButtonType g_type) {
		foreach (CS_Prop_Button f_button in myButtons) {
			if (f_button.MyButtonType == g_type && f_button.gameObject.activeSelf == false) {
				f_button.gameObject.SetActive (true);
				f_button.transform.position = GetRandomSpawnArea ().GetRandomPoint ();
				return;
			}
		}
		//create a new one

		CreateButton (g_type, true);
	}

	public void OnButton (CS_Prop_Button.ButtonType g_type) {
		switch (g_type) {
		case CS_Prop_Button.ButtonType.Quit:
			OnButtonQuit ();
			break;
		case CS_Prop_Button.ButtonType.Continue:
			OnButtonContinue ();
			break;
		}
	}

	public void OnButtonQuit () {
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Menu");
	}

	public void OnButtonContinue () {
		foreach (CS_Prop_Button f_button in myButtons) {
			f_button.gameObject.SetActive (false);
		}
	}

	public void Update_Timer () {
		if (myGameStatus == GameStatus.Play || myGameStatus == GameStatus.Prepare) {
			myGameTimer -= Time.deltaTime;
		} else {
			return;
		}

		if (myGameTimer <= 0) {
			myGameTimer = 0;
			switch (myGameStatus) {
			case GameStatus.Play:
				RoundEnd ();
				break;
			case GameStatus.Prepare:
				if (!CheckWinning ())
					RoundStart ();
				break;
			}
		}
	}

	public bool CheckWinning () {
		List<int> t_winningIndex = new List<int> ();

		for (int i = 0; i < myGameScore.Count; i++){
			if (myGameScore [i] >= myWinningScore) {
				t_winningIndex.Add (i);
			}
		}

		if (t_winningIndex.Count > 0) {
			//there are teams winnning
			foreach (int f_index in t_winningIndex) {
				// create game object
				GameObject f_trophy = Instantiate (myTrophyPrefab, this.transform);
				// move to a random position
				f_trophy.transform.position = GetRandomSpawnArea ().GetRandomPoint ();
				// change color
				f_trophy.GetComponent<CS_Prop_Color> ().SetColor (CS_PlayerManager.Instance.GetTeamColorFromIndex (f_index));
			}
			myGameStatus = GameStatus.End;
			CS_GameScoreBoard.Instance.Hide ();
			return true;
		} else
			return false;
	}

	public void RoundEnd () {

		List<int> t_roundScore = CS_RuleManager.Instance.GetRoundScore ();
		for (int i = 0; i < myGameScore.Count; i++) {
			myGameScore [i] += t_roundScore [i];
		}

		CS_GameScoreBoard.Instance.UpdateScore (myGameScore);

		CS_GameScoreBoard.Instance.Show ();

		myGameStatus = GameStatus.Prepare;
		myGameTimer = Constants.TIME_PREPARE;
	}

	public void RoundStart () {
		CS_GameScoreBoard.Instance.Hide ();

		AnyBall.Rule.CS_RuleManager.Instance.NextRound ();
		myGameStatus = GameStatus.Play;
		myGameTimer = Constants.TIME_PLAY;
	}

	public GameStatus GetGameStatus () {
		return myGameStatus;
	}

	public void StartGameTimer () {
		myGameStatus = GameStatus.Prepare;
		myGameTimer = Constants.TIME_PREPARE;
	}

	public void StopGameTimer () {
		myGameStatus = GameStatus.Stop;
	}

	public float GetGameTimer () {
		return myGameTimer;
	}

	public CS_Prop_SpawnArea GetRandomSpawnArea (CS_Prop_SpawnArea.Type g_type = CS_Prop_SpawnArea.Type.Object) {
		List<CS_Prop_SpawnArea> t_areaList;
		switch (g_type) {
		case CS_Prop_SpawnArea.Type.Player:
			t_areaList = mySpawnAreaList_Player;
			break;
		default:
			t_areaList = mySpawnAreaList_Object;
			break;
		}
		if (t_areaList == null || t_areaList.Count == 0) {
			Debug.LogWarning ("can not find area in list, use the default one");
			return mySpawnArea_Default;
		}
		
        return t_areaList [Random.Range (0, t_areaList.Count)];
	}

	public CS_Prop_SpawnArea GetLargestSpawnArea (CS_Prop_SpawnArea.Type g_type = CS_Prop_SpawnArea.Type.Object) {
		List<CS_Prop_SpawnArea> t_areaList;
		switch (g_type) {
		case CS_Prop_SpawnArea.Type.Player:
			t_areaList = mySpawnAreaList_Player;
			break;
		default:
			t_areaList = mySpawnAreaList_Object;
			break;
		}
		if (t_areaList == null || t_areaList.Count == 0)
			return mySpawnArea_Default;

		CS_Prop_SpawnArea t_largest = t_areaList [0];
		for (int i = 0; i < t_areaList.Count; i++) {
			if (t_areaList [i].GetSize () > t_largest.GetSize ()) {
				t_largest = t_areaList [i];
			}
		}
		return t_largest;
	}

//	public void SetScoreBoardSize (int g_size) {
//		for (int i = 0; i < myScoreBoards.Count; i++) {
//			myScoreBoards [i].ChangeSize (g_size);
//		}
//	}
}
