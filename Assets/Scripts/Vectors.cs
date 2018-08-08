using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vectors : MonoBehaviour {



	// Use this for initialization
	void Start () {
        GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        sphere1.transform.localScale = new Vector3(.2f, .2f, .2f);
        sphere1.GetComponent<Renderer>().material.color = Color.red;
        sphere2.transform.position = new Vector3(1f, 1f, 1f);
        sphere2.transform.localScale = new Vector3(.2f, .2f, .2f);
        sphere2.GetComponent<Renderer>().material.color = Color.blue;

        var calc = sphere1.transform.position - sphere2.transform.position;
        calc = (calc.normalized * calc.magnitude / -2f) + sphere1.transform.position;
        sphere3.transform.position = calc;
        sphere3.transform.localScale = new Vector3(.2f, .2f, .2f);
        sphere3.GetComponent<Renderer>().material.color = Color.green;

        GameObject sphere4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject sphere5 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var calc2 = (calc.normalized * calc.magnitude / 2f) + sphere1.transform.position;
        var calc3 = (calc.normalized * calc.magnitude / -2f) + sphere2.transform.position;

        sphere4.transform.position = calc2;
        sphere5.transform.position = calc3;
        sphere4.transform.localScale = new Vector3(.2f, .2f, .2f);
        sphere5.transform.localScale = new Vector3(.2f, .2f, .2f);

        Debug.Log("Norm:" + calc.normalized+ " MAG:" + calc.magnitude/-2f + " FINALPOS:" + sphere3.transform.position);


    }

    // Update is called once per frame
    void Update () {
		
	}
}
