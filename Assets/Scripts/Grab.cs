using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

namespace Valve.VR.InteractionSystem
{
    [RequireComponent(typeof(Interactable))]
    public class Grab : MonoBehaviour
    {
        public GameObject imaginaryPrefab;
        public Material hoverMat;

        private GameObject imaginary;

        private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers);
        private Color materialOriginalColor;
        private Color hoverColor, selectColor;

        //-------------------------------------------------
        void SetImaginaryTransformation()
        {
            //Sets the imaginary object position to the same as the visual representation
            imaginary.transform.position = this.transform.position;
            imaginary.transform.rotation = this.transform.rotation;
        }

        //-------------------------------------------------
        void Awake()
        {
        }

        void Start()
        {

            materialOriginalColor = GetComponent<Renderer>().material.color;
            hoverColor = new Color(0.7f, 1.0f, 0.7f, 1.0f);
            selectColor = new Color(0.1f, 1.0f, 0.1f, 1.0f);

        }
        //-------------------------------------------------
        // Called when a Hand starts hovering over this object
        //-------------------------------------------------
        private void OnHandHoverBegin(Hand hand)
        {
            this.GetComponent<MeshRenderer>().material.SetColor("_Color", hoverColor);
        }

        //-------------------------------------------------
        // Called when a Hand stops hovering over this object
        //-------------------------------------------------
        private void OnHandHoverEnd(Hand hand)
        {
            this.GetComponent<Renderer>().material.SetColor("_Color", materialOriginalColor);
        }

        //-------------------------------------------------
        private void OnHandFocusAcquired(Hand hand)
        {
            gameObject.SetActive(true);
        }

        //-------------------------------------------------
        private void OnHandFocusLost(Hand hand)
        {
            gameObject.SetActive(false);
        }

        //-------------------------------------------------
        // Called every Update() while a Hand is hovering over this object
        //-------------------------------------------------
        private void HandHoverUpdate(Hand hand)
        {

            if (hand.GetStandardInteractionButtonDown())
            {// || ((hand.controller != null) && hand.controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))) {
                if (hand.currentAttachedObject != gameObject)
                {
                    //Instantiate and imaginary god-object
                    imaginary = Instantiate(imaginaryPrefab);
                    SetImaginaryTransformation();

                    hand.HoverLock(null);
                    //Save object selected
                    //ObjectManager.SetSelected(this.gameObject,hand);

                    //Disable the logic object gravity so we can move it around freely
                    var Rb = gameObject.GetComponent<Rigidbody>();
                    Rb.useGravity = false;
                    Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    Rb.isKinematic = true;

                    var handleTransform = imaginary.GetComponent<HandleLocalTransformations>();
                    //Tie this object with the imaginary
                    handleTransform.logic = this.gameObject;

                    this.GetComponent<Renderer>().material.SetColor("_Color", selectColor);

                    // Call this to continue receiving HandHoverUpdate messages,
                    // and prevent the hand from hovering over anything else
                    //hand.HoverLock(GetComponent<Interactable>());

                    // Attach this object to the hand
                    hand.AttachObject(imaginary, attachmentFlags);
                }
            }
            if (hand.GetStandardInteractionButtonUp())// || hand.controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_Grip) && hand.currentAttachedObject != null)
            {
                hand.HoverUnlock(null);

                //ObjectManager.DeleteSelected();

                var Rb = gameObject.GetComponent<Rigidbody>();
                Rb.GetComponent<Rigidbody>().useGravity = true;
                Rb.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
                Rb.isKinematic = false;
                this.GetComponent<Renderer>().material.SetColor("_Color", hoverColor);

                hand.DetachObject(imaginary);
                Destroy(imaginary);
                // Call this to undo HoverLock
                //hand.HoverUnlock(GetComponent<Interactable>());
                //logicObject.transform.Translate(Vector3.zero);
            }
        }
    }
}

