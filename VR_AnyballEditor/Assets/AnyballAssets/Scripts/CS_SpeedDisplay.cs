using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_SpeedDisplay : MonoBehaviour {

	[SerializeField] GameObject target;
	[SerializeField] SpriteRenderer hundred;
	[SerializeField] SpriteRenderer ten;
	[SerializeField] SpriteRenderer one;

	[SerializeField] List <Sprite> numbers = new List <Sprite> ();

	int hundredNr;
	int tenNr;
	int oneNr;

	Rigidbody targetRb;
	public float targetVelocity;
	void Start () {
		targetRb = target.GetComponent<Rigidbody> ();
	}
	

	void Update () {
		targetVelocity = Mathf.Abs(targetRb.velocity.magnitude);


		hundredNr = Mathf.FloorToInt (targetVelocity / 100f);
		hundred.sprite = numbers [hundredNr];
		tenNr = Mathf.FloorToInt (targetVelocity / 10f) - hundredNr*10;
		ten.sprite = numbers [tenNr];
		oneNr = Mathf.FloorToInt (targetVelocity) - hundredNr*10 - tenNr*10;
		one.sprite = numbers [oneNr];
	}
}
