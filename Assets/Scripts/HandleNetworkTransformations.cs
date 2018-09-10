using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

//[NetworkSettings(channel = 1, sendInterval = 0.001f)]
public class HandleNetworkTransformations : NetworkBehaviour
{
    public GameObject interactableObjects;
    public GameObject ghostObjects;

    public Vector3 tStep = new Vector3();
    public Quaternion rStep = new Quaternion();
    public float sStep = 1f;

    public const float minScale = 0.03f;
    public const float maxScale = 0.4f;

    SyncTestParameters syncParameters;
    void Start()
    {
        if (interactableObjects == null) interactableObjects = ObjectManager.manager.allInteractable;
        if (ghostObjects == null) ghostObjects = ObjectManager.manager.allGhosts;
    }

    public override void OnStartLocalPlayer()
    {
        if (isServer) return;
        CmdSyncAll();
    }

    [Command]
    public void CmdSyncAll()
    {
        var mainHandler = GameObject.Find("MainHandler");
        if (mainHandler == null) return;
        syncParameters = mainHandler.GetComponent<SyncTestParameters>();
        syncParameters.SYNC();
    }

    public GameObject RetrieveObject(int index, bool isghost)
    {
        if (isghost) return ObjectManager.GetGhost(index);
        else return ObjectManager.Get(index);
    }

    public void VRTranslate(int index, Vector3 translatestep, bool isghost)
    {
        //GameObject g = RetrieveObject(index, isghost);
        //g.transform.position += translatestep;
        CmdVRTranslate(index, translatestep, isghost);
    }

    [Command]
    void CmdVRTranslate(int index, Vector3 translatestep, bool isghost)
    {
        tStep = translatestep;
        GameObject g = RetrieveObject(index, isghost);
        g.transform.position += translatestep;
        //g.transform.position = Vector3.Lerp(g.transform.position, g.transform.position + translatestep, 0.7f);
        syncParameters.SyncObj(index, isghost);
    }

    public void VRRotate(int index, Quaternion rotationstep, bool isghost)
    {
        //GameObject g = RetrieveObject(index, isghost);
        //g.transform.rotation = rotationstep * g.transform.rotation;
        CmdVRRotate(index, rotationstep, isghost);
    }

    [Command]
    void CmdVRRotate(int index, Quaternion rotationstep, bool isghost)
    {
        rStep = rotationstep;
        GameObject g = RetrieveObject(index, isghost);

        g.transform.rotation = rotationstep * g.transform.rotation;
        //g.transform.rotation = Quaternion.Slerp(g.transform.rotation, rotationstep * g.transform.rotation, 0.7f);
        syncParameters.SyncObj(index, isghost);
    }

    public void VRScale(int index, float scalestep, bool isghost)
    {
        //GameObject g = RetrieveObject(index, isghost);
        //var finalScale = g.transform.localScale.x + scalestep;
        //finalScale = Mathf.Min(Mathf.Max(finalScale, minScale), maxScale); //limit the scale min and max
        //g.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        CmdVRScale(index, scalestep, isghost);
    }

    [Command]
    void CmdVRScale(int index, float scalestep, bool isghost)
    {
        sStep = scalestep;

        GameObject g = RetrieveObject(index, isghost);

        var finalScale = g.transform.localScale.x + scalestep;
        finalScale = Mathf.Min(Mathf.Max(finalScale, minScale), maxScale); //limit the scale min and max
        g.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        syncParameters.SyncObj(index, isghost);

    }

    //AR methods////////////////

    public Transform GetLocalTransform(bool isghost)
    {
        if (isghost)
            return ghostObjects.transform;
        return interactableObjects.transform;
    }

    [ClientRpc]
    public void RpcLockTransform(int index, Vector3 position, Quaternion rotation, bool isghost)
    {
        if (isLocalPlayer) return;
        if (interactableObjects == null) interactableObjects = ObjectManager.manager.allInteractable;
        position = GetLocalTransform(isghost).TransformPoint(position);
        rotation = rotation * GetLocalTransform(isghost).rotation;

        GameObject g = RetrieveObject(index, isghost);
        g.transform.position = position;
        g.transform.rotation = rotation;
    }

    [Command]
    public void CmdLockTransform(int index, Vector3 position, Quaternion rotation, bool isghost)
    {
        if (interactableObjects == null) interactableObjects = ObjectManager.manager.allInteractable;
        position = GetLocalTransform(isghost).TransformPoint(position);
        rotation = rotation * GetLocalTransform(isghost).rotation;

        GameObject g = RetrieveObject(index, isghost);
        g.transform.position = position;
        g.transform.rotation = rotation;

        RpcLockTransform(index, position, rotation, isghost);
        //RpcSyncObj(index, position, rotation, Vector3.zero);
    }
    public void LockTransform(int index, Vector3 position, Quaternion rotation, bool isghost)
    {
        position = GetLocalTransform(isghost).InverseTransformPoint(position);
        rotation = Quaternion.Inverse(GetLocalTransform(isghost).rotation) * rotation;
        CmdLockTransform(index, position, rotation, isghost);
    }

    public void ARTranslate(int index, Vector3 vec, bool isghost)
    {
        //GameObject g = RetrieveObject(index, isghost);
       // g.transform.localPosition += vec;
        CmdARTranslate(index, vec, isghost);
    }

    [Command]
    public void CmdARTranslate(int index, Vector3 translatestep, bool isghost)
    {
        tStep = translatestep;
        GameObject g = RetrieveObject(index, isghost);
        //objTranslateStep = vec;
        g.transform.localPosition += translatestep;
        syncParameters.SyncObj(index, isghost);
    }

    [Command]
    public void CmdARRotate(int index, Vector3 avg, Vector3 axis, float mag, bool isghost)
    {
        GameObject g = RetrieveObject(index, isghost);
        avg = GetLocalTransform(isghost).TransformPoint(avg);
        axis = GetLocalTransform(isghost).TransformVector(axis);
        g.transform.RotateAround(avg, axis, mag);
        syncParameters.SyncObj(index, isghost);
    }

    public void ARRotate(int index, Vector3 avg, Vector3 axis, float mag, bool isghost)
    {
        //GameObject g = RetrieveObject(index, isghost);
        //avg = GetLocalTransform(isghost).InverseTransformPoint(avg);
        //axis = GetLocalTransform(isghost).InverseTransformVector(axis);
        //g.transform.RotateAround(avg, axis, mag);
        CmdARRotate(index, avg, axis, mag, isghost);
    }

    [Command]
    public void CmdRotStep( Quaternion q)
    {
        rStep = q;
    }

    public void ARScale(int index, float scalestep, bool isghost)
    {
        //GameObject g = RetrieveObject(index, isghost);
        //g.transform.localScale *= scalestep;
        //var s = g.transform.localScale.x;
        //s = Mathf.Min(Mathf.Max(s, minScale), maxScale);
        //g.transform.localScale = new Vector3(s, s, s);
        CmdARScale(index, scalestep, isghost);
    }

    [Command]
    public void CmdARScale(int index, float scalestep, bool isghost)
    {
        sStep = scalestep;
        GameObject g = RetrieveObject(index, isghost);

        g.transform.localScale *= scalestep;
        var s = g.transform.localScale.x;
        s = Mathf.Min(Mathf.Max(s, minScale), maxScale);
        g.transform.localScale = new Vector3(s, s, s);

        syncParameters.SyncObj(index, isghost);
    }
}
