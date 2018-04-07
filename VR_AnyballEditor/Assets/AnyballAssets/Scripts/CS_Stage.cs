using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Stage : MonoBehaviour {

    [SerializeField] GameObject tile;
    [SerializeField] GameObject tileInnerCorner;
    [SerializeField] GameObject tileOuterCorner;
    [SerializeField] GameObject tileCircle;

    [SerializeField] int rectangleRowsX;
    [SerializeField] int rectangleRowsZ;
    [SerializeField] float tileSize;

    [SerializeField] int circleSize;


    // - nothing
    // * square
    // a outer corner top left
    // b outer corner top right
    // c outer corner bottom right
    // d outer corner bottom left
    // 1 inner corner top left
    // 2 inner corner top right
    // 3 inner corner bottom right
    // 4 inner corner bottom left
    // < circle left
    // n circle top
    // > circle right
    // u circle bottom

//    string[] stageString = new string[]{                 
//        "ww--|-rw-|----|------------",          
//        "-ww-|-wr-rrrr-|---rrrr-----",          
//        "-ww-|----r----|------r--www",
//        "-ww-|----r----|------r--ww-",
//        "-ww-|----rrrrrrrrrrrr--dww-",
//        "-wwwwwwww|----|-------dww--",          
//        "-wwwwwwwwww---|------d-----",          
//        "----|---www---|-----dd-----",
//        "----|---www---|----dd------",
//        "--ddd----w--www---dd-------",
//        "--drd----wwww-w--dd--------",
//        "--drd----|----wwdd---------",
//        "--ddd----|----|wdd---------",
//        "----dddddd----|------------",
//        "----|----d----|------------",
//    };
    
	void Start () {
        //CreateRectangleStage();
        CreateCircleStage();
	}

//    void CreateRectangleStage () {
//        for (int i = 1; i < rectangleRowsX; i++) {
//            for (int j = 1; j < rectangleRowsZ; j++) {
//                if (i == 1 && j == 1) {
//                    GameObject cornerTile = Instantiate(tileOuterCorner, new Vector3(i * tileSize, 0, j * tileSize), Quaternion.identity);
//                    } else if (i == rectangleRowsX - 1 && j == 1) {
//                    GameObject cornerTile = Instantiate(tileOuterCorner, new Vector3(i * tileSize, 0, j * tileSize), Quaternion.identity);
//                    cornerTile.transform.Rotate(new Vector3(0, -90, 0));
//                    } else if (i == 1 && j == rectangleRowsZ - 1) {
//                    GameObject cornerTile = Instantiate(tileOuterCorner, new Vector3(i * tileSize, 0, j * tileSize), Quaternion.identity);
//                    cornerTile.transform.Rotate(new Vector3(0, 90, 0));
//                    } else if (i == rectangleRowsX - 1 && j == rectangleRowsZ - 1) {
//                    GameObject cornerTile = Instantiate(tileOuterCorner, new Vector3(i * tileSize, 0, j * tileSize), Quaternion.identity);
//                    cornerTile.transform.Rotate(new Vector3(0, 180, 0));
//                    } else {
////                    GameObject newTile = Instantiate(tile, new Vector3(i * tileSize, 0, j * tileSize), Quaternion.identity);
//                    }
//                }
//            }
//    }

    void CreateCircleStage() {
        for (int i = 0; i < circleSize; i++) {
            for (int j = 0; j < circleSize; j++) {
//                    GameObject newTile = Instantiate(tile, new Vector3(i * tileSize, 0, j * tileSize), Quaternion.identity);
                }
            }
        //GameObject leftCircle = Instantiate(tileCircle, new Vector3 (-(tileSize*circleSize)/2 - tileSize/2, 0, ((circleSize -1)* tileSize) / 2), Quaternion.identity);
        GameObject bottomCircle = Instantiate(tileCircle, new Vector3 (((circleSize-1)*tileSize)/2, 0, - (tileSize * circleSize) / 2 - tileSize / 2), Quaternion.identity);
        //GameObject rightCircle = Instantiate(tileCircle, new Vector3(circleSize*tileSize, 0, ((circleSize-1) * tileSize) / 2), Quaternion.identity);
        //GameObject topCircle = Instantiate(tileCircle, new Vector3(((circleSize-1) * tileSize) / 2, 0, circleSize * tileSize), Quaternion.identity);

        //leftCircle.transform.Rotate(new Vector3(0, 270, 0));
        bottomCircle.transform.Rotate(new Vector3(0, 180, 0));
        //rightCircle.transform.Rotate(new Vector3(0, 90, 0));

        //leftCircle.transform.localScale = new Vector3(circleSize, 1, circleSize);
        bottomCircle.transform.localScale = new Vector3(circleSize, 1, circleSize);
        //rightCircle.transform.localScale = new Vector3(circleSize, 1, circleSize);
        //topCircle.transform.localScale = new Vector3(circleSize, 1, circleSize);

        }

    void CreateStageFromString(){
        
    }
	
	void Update () {
		
	}
}
