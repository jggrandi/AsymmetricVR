using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vectors : MonoBehaviour {
    GameObject s1, s2, s12, s3, s4, s34 ,s5;
    GameObject c1, c2;
    // Use this for initialization
    void Start () {
        s1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s12 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //s34 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //s5 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        //c1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //c2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //sphere1.transform.localScale = new Vector3(.2f, .2f, .2f);
        //sphere1.GetComponent<Renderer>().material.color = Color.red;
        //sphere2.transform.position = new Vector3(1f, 1f, 1f);
        //sphere2.transform.localScale = new Vector3(.2f, .2f, .2f);
        //sphere2.GetComponent<Renderer>().material.color = Color.blue;

        //var calc = sphere1.transform.position - sphere2.transform.position;
        //calc = (calc.normalized * calc.magnitude / -2f) + sphere1.transform.position;
        //sphere3.transform.position = calc;
        //sphere3.transform.localScale = new Vector3(.2f, .2f, .2f);
        //sphere3.GetComponent<Renderer>().material.color = Color.green;

        //GameObject sphere4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //GameObject sphere5 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //var calc2 = (calc.normalized * calc.magnitude / 2f) + sphere1.transform.position;
        //var calc3 = (calc.normalized * calc.magnitude / -2f) + sphere2.transform.position;

        //sphere4.transform.position = calc2;
        //sphere5.transform.position = calc3;
        //sphere4.transform.localScale = new Vector3(.2f, .2f, .2f);
        //sphere5.transform.localScale = new Vector3(.2f, .2f, .2f);

        //Debug.Log("Norm:" + calc.normalized+ " MAG:" + calc.magnitude/-2f + " FINALPOS:" + sphere3.transform.position);


        s1.transform.position = new Vector3(0f, 0f, 0f);
        s1.transform.localScale = new Vector3(.2f, .2f, .2f);
        s1.GetComponent<Renderer>().material.color = Color.red;
        s2.transform.position = new Vector3(1f, 0f, 0f);
        s2.transform.localScale = new Vector3(.2f, .2f, .2f);
        s2.GetComponent<Renderer>().material.color = Color.blue;
        s12.transform.position = new Vector3(2f, 0f, 2f);
        s12.transform.localScale = new Vector3(.2f, .2f, .2f);
        s12.GetComponent<Renderer>().material.color = Color.green;
        s3.transform.position = new Vector3(0f, 0f, 0f);
        s3.transform.localScale = new Vector3(.2f, .2f, .2f);
        s3.GetComponent<Renderer>().material.color = Color.cyan;
        s4.transform.position = new Vector3(1f, 0f, 1f);
        s4.transform.localScale = new Vector3(.2f, .2f, .2f);
        s4.GetComponent<Renderer>().material.color = Color.magenta;
        //s34.transform.position = new Vector3(0f, 0f, 0f);
        //s34.transform.localScale = new Vector3(.2f, .2f, .2f);
        //s34.GetComponent<Renderer>().material.color = Color.yellow;
        //s5.transform.position = new Vector3(0f, 0f, 0f);
        //s5.transform.localScale = new Vector3(.2f, .2f, .2f);
        //s5.GetComponent<Renderer>().material.color = Color.black;

        //c1.transform.position = new Vector3(0f, 0f, 0f);
        //c1.transform.localScale = new Vector3(.2f, .2f, .2f);
        //c1.GetComponent<Renderer>().material.color = Color.grey;
        //c2.transform.position = new Vector3(0f, 0f, 0f);
        //c2.transform.localScale = new Vector3(.2f, .2f, .2f);
        //c2.GetComponent<Renderer>().material.color = Color.green;

        s3.transform.position = (s1.transform.position - s2.transform.position);
        s12.transform.position += s3.transform.position;
 //       s34.transform.position = s12.transform.position + (s3.transform.position - s4.transform.position).normalized;
 //       float amountToRot = Vector3.Angle(s12.transform.position, s34.transform.position);
 //       Debug.Log(amountToRot);
 //       Debug.Log(s4.transform.rotation.eulerAngles);
 //       s5.transform.position = (Vector3.Cross(s12.transform.position, s34.transform.position)).normalized;

        //c2.transform.rotation = Quaternion.AngleAxis(amountToRot, s5.transform.position.normalized);
 //       c2.transform.rotation = Quaternion.AngleAxis(s4.transform.rotation.eulerAngles.x, s4.transform.position.normalized);
    }

    // Update is called once per frame
    void Update () {
        Debug.DrawLine(s1.transform.position, s2.transform.position, Color.red);
        Debug.DrawLine(s1.transform.position, s4.transform.position, Color.red);
    }
}
