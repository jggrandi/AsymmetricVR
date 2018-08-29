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

    float prevAngle = 0f;

    Quaternion CalcRotation(ObjSelected objSelected, List<Hand> interactingHands)
    {
        Quaternion q = Quaternion.identity;
        if (interactingHands.Count == 1) // grabbing with one hand
            q = interactingHands[0].GetComponent<TransformStep>().rotationStep;

        else if (interactingHands.Count == 2) // grabbing with both hands, bimanual rotation
        {

            
            Vector3 direction1 = oldPointsForRotation[0] - oldPointsForRotation[1];
            Vector3 direction2 = interactingHands[0].transform.position - interactingHands[1].transform.position;

            var angle = Utils.AngleAroundAxis(direction2.normalized, interactingHands[0].transform.forward.normalized, Vector3.up);

            Debug.Log(angle);

            //var angle1H1 = Utils.AngleOffAroundAxis(direction2.normalized, interactingHands[0].transform.forward.normalized, Vector3.up);        
            //angle1H1 *= 180 / Mathf.PI;

            //var result = angle1H1 - prevAngle;
            //var angle1H2 = Utils.AngleOffAroundAxis(direction2.normalized, interactingHands[1].transform.forward.normalized, Vector3.up);
            //angle1H2 *= 180 / Mathf.PI;

            //var finalAngle = (angle1H1 + angle1H2) / 2;
            //Debug.Log( finalAngle);

            //var rot = Quaternion.AngleAxis(result, direction2.normalized);
            


            Vector3 cross = Vector3.Cross(direction1, direction2);
            float amountToRot = Vector3.Angle(direction1, direction2);
            q = Quaternion.AngleAxis(amountToRot, cross.normalized); //calculate the rotation with 2 hands
            //q =  rot;
            
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
