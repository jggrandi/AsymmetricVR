using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class InteractionManager : MonoBehaviour {

    Vector3[] oldPointsForRotation = new Vector3[2];
    float rotXOld = 0f;
    float oldScaleMag = 0f;

    bool firstPass = true;
    // Use this for initialization
    void Start () {


    }
	
	// Update is called once per frame
	void FixedUpdate () {
        var selected = ObjectManager.GetSelected();
        if (selected == null) return; // there is no object to interact with

        List<Hand> interactingHands = new List<Hand>();
        foreach (Hand h in selected.hands) // get the hands that are manipulating the object
        {
            if (h.GetComponent<Hand>().GetStandardInteractionButton())
                interactingHands.Add(h);
        }
        if (interactingHands.Count == 1)
        {
            firstPass = true;
        }
        else if (interactingHands.Count <= 0)
        {
            firstPass = true;
            return; // Dont need to apply transformations
        }


        ApplyTranslation(selected, interactingHands);
        ApplyRotation(selected, interactingHands);
        ApplyScale(selected, interactingHands);

        if (interactingHands.Count == 2 && firstPass)
            firstPass = false;

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

    void ApplyRotation(ObjSelected objSelected, List<Hand> interactingHands)
    {
        if(interactingHands.Count == 1)
        {
            objSelected.gameobject.transform.rotation = interactingHands[0].GetComponent<TransformStep>().rotationStep * objSelected.gameobject.transform.rotation;
        }

        else if (interactingHands.Count == 2) // grabbing with both hands, bimanual rotation
        {

            float rotX = interactingHands[0].transform.rotation.eulerAngles.x + interactingHands[1].transform.rotation.eulerAngles.x; //calculate the rot difference between the controllers in the X axis.
            if (firstPass) //if it is the fist pass grabbing with both hands, start the old variables
            {
                oldPointsForRotation[0] = interactingHands[0].transform.position;
                oldPointsForRotation[1] = interactingHands[1].transform.position;
                rotXOld = rotX;
            }


            Vector3 direction1 = oldPointsForRotation[0] - oldPointsForRotation[1];
            Vector3 direction2 = interactingHands[0].transform.position - interactingHands[1].transform.position;
            Vector3 cross = Vector3.Cross(direction1, direction2);
            float amountToRot = Vector3.Angle(direction1, direction2);
            Quaternion q = Quaternion.AngleAxis(amountToRot, cross.normalized); //calculate the rotation with 2 hands

            var difRotX = rotX - rotXOld;
            objSelected.gameobject.transform.rotation = q  * Quaternion.Euler(difRotX,0f,0f) * objSelected.gameobject.transform.rotation; // add all rotations to the object

            rotXOld = rotX;
            oldPointsForRotation[0] = interactingHands[0].transform.position;
            oldPointsForRotation[1] = interactingHands[1].transform.position;
        }

    }

    void ApplyScale(ObjSelected objSelected, List<Hand> interactingHands)
    {

        if (interactingHands.Count == 1) return; //here we will add scale with single hand

        if (interactingHands.Count == 2)
        {
            float avgScaleMag = 0f;
            foreach (Hand h1 in interactingHands)
                foreach (Hand h2 in interactingHands)
                    if (h1 != h2)
                        avgScaleMag += (h1.transform.position - h2.transform.position).magnitude;


            avgScaleMag /= interactingHands.Count; // to scale the object in between both hands

            if (firstPass) // start the old variables if it is the first pass. to avoid discontinuous transformations
                oldScaleMag = avgScaleMag;    
            

            var scaleStep = avgScaleMag - oldScaleMag;
            objSelected.gameobject.transform.localScale += new Vector3(scaleStep, scaleStep, scaleStep);

            oldScaleMag = avgScaleMag;
            //objSelected.gameobject.transform.localScale = new Vector3(averageDistance, averageDistance, averageDistance);
        }
    }

}
