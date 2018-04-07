using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ScoreboardNumber : MonoBehaviour {


	SpriteRenderer spriteRend;
	[SerializeField] List <Sprite> numbers = new List <Sprite> ();
	[SerializeField] int teamNr;
	int score=0;



	void Awake () {
		spriteRend = GetComponent<SpriteRenderer> ();

		
	}

	void Start() {
//		spriteRend.color = CS_GameManager.Instance.GetTeamColor (teamNr);
	}


	public void AddScore() {

		score = Mathf.Min (9, score + 1);
		spriteRend.sprite = numbers [score];
	}

	public int GetScore () {
		return score;
	}

	public void MinusScore() {

		if (score > 0) {
			score = Mathf.Min (9, score - 1);
			spriteRend.sprite = numbers [score];
		}


	}

	 
}
