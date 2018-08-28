using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLikeHierarchyChild : MonoBehaviour
{
    public GameObject TargetParent;

    private Vector3 _localPosition;


    private Vector3 _lastPosition;



    HandleControllersButtons buttons;
    GameObject smooth; 
    void Start()
    {
        smooth = new GameObject(); 
        //smooth.transform.position = TargetParent.transform.position;
        //smooth.transform.rotation = TargetParent.transform.rotation;
        //_lastPosition = transform.position;
        //_localPosition = smooth.transform.InverseTransformPoint(transform.position);

        buttons = TargetParent.GetComponent<HandleControllersButtons>();
        
    }

    bool pressed = false;


    public Vector3 asd = new Vector3();



    void Update()
    {
        //smooth.transform.position = Vector3.SmoothDamp(smooth.transform.position, TargetParent.transform.position, ref velocity, smoothTime);
        smooth.transform.position = Vector3.Lerp(smooth.transform.position, TargetParent.transform.position, 0.3f);

        buttons = TargetParent.GetComponent<HandleControllersButtons>();

        if (buttons.GetADown() && !pressed)
        {
            _localPosition = smooth.transform.InverseTransformPoint(transform.position);
            pressed = true;

        }
        else if (buttons.GetARelease() && pressed)
        {
            pressed = false;
        }



        if (!pressed) return;
        asd = smooth.transform.TransformPoint(_localPosition) - _lastPosition;
        Debug.Log(asd);
        transform.position += asd;

        _lastPosition = transform.position;
    }

}