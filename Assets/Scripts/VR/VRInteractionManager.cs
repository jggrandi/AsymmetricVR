using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class VRInteractionManager : NetworkBehaviour {

    Vector3[] oldPointsForRotation = new Vector3[2];
    float rotXOld = 0f;
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
            if (buttonSync.lTrigger )
            {
                interactingHands.Add(buttonSync.leftHand); //there is only one hand interacting. send it to the transform functions
            }
            else if (buttonSync.rTrigger)
            {
                interactingHands.Add(buttonSync.rightHand);
            }


        }

        
        
        if (interactingHands.Count <= 1)
        {
            oldPointsForRotation[0] = buttonSync.leftHand.transform.position;
            oldPointsForRotation[1] = buttonSync.rightHand.transform.position;


            oldScaleMag = 0f;
            oldScaleMag += (buttonSync.leftHand.transform.position - buttonSync.rightHand.transform.position).magnitude;
            oldScaleMag += (buttonSync.rightHand.transform.position - buttonSync.leftHand.transform.position).magnitude;
            oldScaleMag /= 2;
        }

        if (interactingHands.Count == 0)
            return;


        var newTranslation = CalcTranslation(selected, interactingHands);
        var newRotation = CalcRotation(selected, interactingHands);
        var newScale = CalcScale(selected, interactingHands);

        transformSync.isTranslating = false;
        transformSync.isRotating = false;
        transformSync.isScaling = false;

        if (buttonSync.lockCombination == 1 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 8 || buttonSync.lockCombination == 0 || buttonSync.lockCombination == 9)
        {
            this.gameObject.GetComponent<HandleNetworkTransformations>().VRTranslate(selected.index, newTranslation); // add position changes to the object
            if (Vector3.Distance(newTranslation,prevTranslation) > 0.0001f)
                transformSync.isTranslating = true;

        }
        if (buttonSync.lockCombination == 3 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 0 || buttonSync.lockCombination == 9)
        {
            //Debug.Log(newRotation);
            this.gameObject.GetComponent<HandleNetworkTransformations>().VRRotate(selected.index, newRotation); // add all rotations to the object
            if (newRotation != Quaternion.identity)
                transformSync.isRotating = true;

        }
        if (buttonSync.lockCombination == 5 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 8 || buttonSync.lockCombination == 0 || buttonSync.lockCombination == 9)
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


    public const float RAD_TO_DEG = 57.2957795130823f;


    float AngleAroundAxis(Quaternion controllerRot, Vector3 dir)
    {
        var rot = controllerRot * Vector3.forward;
        var cross1 = Vector3.Cross(rot, dir);
        Vector3 right = Vector3.Cross(cross1, dir).normalized;
        cross1 = Vector3.Cross(right, dir).normalized;

        Vector3 v = Vector3.Cross(Vector3.forward, dir);
        return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, cross1)) * RAD_TO_DEG;

    }


    float prevA1;
    float prevA2;

    float deltaAngle1;
    float deltaAngle2;

    float prevFinalAngle;

    Quaternion CalcRotation(ObjSelected objSelected, List<Hand> interactingHands)
    {
        Quaternion q = Quaternion.identity;
        if (interactingHands.Count == 1) // grabbing with one hand
            q = interactingHands[0].GetComponent<TransformStep>().rotationStep;

        else if (interactingHands.Count == 2) // grabbing with both hands, bimanual rotation
        {

            
            Vector3 directionOld = oldPointsForRotation[0] - oldPointsForRotation[1];
            Vector3 directionNew = interactingHands[0].transform.position - interactingHands[1].transform.position;

            //var q1 = interactingHands[0].transform.rotation * Quaternion.Inverse(prevQuat1);
            //var q2 = interactingHands[1].transform.rotation * Quaternion.Inverse(prevQuat2);

            var a1 = AngleAroundAxis(interactingHands[0].transform.rotation, directionNew);
            var a2 = AngleAroundAxis(interactingHands[1].transform.rotation, directionNew);

            deltaAngle1 += Mathf.DeltaAngle(prevA1, a1); 
            deltaAngle2 += Mathf.DeltaAngle(prevA2, a2);


            var alpha = (deltaAngle1 - deltaAngle2) / (deltaAngle1 + deltaAngle2);
            var b = (alpha + 1) / 2;
            var finalAngle = (deltaAngle1 * b) + (deltaAngle2 * (1 - b));

            var qq = Quaternion.AngleAxis(prevFinalAngle - finalAngle, directionNew.normalized);
            q = qq;
            //prevAngle = finalAngle;

            prevA1 = a1;
            prevA2 = a2;
            prevFinalAngle = finalAngle;

            //Debug.Log(angle);


            //Debug.DrawLine(interactingHands[0].transform.position, interactingHands[0].transform.position + cross1.normalized);
            //Debug.DrawLine(interactingHands[0].transform.position, interactingHands[0].transform.position + right.normalized, Color.red);
            //Debug.DrawLine(interactingHands[0].transform.position, interactingHands[0].transform.position + rot, Color.blue);
            //Debug.DrawLine(interactingHands[0].transform.position, interactingHands[0].transform.position + v, Color.magenta);
            //var angle1H1 = Utils.AngleOffAroundAxis(interactingHands[0].transform.forward, direction2);
            //angle1H1 *= 180 / Mathf.PI;

            //var result = angle1H1 - prevAngle;
            //var angle1H2 = Utils.AngleOffAroundAxis(direction2.normalized, interactingHands[1].transform.forward.normalized, Vector3.up);
            //angle1H2 *= 180 / Mathf.PI;

            //var finalAngle = (angle1H1 + angle1H2) / 2;
            //Debug.Log( finalAngle);

            //var rot = Quaternion.AngleAxis(result, direction2.normalized);



            Vector3 cross = Vector3.Cross(directionOld, directionNew);
            float amountToRot = Vector3.Angle(directionOld, directionNew);
            //q = Quaternion.AngleAxis(amountToRot, cross.normalized); //calculate the rotation with 2 hands
            //q =  qq;
            
            //rotXOld = rotX;
            oldPointsForRotation[0] = interactingHands[0].transform.position;
            oldPointsForRotation[1] = interactingHands[1].transform.position;

            
        }
        return q;
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
