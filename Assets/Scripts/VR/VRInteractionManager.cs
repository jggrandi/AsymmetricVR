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
            rotXOld = buttonSync.leftHand.transform.rotation.eulerAngles.x + buttonSync.rightHand.transform.rotation.eulerAngles.x;

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


        if (buttonSync.lockCombination == 1 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 8 || buttonSync.lockCombination == 0 || buttonSync.lockCombination == 9)
            this.gameObject.GetComponent<HandleNetworkTransformations>().VRTranslate(selected.index, newTranslation); // add position changes to the object
        if (buttonSync.lockCombination == 3 || buttonSync.lockCombination == 4 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 0 || buttonSync.lockCombination == 9)
            this.gameObject.GetComponent<HandleNetworkTransformations>().VRRotate(selected.index, newRotation); // add all rotations to the object
        if (buttonSync.lockCombination == 5 || buttonSync.lockCombination == 6 || buttonSync.lockCombination == 8 || buttonSync.lockCombination == 0 || buttonSync.lockCombination == 9)
            this.gameObject.GetComponent<HandleNetworkTransformations>().VRScale(selected.index, newScale); // add scale to the object

    }




    Vector3 CalcTranslation(ObjSelected objSelected, List<Hand> interactingHands)
    {       
        Vector3 averagePoint = new Vector3();
        foreach (Hand h in interactingHands)
            averagePoint += h.gameObject.GetComponent<TransformStep>().positionStep;
        averagePoint /= interactingHands.Count;

        return averagePoint;
    }

    Quaternion CalcRotation(ObjSelected objSelected, List<Hand> interactingHands)
    {
        Quaternion q = Quaternion.identity;
        if (interactingHands.Count == 1) // grabbing with one hand
            q = interactingHands[0].GetComponent<TransformStep>().rotationStep;

        else if (interactingHands.Count == 2) // grabbing with both hands, bimanual rotation
        {

            //float rotX = interactingHands[0].transform.localRotation.eulerAngles.x + interactingHands[1].transform.localRotation.eulerAngles.x; //calculate the rot difference between the controllers in the X axis.
            
            //Debug.DrawLine(interactingHands[0].transform.position, interactingHands[0].transform.right);
            //Quaternion rotHands = interactingHands[0].gameObject.GetComponent<TransformStep>().rotationStep;
            
            Vector3 direction1 = oldPointsForRotation[0] - oldPointsForRotation[1];
            Vector3 direction2 = interactingHands[0].transform.position - interactingHands[1].transform.position;
            //var q2 = Quaternion.AngleAxis(, -direction2.normalized);
            
            Debug.DrawLine(interactingHands[0].transform.position, direction2.normalized + interactingHands[0].transform.position);
            Debug.Log(interactingHands[0].GetComponent<TransformStep>().rotationStep.eulerAngles.x);
            var asd = Quaternion.AngleAxis(30f, direction2.normalized + interactingHands[0].transform.position);
            //var difRotX = rotX - rotXOld;

            Vector3 cross = Vector3.Cross(direction1, direction2);
            float amountToRot = Vector3.Angle(direction1, direction2);
            q = Quaternion.AngleAxis(amountToRot, cross.normalized); //calculate the rotation with 2 hands
            //q = q * asd;
            //q = q * rotHands;
            
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
