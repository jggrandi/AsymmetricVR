﻿//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Demonstrates how to create a simple interactable object
//
//=============================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    [RequireComponent(typeof(Interactable))]
    public class NetInteractable : NetworkBehaviour
    {
        private TextMesh textMesh;
        private Vector3 oldPosition;
        private Quaternion oldRotation;

        private float attachTime;

        private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers);

        //-------------------------------------------------
        void Awake()
        {

        }


        //-------------------------------------------------
        // Called when a Hand starts hovering over this object
        //-------------------------------------------------
        private void OnHandHoverBegin(Hand hand)
        {

        }


        //-------------------------------------------------
        // Called when a Hand stops hovering over this object
        //-------------------------------------------------
        private void OnHandHoverEnd(Hand hand)
        {

        }


        //-------------------------------------------------
        // Called every Update() while a Hand is hovering over this object
        //-------------------------------------------------
        private void HandHoverUpdate(Hand hand)
        {
            if (!isLocalPlayer) return;
            if (hand.GetStandardInteractionButtonDown() || ((hand.controller != null) && hand.controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip)))
            {
                if (hand.currentAttachedObject != gameObject)
                {
                    // Save our position/rotation so that we can restore it when we detach
                    oldPosition = transform.position;
                    oldRotation = transform.rotation;

                    // Call this to continue receiving HandHoverUpdate messages,
                    // and prevent the hand from hovering over anything else
                    hand.HoverLock(GetComponent<Interactable>());

                    // Attach this object to the hand
                    hand.AttachObject(gameObject, attachmentFlags);
                }
                else
                {
                    // Detach this object from the hand
                    hand.DetachObject(gameObject);

                    // Call this to undo HoverLock
                    hand.HoverUnlock(GetComponent<Interactable>());

                    // Restore position/rotation
                    transform.position = oldPosition;
                    transform.rotation = oldRotation;
                }
            }
        }


        //-------------------------------------------------
        // Called when this GameObject becomes attached to the hand
        //-------------------------------------------------
        private void OnAttachedToHand(Hand hand)
        {

        }


        //-------------------------------------------------
        // Called when this GameObject is detached from the hand
        //-------------------------------------------------
        private void OnDetachedFromHand(Hand hand)
        {

        }


        //-------------------------------------------------
        // Called every Update() while this GameObject is attached to the hand
        //-------------------------------------------------
        private void HandAttachedUpdate(Hand hand)
        {

        }


        //-------------------------------------------------
        // Called when this attached GameObject becomes the primary attached object
        //-------------------------------------------------
        private void OnHandFocusAcquired(Hand hand)
        {
        }


        //-------------------------------------------------
        // Called when another attached GameObject becomes the primary attached object
        //-------------------------------------------------
        private void OnHandFocusLost(Hand hand)
        {
        }
    }
}
