using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imaginary : MonoBehaviour {

    public GameObject objReference;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (objReference == null) return;

        objReference.transform.position += this.gameObject.GetComponent<TransformStep>().positionStep;
        objReference.transform.rotation = this.gameObject.GetComponent<TransformStep>().rotationStep * objReference.transform.rotation;
        objReference.transform.localScale += this.gameObject.GetComponent<TransformStep>().scaleStep;

        //objReference.transform.position = this.gameObject.transform.position;
        //objReference.transform.rotation = this.gameObject.transform.rotation;
        //objReference.transform.localScale = this.gameObject.transform.localScale;


    }
}
