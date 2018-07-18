using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]

public class InteractableObject : MonoBehaviour
{
    //public GameObject imaginaryPrefab;

    public GameObject imaginary;

    //-------------------------------------------------
    void SetImaginaryTransformation()
    {
        //Sets the imaginary object position to the same as the visual representation
        imaginary.transform.position = this.transform.position;
        imaginary.transform.rotation = this.transform.rotation;
        imaginary.transform.localScale = this.transform.localScale;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            if (hand.currentAttachedObject != gameObject)
            {
                //Instantiate and imaginary god-object
                SetImaginaryTransformation();
                imaginary.GetComponent<Imaginary>().objReference = this.gameObject;

                hand.HoverLock(null);
                hand.otherHand.HoverLock(null);
                ObjectManager.SetSelected(gameObject, hand);

            }
        }
    }
}

