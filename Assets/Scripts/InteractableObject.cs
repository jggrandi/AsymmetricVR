using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]

public class InteractableObject : MonoBehaviour
{

    private void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            if (hand.currentAttachedObject != gameObject)
            {
                hand.HoverLock(null);
                hand.otherHand.HoverLock(null);
                ObjectManager.SetSelected(gameObject);

            }
        }
    }
}

