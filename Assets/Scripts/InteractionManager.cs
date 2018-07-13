using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class InteractionManager : MonoBehaviour {

    public int handsGrabbingQnt = 0; // 0, 1 and 2 hands pressing the interaction button
    public GameObject interactableObjs = null;
	// Use this for initialization
	void Start () {
        interactableObjs = GameObject.Find("InteractableObjects");

    }
	
	// Update is called once per frame
	void Update () {
        var selected = ObjectManager.GetSelected();
        if (selected == null) return; // there is no object to interact with

        handsGrabbingQnt = 0;
        List<Hand> interactingHands = new List<Hand>();
        foreach (Hand h in selected.hands) // get the hands that are manipulating the object
        {
            if (h.GetComponent<Hand>().GetStandardInteractionButton())
            {
                interactingHands.Add(h);
                handsGrabbingQnt++;
            }
            
            //calculate the average and stuff
            //apply transformation to the object
        }
        if (interactingHands.Count <= 0) return; // Dont need to apply transformations

        ApplyTranslation(selected, interactingHands);
        ApplyRotation(selected, interactingHands);
        //ApplyScale(selected, interactingHands);
        //switch (handsGrabbingQnt) // the object selected will be transformed according to the hands gesture.
        //{
        //    case 1: //one hand grabbing
        //        OneHandTransform(selected, interactingHands[0]); // we know that one and only one hand is interacting.
        //        break;
        //    case 2: //two hands grabbing
        //        TwoHandsTransform(selected,interactingHands);
        //        break;
        //    default:
        //        break;
        //}

    }

    Vector3 AveragePoint(List<Hand> interactingHands)
    {
        Vector3 averagePoint = new Vector3();
        foreach (Hand h in interactingHands)
        {
            averagePoint += h.gameObject.GetComponent<TransformStep>().positionStep;
        }
        averagePoint /= interactingHands.Count;
        return averagePoint;
    }

    void ApplyTranslation(ObjSelected objSelected, List<Hand> interactingHands)
    {
        var averagePoint = AveragePoint(interactingHands);
        objSelected.gameobject.transform.position += averagePoint;

    }

    Vector3[] oldPointsForRotation = new Vector3[2];

    void ApplyRotation(ObjSelected objSelected, List<Hand> interactingHands)
    {
        if(interactingHands.Count == 1)
        {
            objSelected.gameobject.transform.rotation = interactingHands[0].GetComponent<TransformStep>().rotationStep * objSelected.gameobject.transform.rotation;
        }

        else if (interactingHands.Count == 2)
        {
            var averagePoint = AveragePoint(interactingHands);


            Vector3 direction1 = oldPointsForRotation[0] - oldPointsForRotation[1];
            Vector3 direction2 = interactingHands[0].transform.position - interactingHands[1].transform.position;
            Vector3 cross = Vector3.Cross(direction1, direction2);
            float amountToRot = Vector3.Angle(direction1, direction2);
            //Quaternion q = Quaternion.AngleAxis(amountToRot, cross.normalized);

            averagePoint = objSelected.gameobject.transform.parent.InverseTransformPoint(averagePoint);
            cross = objSelected.gameobject.transform.parent.InverseTransformVector(cross.normalized);

            //objSelected.gameobject.transform.RotateAround(averagePoint, cross, amountToRot);





            objSelected.gameobject.transform.RotateAround(cross.normalized, amountToRot*0.01f) ;

            oldPointsForRotation[0] = interactingHands[0].transform.position;
            oldPointsForRotation[1] = interactingHands[1].transform.position;
        }
    }

    void ApplyScale(ObjSelected objSelected, List<Hand> interactingHands)
    {
        if (interactingHands.Count > 1)
        {
            float averageDistance = 0;
            foreach (Hand h1 in interactingHands)
                foreach (Hand h2 in interactingHands)
                    if (h1 != h2)
                        averageDistance += (h1.transform.position - h2.transform.position).magnitude;
                
            
            averageDistance /= interactingHands.Count;
            objSelected.gameobject.transform.localScale = new Vector3(averageDistance, averageDistance, averageDistance);
        }
    }

    public Transform GetLocalTransform()
    {
        return interactableObjs.transform.parent;
    }
}
