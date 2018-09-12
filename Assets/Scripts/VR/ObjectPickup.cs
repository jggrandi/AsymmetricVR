using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
//-------------------------------------------------------------------------------------------------\\
// This code has been developed by Feisty Crab Studios for personal, commercial, and education use.\\
//                                                                                                 \\
// You are free to edit and redistribute this code, subject to the following:                      \\
//                                                                                                 \\
//      1. You will not sell this code or an edited version of it.                                 \\
//      2. You will not remove the copyright messages                                              \\
//      3. You will give credit to Feisty Crab Studios if used commercially                        \\
//      4. Don't be a mean sausage, nobody likes a mean sausage.                                   \\
//                                                                                                 \\
// Contact us @ feistycrabstudios.gmail.com with any questions.                                    \\
//-------------------------------------------------------------------------------------------------\\

public class ObjectPickup : MonoBehaviour
{
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    //private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    //private SteamVR_TrackedObject trackedObj;

    private GameObject obj;
    private FixedJoint fJoint;
    SteamVR_Controller.Device controller;
    Hand hand;
    void Start()
    {
        //trackedObj = GetComponent<SteamVR_TrackedObject>();
        hand = this.gameObject.GetComponent<Hand>();
        controller = hand.controller;
        fJoint = GetComponent<FixedJoint>();
    }

    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        //var device = SteamVR_Controller.Input((int)trackedObj.index);

        if (controller.GetPressDown(triggerButton))
        {
            PickUpObj();
        }

        if (controller.GetPressUp(triggerButton))
        {
            DropObj();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pickupable"))
        {
            obj = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        obj = null;
    }

    void PickUpObj()
    {
        if (obj != null)
        {
            fJoint.connectedBody = obj.GetComponent<Rigidbody>();
        }
        else
        {
            fJoint.connectedBody = null;
        }
    }

    void DropObj()
    {
        if (fJoint.connectedBody != null)
        {
            fJoint.connectedBody = null;
        }
    }
}