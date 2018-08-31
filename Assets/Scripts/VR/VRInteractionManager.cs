using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class VRInteractionManager : NetworkBehaviour {

    Vector3[] oldPointsForRotation = new Vector3[2];
    float oldScaleMag = 0f;

    ButtonSync buttonSync;
    VRTransformSync transformSync;

    Vector3 prevTranslation = new Vector3();
    Quaternion prevRotation = new Quaternion();
    float prevScale = 0f;

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;
        buttonSync = this.gameObject.GetComponent<ButtonSync>();
        transformSync = this.gameObject.GetComponent<VRTransformSync>();
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
            if (buttonSync.lTrigger || buttonSync.lA || buttonSync.lApp || buttonSync.lGrip)
                interactingHands.Add(buttonSync.leftHand); //there is only one hand interacting. send it to the transform functions
            else if (buttonSync.rTrigger || buttonSync.rA || buttonSync.rApp || buttonSync.rGrip)
                interactingHands.Add(buttonSync.rightHand);
        }

        if (interactingHands.Count <= 1)
        {
            oldPointsForRotation[0] = buttonSync.leftHand.transform.position;
            oldPointsForRotation[1] = buttonSync.rightHand.transform.position;

            Vector3 dir = buttonSync.leftHand.transform.position - buttonSync.rightHand.transform.position;
            prevA1 = AngleAroundAxis(buttonSync.leftHand.transform.rotation, dir);
            prevA2 = AngleAroundAxis(buttonSync.rightHand.transform.rotation, dir);

            oldScaleMag = 0f;
            oldScaleMag += (buttonSync.leftHand.transform.position - buttonSync.rightHand.transform.position).magnitude;
            oldScaleMag += (buttonSync.rightHand.transform.position - buttonSync.leftHand.transform.position).magnitude;
            oldScaleMag /= 2;
        }

        transformSync.isTranslating = false;
        transformSync.isRotating = false;
        transformSync.isScaling = false;

        if (interactingHands.Count == 0)
            return;

        var newTranslation = CalcTranslation(selected, interactingHands);
        var newRotation = CalcRotation(selected, interactingHands);
        var newScale = CalcScale(selected, interactingHands);

        if (buttonSync.lTrigger || buttonSync.rTrigger || buttonSync.lockCombination == 1 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 9)
        {
            this.gameObject.GetComponent<HandleNetworkTransformations>().VRTranslate(selected.index, newTranslation); // add position changes to the object
            if (Vector3.Distance(newTranslation,prevTranslation) > 0.0001f)
                transformSync.isTranslating = true;

        }
        if (buttonSync.lTrigger || buttonSync.rTrigger || buttonSync.lockCombination == 3 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 8  || buttonSync.lockCombination == 9)
        {
            //Debug.Log(newRotation);
            this.gameObject.GetComponent<HandleNetworkTransformations>().VRRotate(selected.index, newRotation); // add all rotations to the object
            if (newRotation != Quaternion.identity)
                transformSync.isRotating = true;

        }
        if (buttonSync.lTrigger || buttonSync.rTrigger || buttonSync.lockCombination == 5 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 8 ||  buttonSync.lockCombination == 9)
        {
            this.gameObject.GetComponent<HandleNetworkTransformations>().VRScale(selected.index, newScale); // add scale to the object
            if (Mathf.Abs(newScale) > 0.0001f)
                transformSync.isScaling = true;
        }

        prevTranslation = newTranslation;
        prevRotation = newRotation;
        prevScale = newScale;

    }

    Vector3 CalcTranslation(ObjSelected objSelected, List<Hand> interactingHands)
    {       
        Vector3 averagePoint = new Vector3();
        foreach (Hand h in interactingHands)
            averagePoint += h.gameObject.GetComponent<TransformStep>().positionStep;
        averagePoint /= interactingHands.Count;

        return averagePoint;
    }

    float AngleAroundAxis(Quaternion controllerRot, Vector3 dir)
    {
        var rot = controllerRot * Vector3.forward; //creates a new coordinate system
        var cross1 = Vector3.Cross(rot, dir); //based on the axis of rotation 
        Vector3 right = Vector3.Cross(cross1, dir).normalized; // and the controller
        cross1 = Vector3.Cross(right, dir).normalized;

        Vector3 v = Vector3.Cross(Vector3.forward, dir);
        return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, cross1)) * MathUtil.RAD_TO_DEG;

    }

    float ContributionOfEachHand(float angleHand1, float angleHand2)
    {
        var alpha = (angleHand1 - angleHand2) / (angleHand1 + angleHand2);
        var b = (alpha + 1) / 2;
        return (angleHand1 * b) + (angleHand2 * (1 - b));
    }

    float prevA1;
    float prevA2;

    float deltaAngle1;
    float deltaAngle2;

    float prevFinalAngle;

    Quaternion CalcRotation(ObjSelected objSelected, List<Hand> interactingHands)
    {
        Quaternion finalRotation = Quaternion.identity;
        if (interactingHands.Count == 1) // grabbing with one hand
            finalRotation = interactingHands[0].GetComponent<TransformStep>().rotationStep;

        else if (interactingHands.Count == 2) // grabbing with both hands, bimanual rotation
        {
            
            Vector3 directionOld = oldPointsForRotation[0] - oldPointsForRotation[1];
            Vector3 directionNew = interactingHands[0].transform.position - interactingHands[1].transform.position;

            Vector3 cross = Vector3.Cross(directionOld, directionNew);
            float amountToRot = Vector3.Angle(directionOld, directionNew);
            var rotationWithTwoHands = Quaternion.AngleAxis(amountToRot, cross.normalized); //calculate the rotation with 2 hands

            var a1 = AngleAroundAxis(interactingHands[0].transform.rotation, directionNew);
            var a2 = AngleAroundAxis(interactingHands[1].transform.rotation, directionNew);

            deltaAngle1 += Mathf.DeltaAngle(prevA1, a1); // return the shortest angle between two angles (359 -> 5) will return 6
            deltaAngle2 += Mathf.DeltaAngle(prevA2, a2);
            var finalAngle = ContributionOfEachHand(deltaAngle1, deltaAngle2);

            Debug.Log(prevFinalAngle - finalAngle);


            var rotationXAxisController = Quaternion.AngleAxis(prevFinalAngle - finalAngle, directionNew.normalized);
            
            finalRotation = rotationWithTwoHands * rotationXAxisController;

            prevA1 = a1;
            prevA2 = a2;
            prevFinalAngle = finalAngle;
            oldPointsForRotation[0] = interactingHands[0].transform.position;
            oldPointsForRotation[1] = interactingHands[1].transform.position;

        }
        return finalRotation;
    }

    float CalcScale(ObjSelected objSelected, List<Hand> interactingHands)
    {
        float scaleStep = 0f;
        scaleStep += (buttonSync.lJoystick.y * 0.01f);
        scaleStep += (buttonSync.rJoystick.y * 0.01f);

        if (interactingHands.Count == 2)
        {
            float avgScaleMag = 0f;
            avgScaleMag += (buttonSync.leftHand.transform.position - buttonSync.rightHand.transform.position).magnitude;
            avgScaleMag += (buttonSync.rightHand.transform.position - buttonSync.leftHand.transform.position).magnitude;
            avgScaleMag /= interactingHands.Count; // to scale the object in between both hands

            scaleStep += (avgScaleMag - oldScaleMag);
            
            oldScaleMag = avgScaleMag;
        }
        return scaleStep;
    }
}
