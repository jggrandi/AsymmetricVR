using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class HandleNetworkTransformations : NetworkBehaviour
{
    public GameObject imaginary;
    public GameObject interactableObjects;


    [Command]
    void CmdSyncAll()
    {
        if (interactableObjects == null) interactableObjects = GameObject.Find("InteractableObjects");
        for (int i = 0; i < interactableObjects.transform.childCount; i++)
        {
            SyncObj(i);
        }
    }

    public void SyncObj(int index, bool pos = true, bool rot = true, bool scale = true)
    {
        Vector3 p = Vector3.zero;
        Quaternion r = new Quaternion(0, 0, 0, 0);
        Vector3 s = Vector3.zero;
        var g = ObjectManager.Get(index);
        if (pos) p = g.transform.localPosition;
        if (rot) r = g.transform.localRotation;
        if (scale) s = g.transform.localScale;
        RpcSyncObj(index, p, r, s);
    }

    [ClientRpc]
    public void RpcSyncObj(int index, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        var g = ObjectManager.Get(index);
        if (pos != Vector3.zero) g.transform.localPosition = pos;
        if (rot != new Quaternion(0, 0, 0, 0)) g.transform.localRotation = rot;
        if (scale != Vector3.zero) g.transform.localScale = scale;
    }

    [Command]
    void CmdSyncTransform(int index, Vector3 objtransstep, Quaternion objrotstep, Vector3 objscalestep)
    {
        RpcSyncTransform(index, objtransstep, objrotstep, objscalestep);
    }

    [ClientRpc]
    void RpcSyncTransform(int index, Vector3 objetransstep, Quaternion objrotstep, Vector3 objscalestep)
    {
        if (isLocalPlayer) return;
        var g = ObjectManager.Get(index);
        
        g.transform.position += objetransstep;
        g.transform.rotation = objrotstep * g.transform.rotation;
        g.transform.localScale += objscalestep; 

    }



    // Use this for initialization
    void Start()
    {
        imaginary = GameObject.Find("Imaginary");

    }

    public override void OnStartLocalPlayer()
    {
        if (isServer) return;
        CmdSyncAll(); // sync objects prosition when connected.
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (!isLocalPlayer) return;
        if (imaginary == null) return;
        if (ObjectManager.GetSelected() == null) return; // if localplayer is not selecting an object
        var objSelected = ObjectManager.GetSelected();

        List<Hand> interactingHands = new List<Hand>();
        foreach (Hand h in objSelected.hands) // get the hands that are manipulating the object
        {
            if (h.GetComponent<Hand>().GetStandardInteractionButton())
                interactingHands.Add(h);
        }

        if (interactingHands.Count <= 0) return; // Dont need to apply transformations

        //var objSelectedTransformStep = objSelected.gameobject.GetComponent<TransformStep>();
        //var posStep = objSelectedTransformStep.positionStep;
        //var rotStep = objSelectedTransformStep.rotationStep;
        //var scaleStep = objSelectedTransformStep.scaleStep;

        var objSelectedTransformStep = imaginary.GetComponent<TransformStep>();
        var posStep = objSelectedTransformStep.positionStep;
        var rotStep = objSelectedTransformStep.rotationStep;
        var scaleStep = objSelectedTransformStep.scaleStep;


        CmdSyncTransform(objSelected.index, posStep, rotStep,scaleStep);
        
    }

}
