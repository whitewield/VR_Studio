using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_LineRendHanging : MonoBehaviour {

    [SerializeField] LineRenderer myLineRend;
    int myPointCount;


    private void Awake() {
        myPointCount = myLineRend.positionCount;
    }

 //   void Update () {
 //       for (int i = 0; i < myPointCount; i++){
            
 //       }
		
	//}
}
