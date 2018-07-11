using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]

public class HandleObjectSelection : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnHandHoverBegin(Hand hand)
    {
        //hand.HoverLock(null);
        //this.GetComponent<MeshRenderer>().material.SetColor("_Color", hoverColor);
    }

    private void OnHandHoverEnd(Hand hand)
    {
        // hand.HoverUnlock(null);
        //this.GetComponent<MeshRenderer>().material.SetColor("_Color", hoverColor);
    }

    private void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            if (hand.currentAttachedObject != gameObject)
            {
                //hand.HoverLock(null);
                //hand.HoverLock(gameObject.GetComponent<Interactable>());
                ObjectManager.SetSelected(gameObject, hand);

            }
        }
        if (hand.GetStandardInteractionButtonUp())
        {
            //hand.HoverUnlock(null);
            //hand.HoverUnlock(gameObject.GetComponent<Interactable>());
            ObjectManager.DetachObjectFromHand(gameObject, hand);
        }
    }
}

