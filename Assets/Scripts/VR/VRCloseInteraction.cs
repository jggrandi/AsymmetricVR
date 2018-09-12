using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;

public class VRCloseInteraction : MonoBehaviour
{
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    InteractableItem objectHoveringOver = null;

    private InteractableItem closestItem;
    private InteractableItem interactingItem;

    // Use this for initialization

    SteamVR_Controller.Device controller;
    Hand hand;
    void Start()
    {
        hand = this.gameObject.GetComponent<Hand>();
        controller = hand.controller;
        //trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        controller = hand.controller;
        if ( controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }
        

        if (controller.GetPressDown(triggerButton))
        {
            if (objectHoveringOver == null) return;

            if (objectHoveringOver)
            {
                if (objectHoveringOver.IsInteracting())
                {
                    objectHoveringOver.EndInteraction(this);
                }

                objectHoveringOver.BeginInteraction(this);
            }
        }

        if (controller.GetPressUp(triggerButton) && objectHoveringOver != null)
        {
            objectHoveringOver.EndInteraction(this);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectHoveringOver = collidedItem;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectHoveringOver = null;
        }
    }
}
