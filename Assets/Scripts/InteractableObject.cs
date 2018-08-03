using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]

public class InteractableObject : MonoBehaviour
{
    //public GameObject imaginaryPrefab;

    

    //-------------------------------------------------


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    //private void SetImaginaryTransform()
    //{
    //    imaginary.transform.position = this.gameObject.transform.position;
    //    imaginary.transform.rotation = this.gameObject.transform.rotation;
    //    imaginary.transform.localScale = this.gameObject.transform.localScale;
    //}

    private void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            if (hand.currentAttachedObject != gameObject)
            {
                //Instantiate and imaginary god-object

                //imaginary.GetComponent<Imaginary>().objReference = this.gameObject;
                //SetImaginaryTransform();
                hand.HoverLock(null);
                hand.otherHand.HoverLock(null);
                ObjectManager.SetSelected(gameObject, hand);

            }
        }
    }
}

