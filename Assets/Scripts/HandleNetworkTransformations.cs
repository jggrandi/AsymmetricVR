using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class HandleNetworkTransformations : NetworkBehaviour
{
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

    public void Translate(int index, Vector3 translatestep)
    {
        var g = ObjectManager.Get(index);
        CmdTranslate(index, translatestep);
    }

    [Command]
    void CmdTranslate(int index, Vector3 translatestep)
    {
        var g = ObjectManager.Get(index);
        //g.transform.localPosition += translatestep;
        g.transform.position = Vector3.Lerp(g.transform.position, g.transform.position + translatestep, 0.7f);
        SyncObj(index);
    }

    public void Rotate(int index, Quaternion rotationstep)
    {
        CmdRotate(index, rotationstep);
    }

    [Command]
    void CmdRotate(int index, Quaternion rotationstep)
    {
        var g = ObjectManager.Get(index);
        //g.transform.rotation = rotationstep * g.transform.rotation;
        g.transform.rotation = Quaternion.Slerp(g.transform.rotation, rotationstep * g.transform.rotation, 0.7f);
        SyncObj(index);
    }

    public void Scale(int index, float scalestep)
    {
        CmdScale(index, scalestep);
    }

    [Command]
    void CmdScale(int index, float scalestep)
    {
        var g = ObjectManager.Get(index);
        var finalScale = g.transform.localScale.x + scalestep;
        
        finalScale = Mathf.Min(Mathf.Max(finalScale, 0.05f), 1.0f); //limit the scale min and max
        g.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        SyncObj(index);

    }


    // Use this for initialization
    void Start()
    {
        if (interactableObjects == null) interactableObjects = GameObject.Find("InteractableObjects");
    }

    public override void OnStartLocalPlayer()
    {
        if (isServer) return;
        CmdSyncAll(); // sync objects prosition when connected.
    }


}
