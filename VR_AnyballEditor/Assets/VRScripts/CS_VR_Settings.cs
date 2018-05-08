using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnyBall.Editor;

using Valve.VR.InteractionSystem;

public class CS_VR_Settings : MonoBehaviour {

	private static CS_VR_Settings instance = null;
	public static CS_VR_Settings Instance { get { return instance; } }

	[SerializeField] Transform myHandleTransform;
	public float HandleScale { get { return myHandleTransform.localScale.x; } }
	public Transform HandleTransform { get { return myHandleTransform; } }

	public enum SelectionMode {
		Scene,
		Object,
	}

	private SelectionMode mySelectionMode;
	private float mySnappingPosition;
	private float mySnappingScale;
	private float mySnappingRotation;

	[SerializeField] float mySnappingSpeed = 10;
	public float SnappingSpeed { get { return mySnappingSpeed; } }

	[SerializeField] GameObject myPage_File;
	[SerializeField] GameObject myPage_Edit;

	[SerializeField] TextMesh myFileName_Text;
	private List<string> myLoad_NameList;
	[SerializeField] List<TextMesh> myLoad_TextList;
	private int myLoad_PageIndex = 0;

	//Hand
	public enum HandMode {
		Edit,
		Copy,
		Delete
	}

	[SerializeField] GameObject myHandModeDisplayPrefab;
	const float myHandModeDisplay_Distance = 0.1f;

	public Sprite myHandModeSprite_Edit;
	public Sprite myHandModeSprite_Copy;
	public Sprite myHandModeSprite_Delete;

	public class HandInfo {
		public Hand myHand;
		public HandMode myMode;
		public CS_VR_HandModeDisplay myModeDisplay;

		public HandInfo (GameObject g_prefab, Hand g_hand) {
			myHand = g_hand;
			myMode = HandMode.Edit;
			myModeDisplay = Instantiate (g_prefab).GetComponent<CS_VR_HandModeDisplay> ();
		}

		public void Update () {
			Debug.Log (myModeDisplay);

			myModeDisplay.transform.position = myHand.transform.position +
			(Camera.main.transform.position -
			myHand.transform.position).normalized *
			myHandModeDisplay_Distance;
			
			myModeDisplay.transform.rotation = Camera.main.transform.rotation;
		}

		public void SetMode (HandMode g_newMode) {
			myMode = g_newMode;
			Sprite t_sprite = null;
			switch (g_newMode) {
			case HandMode.Copy:
				t_sprite = CS_VR_Settings.Instance.myHandModeSprite_Copy;
				break;
			case HandMode.Delete:
				t_sprite = CS_VR_Settings.Instance.myHandModeSprite_Delete;
				break;
			}
			myModeDisplay.ChangeSprite (t_sprite);
		}
	}

