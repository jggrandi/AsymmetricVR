using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleLocalTransformations : MonoBehaviour {

    public GameObject logic;

    GetTransformStep step;

	// Use this for initialization
	void Start () {

        step = gameObject.transform.parent.GetComponent<GetTransformStep>();
	}

    // Update is called once per frame
    void FixedUpdate () {

        logic.transform.position += step.positionStep;
        logic.transform.rotation = step.rotationStep * logic.transform.rotation;


    }



}
