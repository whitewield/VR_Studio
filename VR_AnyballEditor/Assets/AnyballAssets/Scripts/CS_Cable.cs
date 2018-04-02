using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Cable : MonoBehaviour {

    [SerializeField] GameObject segmentPrefab;
    [SerializeField] GameObject endPrefab;
    [SerializeField] float nrOfSegments;
    [SerializeField] float offset;
    [SerializeField] float width;
    LineRenderer lineRend;

    List<GameObject> segments = new List<GameObject>();

    private void Awake() {
        segments.Add(gameObject);
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = (int)nrOfSegments + 3;
        // first segment
        GameObject currentSegment = Instantiate(segmentPrefab, new Vector3(transform.position.x,
                                                                            transform.position.y - offset,
                                                                            transform.position.z
                                                                           ),
                                                Quaternion.identity);
        segments.Add(currentSegment);
        HingeJoint hJointFirst = GetComponent<HingeJoint>();
        hJointFirst.connectedBody = currentSegment.GetComponent<Rigidbody>();
        currentSegment.transform.parent = transform;

        // middle segments

        for (int i = 0; i < nrOfSegments; i++) {
            GameObject nextSegment = Instantiate(segmentPrefab, new Vector3(currentSegment.transform.position.x,
                                                                            currentSegment.transform.position.y - offset,
                                                                            currentSegment.transform.position.z
                                                                           ),
                                                Quaternion.identity);
            segments.Add(currentSegment);
            HingeJoint hJoint = currentSegment.GetComponent<HingeJoint>();
            hJoint.connectedBody = nextSegment.GetComponent<Rigidbody>();
            nextSegment.transform.parent = currentSegment.transform;
            currentSegment = nextSegment;
        }
        // last segment

        GameObject endSegment = Instantiate(endPrefab, new Vector3(currentSegment.transform.position.x,
                                                                   currentSegment.transform.position.y - offset,
                                                                   currentSegment.transform.position.z
                                                                           ),
                                                Quaternion.identity);
        segments.Add(currentSegment);
        HingeJoint hJointLast = currentSegment.GetComponent<HingeJoint>();
        hJointLast.connectedBody = endSegment.GetComponent<Rigidbody>();
        endSegment.transform.parent = currentSegment.transform;

        // set dimensions
        lineRend.startWidth = width;
        for (int i = 0; i < segments.Count; i++) {
            CapsuleCollider capsCol = segments[0].GetComponent<CapsuleCollider>();
            capsCol.radius = width;
            capsCol.height = offset - 0.1f;
            //segments[i].GetComponent<HingeJoint>().connectedAnchor= currentSegment.
        }
    }

    void Update(){
        // update line renderer
        for (int i= 0; i < segments.Count; i++){
            lineRend.SetPosition(i, segments[i].transform.position);
        }
    }

}
