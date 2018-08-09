using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class InteractionManager : NetworkBehaviour {

    Vector3[] oldPointsForRotation = new Vector3[2];
    float rotXOld = 0f;
    float oldScaleMag = 0f;

    ButtonSync buttonSync;

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;
        buttonSync = this.gameObject.GetComponent<ButtonSync>();

    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;
        if(buttonSync == null) this.gameObject.GetComponent<ButtonSync>();

        var selected = ObjectManager.GetSelected();
        if (selected == null) return; // there is no object to interact with

        List<Hand> interactingHands = new List<Hand>();
        if (buttonSync.bimanual) 
        {
            interactingHands.Add(buttonSync.leftHand); //send both hands for transform functions
            interactingHands.Add(buttonSync.rightHand); //send both hands for transform functions
        }
        else 
        {
            if (buttonSync.lTrigger)
                interactingHands.Add(buttonSync.leftHand); //there is only one hand interacting. send it to the transform functions
            else if (buttonSync.rTrigger)
                interactingHands.Add(buttonSync.rightHand);
        }


        if (interactingHands.Count <= 1)
        {
            oldPointsForRotation[0] = buttonSync.leftHand.transform.position;
            oldPointsForRotation[1] = buttonSync.rightHand.transform.position;
            rotXOld = buttonSync.leftHand.transform.rotation.eulerAngles.x + buttonSync.rightHand.transform.rotation.eulerAngles.x;

            oldScaleMag = 0f;
            oldScaleMag += (buttonSync.leftHand.transform.position - buttonSync.rightHand.transform.position).magnitude;
            oldScaleMag += (buttonSync.rightHand.transform.position - buttonSync.leftHand.transform.position).magnitude;
            oldScaleMag /= 2;
        }

        if (interactingHands.Count == 0)
            return;

        ApplyTranslation(selected, interactingHands);
        ApplyRotation(selected, interactingHands);
        ApplyScale(selected, interactingHands);



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
        var averagePoint = AveragePoint(interactingHands);
        if (buttonSync.lockCombination == 1 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 8 || buttonSync.lockCombination == 0 || buttonSync.lockCombination == 9)
            this.gameObject.GetComponent<HandleNetworkTransformations>().Translate(objSelected.index, averagePoint); // add position changes to the object
        
    }

    void ApplyRotation(ObjSelected objSelected, List<Hand> interactingHands)
    {

        if (interactingHands.Count == 1) // grabbing with one hand
            this.gameObject.GetComponent<HandleNetworkTransformations>().Rotate(objSelected.index, interactingHands[0].GetComponent<TransformStep>().rotationStep); // add single hand rotation
            
        else if (interactingHands.Count == 2) // grabbing with both hands, bimanual rotation
        {

            float rotX = interactingHands[0].transform.rotation.eulerAngles.x + interactingHands[1].transform.rotation.eulerAngles.x; //calculate the rot difference between the controllers in the X axis.

            Vector3 direction1 = oldPointsForRotation[0] - oldPointsForRotation[1];
            Vector3 direction2 = interactingHands[0].transform.position - interactingHands[1].transform.position;
            var difRotX = rotX - rotXOld;

            Vector3 cross = Vector3.Cross(direction1, direction2);
            float amountToRot = Vector3.Angle(direction1, direction2);
            Quaternion q = Quaternion.AngleAxis(amountToRot, cross.normalized); //calculate the rotation with 2 hands

            if (buttonSync.lockCombination == 3 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 0 || buttonSync.lockCombination == 9)
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
            avgScaleMag += (buttonSync.leftHand.transform.position - buttonSync.rightHand.transform.position).magnitude;
            avgScaleMag += (buttonSync.rightHand.transform.position - buttonSync.leftHand.transform.position).magnitude;

            avgScaleMag /= interactingHands.Count; // to scale the object in between both hands

            var scaleStep = avgScaleMag - oldScaleMag;

            if (buttonSync.lockCombination == 5 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 8 || buttonSync.lockCombination == 0 || buttonSync.lockCombination == 9)
                this.gameObject.GetComponent<HandleNetworkTransformations>().Scale(objSelected.index, scaleStep); // add scale to the object
            
            oldScaleMag = avgScaleMag;
        }
    }
}
