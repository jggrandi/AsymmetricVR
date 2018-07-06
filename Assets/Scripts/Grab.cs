using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;



[RequireComponent(typeof(Interactable))]
public class Grab : MonoBehaviour {
    public GameObject imaginaryPrefab;
    public Material hoverMat;

    private GameObject imaginary;
    //private GameObject logicObject;

    public List<Stackable> stackList;

    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers);
    private Color materialOriginalColor;
    private Color hoverColor, selectColor;

    public int qntObjsAbove = 0;
    List<List<Stackable>> allStacks = new List<List<Stackable>>();
    private List<Vector3> pointsSuggestion = new List<Vector3>();

    private GameObject drawnPoints;
    private GameObject center;

    private bool attached = false;


    //-------------------------------------------------
    void SetImaginaryTransformation()
    {
        //Sets the god-object position to the same as the visual representation
        imaginary.transform.position = this.transform.position;
        imaginary.transform.rotation = this.transform.rotation;
    }

    //-------------------------------------------------
    void Awake() {
    }

    void Start() {

        materialOriginalColor = GetComponent<Renderer>().material.color;
        hoverColor = new Color(0.7f, 1.0f, 0.7f, 1.0f);
        selectColor = new Color(0.1f, 1.0f, 0.1f, 1.0f);
        //logicObject = GameObject.Find(this.transform.name + " Logic");

    }
    //-------------------------------------------------
    // Called when a Hand starts hovering over this object
    //-------------------------------------------------
    private void OnHandHoverBegin(Hand hand) {
        //if (logicObject.GetComponent<Stackable>().baseStackable == null) return;
        Debug.Log("OnHandHoverBegin");
        this.GetComponent<MeshRenderer>().material.SetColor("_Color", hoverColor);
        //if (!attached)
        //{
        //    if (hand.GetStandardInteractionButton())
        //    {       
        //        //SetImaginaryTransformation();
        //        hand.AttachObject(gameObject, attachmentFlags);
        //    }
        //}

    }

    //-------------------------------------------------
    // Called when a Hand stops hovering over this object
    //-------------------------------------------------
    private void OnHandHoverEnd(Hand hand) {
        Debug.Log("OnHandHoverEnd");
        //if (hand.otherHand && hand.otherHand.currentAttachedObject)
        //    if (logicObject.GetComponent<Stackable>().baseStackable == null) return;
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

                attached = true;

                //Instantiate and imaginary god-object
                imaginary = Instantiate(imaginaryPrefab);
                SetImaginaryTransformation();

                hand.HoverLock(null);
                //Save object selected
                ObjectManager2.SetSelected(this.gameObject);

                //Disable the logic object gravity so we can move it around freely
                var Rb = gameObject.GetComponent<Rigidbody>();
                Rb.useGravity = false;
                Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                Rb.isKinematic = true;

                var simpleSpring = imaginary.GetComponent<SimpleSpring>();
                //Attaches a simple spring joint from the god-objce to the logic representation
                simpleSpring.logic = gameObject;
                simpleSpring.pivot = imaginary;
                simpleSpring.offset = gameObject.transform.rotation;

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

            attached = false;
            hand.HoverUnlock(null);
            
            ObjectManager2.DeleteSelected();

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

    //-------------------------------------------------
    // Search for objects stacked on the other hand and detach
    //-------------------------------------------------
    void AttachObjectToStack(GameObject thisObj, GameObject baseObj) {
        var joint = baseObj.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = thisObj.GetComponent<Rigidbody>();
        //thisObj.GetComponent<Stackable>().VisualRepresentation.gameObject.GetComponent<Renderer>().material.SetColor("_Color", selectColor);
        //thisObj.GetComponent<Rigidbody>().useGravity = false;

    }

    //-------------------------------------------------
    // Search for objects stacked on the other hand and detach
    //-------------------------------------------------
    void DetachFromOtherHand(GameObject thisHandObj, GameObject otherHandObj) {

        var joints = otherHandObj.GetComponents<FixedJoint>();
        if (joints == null) return;
        foreach (var joint in joints) {
            var aboveObjInOtherHand = joint.connectedBody.gameObject;
            if (thisHandObj.name.Equals(aboveObjInOtherHand.name)) {
                Destroy(joint);
                break;
            } else
                DetachFromOtherHand(thisHandObj, aboveObjInOtherHand);
        }
    }

    //-------------------------------------------------
    // Search for objects stacked above that are fixed with a joint and detach
    //-------------------------------------------------
    public void DetachAboveObjects(GameObject obj) {

        var joints = obj.GetComponents<FixedJoint>();
        if (joints == null) return;
        foreach (var joint in joints) {
            var connectedObj = joint.connectedBody.gameObject;
            DetachAboveObjects(connectedObj);
            var objRb = connectedObj.GetComponent<Rigidbody>();
            objRb.useGravity = true;
            objRb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Destroy(joint);
            var objVisual = connectedObj.GetComponent<Stackable>().VisualRepresentation;
            objVisual.gameObject.GetComponent<Renderer>().material.SetColor("_Color", objVisual.GetComponent<Grab>().materialOriginalColor);
        }
    }

    //-------------------------------------------------
    // Search for objects stacked above the grabbed object, attach them with the grabbed object and change their material color.
    //-------------------------------------------------
    public void AttachAboveObjects(Stackable baseObj, ref int qntObjsAbove) {
        var logics = GameObject.FindObjectsOfType<Stackable>();
        foreach (var obj in logics) {
            if (obj.baseStackable != null) // if the obj has an obj below
                if (obj != baseObj) //if the obj is not the grabbed obj
                    if (obj.baseStackable.name.Equals(baseObj.name)) {
                        AttachAboveObjects(obj, ref qntObjsAbove);
                        if (baseObj.gameObject.GetComponent<FixedJoint>() != null) return;
                        var joint = baseObj.gameObject.AddComponent<FixedJoint>();
                        joint.connectedBody = obj.GetComponent<Rigidbody>();
                        obj.VisualRepresentation.gameObject.GetComponent<Renderer>().material.SetColor("_Color", selectColor);
                        var objRb = obj.GetComponent<Rigidbody>();
                        objRb.useGravity = false;
                        objRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        qntObjsAbove++;
                    }
        }
    }

    //-------------------------------------------------
    // Search for the object with the name "podium"
    //-------------------------------------------------
    bool FindPodium(Stackable obj) {
        bool found = false;
        if (obj != null && obj.baseStackable != null) {
            if (obj.baseStackable.gameObject.name.Equals("Podium"))
                return true;
            found = FindPodium(obj.baseStackable.GetComponent<Stackable>());
        }

        if (found)
            return true;
        else
            return false;

    }

    //-------------------------------------------------
    // Find the above objects given a base object. Return the list of all above objects
    //-------------------------------------------------
    public void FindAboveObjects(Stackable baseObj, ref List<Stackable> objsAbove) {
        var logics = GameObject.FindObjectsOfType<Stackable>();
        foreach (var obj in logics) {
            if (obj.baseStackable != null) // if the obj has an obj below
                                           //if (obj != baseObj) //if the obj is not the grabbed obj
                if (obj.baseStackable.name.Equals(baseObj.name)) {
                    objsAbove.Add(obj);
                    FindAboveObjects(obj, ref objsAbove);
                }
        }
    }
}

