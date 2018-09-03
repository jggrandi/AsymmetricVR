using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class HandleNetworkTransformations : NetworkBehaviour
{
    public GameObject interactableObjects;

    public Vector3 tStep = new Vector3();
    public Quaternion rStep = new Quaternion();
    public float sStep = 1f;

    void Start()
    {
        if (interactableObjects == null) interactableObjects = GameObject.Find("InteractableObjects");
    }

    public override void OnStartLocalPlayer()
    {
        if (isServer) return;
        if (SceneManager.GetActiveScene().name != "SetupTest")
            CmdSyncAll(); // sync objects prosition when connected.
    }

    [Command]
    void CmdSyncAll()
    {
        if (interactableObjects == null) interactableObjects = GameObject.Find("InteractableObjects");
        for (int i = 0; i < interactableObjects.transform.childCount; i++)
            SyncObj(i);
    }

    public void SyncObj(int index, bool pos = true, bool rot = true, bool scale = true)
    {
        Vector3 p = Vector3.zero;
        Quaternion r = new Quaternion(0, 0, 0, 0);
        Vector3 s = Vector3.zero;
        var g = ObjectManager.Get(index);
        if (pos) p = g.transform.position;
        if (rot) r = g.transform.rotation;
        if (scale) s = g.transform.localScale;
        RpcSyncObj(index, p, r, s);
    }

    [ClientRpc]
    public void RpcSyncObj(int index, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        if (isLocalPlayer) return;
        var g = ObjectManager.Get(index);
        if (pos != Vector3.zero) g.transform.position = Vector3.Lerp(g.transform.position, pos, 0.2f);
        if (rot != new Quaternion(0, 0, 0, 0)) g.transform.rotation = Quaternion.Lerp(g.transform.rotation, rot, 0.2f);
        if (scale != Vector3.zero) g.transform.localScale = scale;
    }

    public void VRTranslate(int index, Vector3 translatestep)
    {
        var g = ObjectManager.Get(index);
        g.transform.position += translatestep;
        CmdVRTranslate(index, translatestep);
    }

    [Command]
    void CmdVRTranslate(int index, Vector3 translatestep)
    {
        var g = ObjectManager.Get(index);
        g.transform.position += translatestep;
        //g.transform.position = Vector3.Lerp(g.transform.position, g.transform.position + translatestep, 0.7f);
        SyncObj(index);
    }

    public void VRRotate(int index, Quaternion rotationstep)
    {
        var g = ObjectManager.Get(index);
        g.transform.rotation = rotationstep * g.transform.rotation;
        CmdVRRotate(index, rotationstep);
    }

    [Command]
    void CmdVRRotate(int index, Quaternion rotationstep)
    {
        rStep = rotationstep;
        var g = ObjectManager.Get(index);
        g.transform.rotation = rotationstep * g.transform.rotation;
        //g.transform.rotation = Quaternion.Slerp(g.transform.rotation, rotationstep * g.transform.rotation, 0.7f);
        SyncObj(index);
    }

    public void VRScale(int index, float scalestep)
    {
        var g = ObjectManager.Get(index);
        var finalScale = g.transform.localScale.x + scalestep;

        finalScale = Mathf.Min(Mathf.Max(finalScale, 0.05f), 1.0f); //limit the scale min and max
        g.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        CmdVRScale(index, scalestep);
    }

    [Command]
    void CmdVRScale(int index, float scalestep)
    {
        sStep = scalestep;
        var g = ObjectManager.Get(index);
        var finalScale = g.transform.localScale.x + scalestep;
        
        finalScale = Mathf.Min(Mathf.Max(finalScale, 0.05f), 1.0f); //limit the scale min and max
        g.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        SyncObj(index);

    }

    //AR methods////////////////

    public Transform GetLocalTransform()
    {
        return interactableObjects.transform;
    }

    [ClientRpc]
    public void RpcLockTransform(int index, Vector3 position, Quaternion rotation)
    {
        if (isLocalPlayer) return;
        if (interactableObjects == null) interactableObjects = GameObject.Find("InteractableObjects");
        position = GetLocalTransform().TransformPoint(position);
        rotation = rotation * GetLocalTransform().rotation;
        ObjectManager.Get(index).transform.position = position;
        ObjectManager.Get(index).transform.rotation = rotation;
    }

    [Command]
    public void CmdLockTransform(int index, Vector3 position, Quaternion rotation)
    {
        if (interactableObjects == null) interactableObjects = GameObject.Find("InteractableObjects");
        position = GetLocalTransform().TransformPoint(position);
        rotation = rotation * GetLocalTransform().rotation;
        ObjectManager.Get(index).transform.position = position;
        ObjectManager.Get(index).transform.rotation = rotation;

        RpcLockTransform(index, position, rotation);
        //RpcSyncObj(index, position, rotation, Vector3.zero);
    }
    public void LockTransform(int index, Vector3 position, Quaternion rotation)
    {
        position = GetLocalTransform().InverseTransformPoint(position);
        rotation = Quaternion.Inverse(GetLocalTransform().rotation) * rotation;
        CmdLockTransform(index, position, rotation);
    }

    public void ARTranslate(int index, Vector3 vec)
    {
        var g = ObjectManager.Get(index);
        g.transform.localPosition += vec;
        CmdARTranslate(index, vec);
    }

    [Command]
    public void CmdARTranslate(int index, Vector3 translatestep)
    {
        tStep = translatestep;
        var g = ObjectManager.Get(index);
        //objTranslateStep = vec;
        g.transform.localPosition += translatestep;
        SyncObj(index);
    }

    [Command]
    public void CmdARRotate(int index, Vector3 avg, Vector3 axis, float mag)
    {
        var g = ObjectManager.Get(index);
        avg = GetLocalTransform().TransformPoint(avg);
        axis = GetLocalTransform().TransformVector(axis);
        g.transform.RotateAround(avg, axis, mag);
        SyncObj(index);
    }

    public void ARRotate(int index, Vector3 avg, Vector3 axis, float mag)
    {
        var g = ObjectManager.Get(index);
        avg = GetLocalTransform().InverseTransformPoint(avg);
        axis = GetLocalTransform().InverseTransformVector(axis);
        g.transform.RotateAround(avg, axis, mag);
        CmdARRotate(index, avg, axis, mag);
    }

    [Command]
    public void CmdRotStep( Quaternion q)
    {
        rStep = q;
    }

    public void ARScale(int index, float scalestep)
    {
        var g = ObjectManager.Get(index);

        g.transform.localScale *= scalestep;
        var s = g.transform.localScale.x;
        s = Mathf.Min(Mathf.Max(s, 0.1f), 4.0f);
        g.transform.localScale = new Vector3(s, s, s);
        CmdARScale(index, scalestep);
    }

    [Command]
    public void CmdARScale(int index, float scalestep)
    {
        sStep = scalestep;
        var g = ObjectManager.Get(index);

        g.transform.localScale *= scalestep;
        var s = g.transform.localScale.x;
        s = Mathf.Min(Mathf.Max(s, 0.1f), 4.0f);
        g.transform.localScale = new Vector3(s, s, s);

        SyncObj(index);
    }
}
