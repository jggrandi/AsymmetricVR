using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class HandleNetworkTransformations : NetworkBehaviour
{

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
        
        //g.transform.position += objetransstep;
        g.transform.rotation = objrotstep * g.transform.rotation;
        //g.transform.localScale += objscalestep; 

    }

    public GameObject imaginary;

    // Use this for initialization
    void Start()
    {
        imaginary = GameObject.Find("Imaginary");

    }

    public override void OnStartClient()
    {

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
