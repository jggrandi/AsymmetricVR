using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandleTransformations : NetworkBehaviour {

    public GameObject allObjects;

    [Command]
    void CmdSyncTransform(int index, Vector3 objPos, Quaternion objRot, bool gravity)
    {
        var g = ObjectManager.Get(index);
        g.transform.position = objPos;
        g.transform.rotation = objRot;
        g.transform.GetComponent<Rigidbody>().useGravity = gravity;


        //g.transform.position = Vector3.Lerp(g.transform.position, objPos, 0.2f);
        //g.transform.rotation = Quaternion.Slerp(g.transform.rotation, objRot, 0.2f);
        RpcSyncTransform(index, g.transform.position, g.transform.rotation,gravity);
        //Debug.Log("Server");
    }

    [ClientRpc]
    void RpcSyncTransform(int index, Vector3 objPos, Quaternion objRot, bool gravity)
    {
        var g = ObjectManager.Get(index);
        g.transform.GetComponent<Rigidbody>().useGravity = gravity;
        g.transform.position = objPos;
        g.transform.rotation = objRot;
        //Debug.Log("Client");
    }


    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;
        allObjects = GameObject.Find("InteractableObjects");
	}


    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer) return;

        for (int i=0; i < allObjects.transform.childCount; i++)
        {
            var objTransform = allObjects.transform.GetChild(i);
            CmdSyncTransform(i, objTransform.position, objTransform.rotation, objTransform.GetComponent<Rigidbody>().useGravity);
        }


    }
}
