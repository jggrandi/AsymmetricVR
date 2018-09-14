using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;

public class VRCloseInteraction : MonoBehaviour
{
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId joystick = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    InteractableItem objectHoveringOver = null;


    SteamVR_Controller.Device controller;
    Hand hand;

    GameObject VRPlayer;
    TransformStep steps;
    CloseInteractionsSteps closeIntSteps;
    void Start()
    {
        hand = this.gameObject.GetComponent<Hand>();
        controller = hand.controller;
        //trackedObj = GetComponent<SteamVR_TrackedObject>();
        VRPlayer = GameObject.FindGameObjectWithTag("PlayerVR");
        if (VRPlayer == null) { Debug.Log("Nao achou PlayerVR"); return; }
        closeIntSteps = VRPlayer.GetComponent<CloseInteractionsSteps>();
        steps = this.gameObject.GetComponent<TransformStep>();

    }

    // Update is called once per frame
    void Update()
    {
        controller = hand.controller;
        if ( controller == null)    
            return;
        
        if (controller.GetPressDown(triggerButton))
        {
            if (objectHoveringOver == null) return;

            if (objectHoveringOver)
            {
                if (objectHoveringOver.IsInteracting())
                    objectHoveringOver.EndInteraction(this);

                objectHoveringOver.BeginInteraction(this);
            }
            
        }

        if(controller.GetPress(triggerButton) && objectHoveringOver != null)
        {
            var sStep = CalcScale();
            objectHoveringOver.ApplyScale(sStep);
            closeIntSteps.tStep = steps.positionStep;
            closeIntSteps.rStep = steps.rotationStep;
            closeIntSteps.sStep = sStep;

        }

        if (controller.GetPressUp(triggerButton) && objectHoveringOver != null)
        {
            objectHoveringOver.EndInteraction(this);
            objectHoveringOver = null;
        }

        
        
    }

    private float CalcScale()
    {
        var jvalue = controller.GetAxis(joystick);
        float scaleStep = 0f;
        scaleStep += (jvalue.y * 0.001f);
        return scaleStep;
    }

    private void OnTriggerEnter(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
            objectHoveringOver = collidedItem;
    }

    private void OnTriggerExit(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
            objectHoveringOver = null;
    }

}
