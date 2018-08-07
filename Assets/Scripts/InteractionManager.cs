using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class InteractionManager : NetworkBehaviour {

    Vector3[] oldPointsForRotation = new Vector3[2];
    float rotXOld = 0f;
    float oldScaleMag = 0f;

    bool firstPass = true;

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;


    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;

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
            averagePoint += h.gameObject.GetComponent<TransformStep>().positionStep;
        averagePoint /= interactingHands.Count;
        return averagePoint;
    }

    void ApplyTranslation(ObjSelected objSelected, List<Hand> interactingHands)
    {
        int locked = VerifyLockedDOF(interactingHands,"translation");
        if (locked > 0) return; // any number greater than 0 means that this dof is locked
        
        var averagePoint = AveragePoint(interactingHands);
        this.gameObject.GetComponent<HandleNetworkTransformations>().Translate(objSelected.index, averagePoint); // add position changes to the object
        
    }

    void ApplyRotation(ObjSelected objSelected, List<Hand> interactingHands)
    {
        int locked = VerifyLockedDOF(interactingHands, "rotation");
        if (locked > 0) return; // any number greater than 0 means that this dof is locked

        if (interactingHands.Count == 1) // grabbing with one hand
            this.gameObject.GetComponent<HandleNetworkTransformations>().Rotate(objSelected.index, interactingHands[0].GetComponent<TransformStep>().rotationStep); // add single hand rotation
            
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

            this.gameObject.GetComponent<HandleNetworkTransformations>().Rotate(objSelected.index, q * Quaternion.Euler(difRotX, 0f, 0f)); // add all rotations to the object

            rotXOld = rotX;
            oldPointsForRotation[0] = interactingHands[0].transform.position;
            oldPointsForRotation[1] = interactingHands[1].transform.position;
        }

    }

    void ApplyScale(ObjSelected objSelected, List<Hand> interactingHands)
    {

        //if (interactingHands.Count == 1)
        //{
        //    Hand h1 = interactingHands[0];
        //    if (h1.gameObject.GetComponent<HandleControllersButtons>().GetGripDown())
        //    {
        //        if (firstPass) // start the old variables if it is the first pass. to avoid discontinuous transformations
        //            oldScaleMag = h1.transform.position.magnitude;
        //        var scaleStep = h1.transform.position.magnitude - oldScaleMag;
        //        var finalScale = imaginary.transform.localScale.x + scaleStep;
        //        finalScale = Mathf.Min(Mathf.Max(finalScale, 0.05f), 1.0f); //limit the scale min and max
        //        imaginary.transform.localScale = new Vector3(finalScale, finalScale, finalScale);

        //        oldScaleMag = h1.transform.position.magnitude;
        //    }

        //}

        if (interactingHands.Count == 2)
        {

            float avgScaleMag = 0f;
            foreach (Hand hi in interactingHands)
                foreach (Hand hj in interactingHands)
                    if (hi != hj)
                        avgScaleMag += (hi.transform.position - hj.transform.position).magnitude;

            avgScaleMag /= interactingHands.Count; // to scale the object in between both hands

            if (firstPass) // start the old variables if it is the first pass. to avoid discontinuous transformations
                oldScaleMag = avgScaleMag;

            var scaleStep = avgScaleMag - oldScaleMag;

            if (BothGripPressed(interactingHands))// apply scale only if the grip button of both controllers are pressed.
                this.gameObject.GetComponent<HandleNetworkTransformations>().Scale(objSelected.index, scaleStep); // add scale to the object
            
            oldScaleMag = avgScaleMag;
        }
    }


    private bool BothGripPressed(List<Hand> interactingHands)
    {
        var h1 = interactingHands[0];
        var h2 = interactingHands[1];
        if (h1.gameObject.GetComponent<HandleControllersButtons>().GetGripPress() && h2.gameObject.GetComponent<HandleControllersButtons>().GetGripPress()) 
            return true;
        return false;
    }

    private int VerifyLockedDOF(List<Hand> interactingHands, string dof)
    {
        int lockedTrans = 0;
        if (string.Compare(dof, "translation") == 0)
        {
            foreach (Hand h in interactingHands)
            {
                if (h.GetComponent<HandleControllersButtons>().GetAppPress())
                    lockedTrans++;
            }
        }
        else if (string.Compare(dof, "rotation") == 0)
        {
            foreach (Hand h in interactingHands)
            {
                if (h.GetComponent<HandleControllersButtons>().GetAPress())
                    lockedTrans++;
            }
        }

        return lockedTrans;
    }
}