	private Dictionary<Hand, HandInfo> myHandModeDictionary = new Dictionary<Hand, HandInfo> ();

	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}

		if (myHandleTransform == null) {
			myHandleTransform = GameObject.Find ("Handle").transform;
		}

		myLoad_NameList = CS_AnyLevelSave.LoadFileNameList ();
		UpdateLoadNameDisplay ();

		Hand[] t_allHands = FindObjectOfType<Player> ().GetComponentsInChildren<Hand> (true);
		for (int i = 0; i < t_allHands.Length; i++) {
			myHandModeDictionary.Add (t_allHands [i], new HandInfo (myHandModeDisplayPrefab, t_allHands [i]));
		}
	}

	public void OnButtonReset () {
		CS_VR_Scene.Instance.Reset ();
	}

	public void OnButtonSave () {
		myFileName_Text.text = CS_VR_LevelManager.Instance.OnButtonSave ();
		myLoad_NameList = CS_AnyLevelSave.LoadFileNameList ();

		UpdateLoadNameDisplay ();
	}

	public void OnButtonSaveAs () {
		myFileName_Text.text = CS_VR_LevelManager.Instance.OnButtonSaveAs ();
		myLoad_NameList = CS_AnyLevelSave.LoadFileNameList ();

		UpdateLoadNameDisplay ();
	}

	public void OnButtonLoad (int g_index) {
		int f_nameIndex = g_index + myLoad_PageIndex * myLoad_TextList.Count;

		if (myLoad_NameList.Count > f_nameIndex) {
			CS_VR_LevelManager.Instance.SetFileName (myLoad_NameList [f_nameIndex]);
			myFileName_Text.text = CS_VR_LevelManager.Instance.OnButtonLoad ();
		}
	}

	public void OnButtonHandMode (Hand g_hand, HandMode g_mode) {
		myHandModeDictionary [g_hand].SetMode (g_mode);
	}

	public void OnButtonLoadPage (bool g_isRight) {
		int t_maxPage = Mathf.CeilToInt ((float)myLoad_NameList.Count / (float)myLoad_TextList.Count);
		Debug.Log ("MaxPage: " + t_maxPage);

		if (g_isRight) {
			myLoad_PageIndex++;
		} else {
			myLoad_PageIndex--;
		}

		Debug.Log ("GoToPage: " + myLoad_PageIndex);

		if (myLoad_PageIndex < 0) {
			Debug.Log ("--go the last page--");
			myLoad_PageIndex = t_maxPage - 1;
		} else if (myLoad_PageIndex >= t_maxPage) {
			Debug.Log ("--go back to the first page--");
			myLoad_PageIndex = 0;
		}

		UpdateLoadNameDisplay ();

		Debug.Log ("CurrentPage: " + myLoad_PageIndex);
	}

	private void UpdateLoadNameDisplay () {
		// update text display
		for (int i = 0; i < myLoad_TextList.Count; i++) {
			int f_index = i + myLoad_PageIndex * myLoad_TextList.Count;
			if (myLoad_NameList.Count > f_index)
				myLoad_TextList [i].text = myLoad_NameList [i + myLoad_PageIndex * myLoad_TextList.Count];
			else {
				myLoad_TextList [i].text = "";
			}
		}
	}

	public void SetPage (int g_index) {
		switch (g_index) {
		case 0:
			myPage_File.SetActive (true);
			myPage_Edit.SetActive (false);
			break;
		case 1:
			myPage_File.SetActive (false);
			myPage_Edit.SetActive (true);
			break;
		default:
			myPage_File.SetActive (false);
			myPage_Edit.SetActive (true);
			break;
		}
	}

	public SelectionMode GetSelectionMode () {
		return mySelectionMode;
	}

	public void SetSelectionMode (int g_index) {
		mySelectionMode = (SelectionMode)g_index;
		Debug.Log ("SetSelectionMode: " + g_index);
	}

	public float GetSnappingPosition () {
		return mySnappingPosition;
	}

	public void SetSnappingPosition (int g_index) {
		switch (g_index) {
		case 0:
			mySnappingPosition = 0f;
			break;
		case 1:
			mySnappingPosition = 0.25f;
			break;
		case 2:
			mySnappingPosition = 0.5f;
			break;
		case 3:
			mySnappingPosition = 1f;
			break;
		default:
			mySnappingPosition = 0f;
			break;
		}
	}

	public float GetSnappingRotation () {
		return mySnappingRotation;
	}

	public void SetSnappingRotation (int g_index) {

//		Debug.Log ("SetSnappingRotation");

		switch (g_index) {
		case 0:
			mySnappingRotation = 0f;
			break;
		case 1:
			mySnappingRotation = 22.5f;
			break;
		case 2:
			mySnappingRotation = 45f;
			break;
		case 3:
			mySnappingRotation = 90f;
			break;
		default:
			mySnappingRotation = 0f;
			break;
		}
	}

	public float GetSnappingScale () {
		return mySnappingScale;
	}

	public void SetSnappingScale (int g_index) {
		switch (g_index) {
		case 0:
			mySnappingScale = 0f;
			break;
		case 1:
			mySnappingScale = 0.5f;
			break;
		case 2:
			mySnappingScale = 1f;
			break;
		case 3:
			mySnappingScale = 2f;
			break;
		default:
			mySnappingScale = 0f;
			break;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Game");
		}

		foreach (Hand f_Hand in myHandModeDictionary.Keys) {
			myHandModeDictionary [f_Hand].Update ();
		}
	}

	public HandMode GetHandMode (Hand g_hand) {
		return myHandModeDictionary [g_hand].myMode;
	}
}
