using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]

public class InteractableObject : MonoBehaviour
{
    public GameObject imaginaryPrefab;
    private GameObject imaginary;
    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers);
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //-------------------------------------------------
    void SetImaginaryTransformation()
    {
        //Sets the imaginary object position to the same as the visual representation
        imaginary.transform.position = this.transform.position;
        imaginary.transform.rotation = this.transform.rotation;
    }

    private void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            if (hand.currentAttachedObject != gameObject)
            {
                //imaginary = Instantiate(imaginaryPrefab);
                //SetImaginaryTransformation();

                hand.HoverLock(null);
                ObjectManager.SetSelected(gameObject, hand);

            }
        }
        //if (hand.GetStandardInteractionButtonUp())
        //{
        //    hand.HoverUnlock(null);
        //    ObjectManager.DetachObjectFromHand(gameObject, hand);
        //}
    }
}

