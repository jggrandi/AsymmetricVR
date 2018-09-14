﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class VRDistantInteraction : MonoBehaviour {

    Vector3[] oldPointsForRotation = new Vector3[2];
    float oldScaleMag = 0f;

    ButtonSync buttonSync;

    public Vector3 tStep = new Vector3();
    public Quaternion rStep = new Quaternion();
    public float sStep = 1f;

    PlayerStuff playerStuff;

    public const float minScale = 0.03f;
    public const float maxScale = 0.4f;

    // Use this for initialization
    void Start () {
        buttonSync = this.gameObject.GetComponent<ButtonSync>();
        playerStuff = this.gameObject.GetComponent<PlayerStuff>();
    }


    // Update is called once per frame
    void Update () {

        if(buttonSync == null) this.gameObject.GetComponent<ButtonSync>();

        var selected = ObjectManager.GetSelected();
        if (selected == null) return; // there is no object to interact with

        List<Hand> interactingHands = new List<Hand>();

        if (buttonSync.whichHand == Utils.Hand.Bimanual)
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

            Vector3 dir1 = buttonSync.leftHand.transform.position - buttonSync.rightHand.transform.position;
            Vector3 dir2 = buttonSync.rightHand.transform.position - buttonSync.leftHand.transform.position ;
            prevA1 = AngleAroundAxis(buttonSync.leftHand.transform.rotation, dir1);
            prevA2 = AngleAroundAxis(buttonSync.rightHand.transform.rotation, dir2);

            oldScaleMag = 0f;
            oldScaleMag += (buttonSync.leftHand.transform.position - buttonSync.rightHand.transform.position).magnitude;
            oldScaleMag += (buttonSync.rightHand.transform.position - buttonSync.leftHand.transform.position).magnitude;
            oldScaleMag /= 2;
        }


        if (interactingHands.Count == 0)
        {
            ResetPhysics(selected);
            return;

        }

        ChangePhysics(selected);

        tStep = CalcTranslation(selected, interactingHands);
        rStep = CalcRotation(selected, interactingHands);
        sStep = CalcScale(selected, interactingHands);

        if (buttonSync.lTrigger || buttonSync.rTrigger || buttonSync.lockCombination == 1 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 9)
            selected.gameobject.transform.position += tStep;
        if (buttonSync.lTrigger || buttonSync.rTrigger || buttonSync.lockCombination == 3 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 8  || buttonSync.lockCombination == 9)
            selected.gameobject.transform.rotation =  rStep * selected.gameobject.transform.rotation;
        if (buttonSync.lTrigger || buttonSync.rTrigger || buttonSync.lockCombination == 5 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 8 ||  buttonSync.lockCombination == 9)
        {
            var finalScale = selected.gameobject.transform.localScale.x + sStep;
            finalScale = Mathf.Min(Mathf.Max(finalScale, minScale), maxScale); //limit the scale min and max
            selected.gameobject.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        }

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
            Vector3 directionOpp = interactingHands[1].transform.position - interactingHands[0].transform.position ;

            Vector3 cross = Vector3.Cross(directionOld, directionNew);
            float amountToRot = Vector3.Angle(directionOld, directionNew);
            var rotationWithTwoHands = Quaternion.AngleAxis(amountToRot, cross.normalized); //calculate the rotation with 2 hands

            var a1 = AngleAroundAxis(interactingHands[0].transform.rotation, directionNew);
            var a2 = AngleAroundAxis(interactingHands[1].transform.rotation, directionOpp);

            deltaAngle1 += Mathf.DeltaAngle(prevA1, a1); // return the shortest angle between two angles (359 -> 5) will return 6
            deltaAngle2 += Mathf.DeltaAngle(prevA2, a2);

            //var finalAngle = ContributionOfEachHand(deltaAngle1, deltaAngle2);
            var finalAngle = (deltaAngle1 - deltaAngle2) /2 ;

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
        scaleStep += (buttonSync.lJoystick.y * 0.001f);
        scaleStep += (buttonSync.rJoystick.y * 0.001f);

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

    void ChangePhysics(ObjSelected obj)
    {
        var objRB = obj.gameobject.GetComponent<Rigidbody>();
        objRB.mass = 10000f;
        objRB.drag = 10000f;
        objRB.angularDrag = 10000f;
        objRB.useGravity = false;
    }

    void ResetPhysics(ObjSelected obj)
    {
        var objRB = obj.gameobject.GetComponent<Rigidbody>();
        objRB.mass = 5f;
        objRB.drag = 0f;
        objRB.angularDrag = 0.05f;
        objRB.useGravity = true;
    }

}
