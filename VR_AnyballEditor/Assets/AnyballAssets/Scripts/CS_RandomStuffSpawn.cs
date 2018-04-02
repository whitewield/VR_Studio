using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_RandomStuffSpawn : MonoBehaviour {

    [SerializeField] GameObject []randomStuff;

	void Start () {
        int rndNr = (int)(Random.Range(0, randomStuff.Length));
        Instantiate(randomStuff[rndNr],transform.position,Quaternion.identity);
	}
	
	
}
