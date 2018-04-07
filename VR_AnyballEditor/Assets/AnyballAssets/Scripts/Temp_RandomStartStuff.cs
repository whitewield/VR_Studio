using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_RandomStartStuff : MonoBehaviour {


    [SerializeField] GameObject[] startStuff;
	[SerializeField] GameObject[] stage;

	void Awake () {
        int rndNr = (int)Random.Range(0, startStuff.Length);
		int rndNr2 = (int)Random.Range(0, stage.Length);

        Instantiate(startStuff[rndNr],new Vector3(0,0,0),Quaternion.identity);
		Instantiate(stage[rndNr2],new Vector3(0,0,0),Quaternion.identity);
		Debug.Log ("map");
	}
	
}
