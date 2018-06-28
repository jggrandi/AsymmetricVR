using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandleTransformations : NetworkBehaviour {

    public GameObject allObjects;

    //[Command]
    //void CmdSyncTransform(int index, Transform objTransform)
    //{
    //    var g = ObjectManager.Get(index);
    //    g.transform.position = objTransform.position;
    //    g.transform.rotation = objTransform.rotation;
    //    RpcSyncTransform(index, objTransform);
    //    Debug.Log("Server");
    //}

    //[ClientRpc]
    //void RpcSyncTransform(int index, Transform objTransform)
    //{
    //    var g = ObjectManager.Get(index);
    //    g.transform.position = objTransform.position;
    //    g.transform.rotation = objTransform.rotation;
    //    Debug.Log("Client");
    //}


    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;

        allObjects = GameObject.Find("InteractableObjects");
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;

        Debug.Log("AAA");
        for(int i=0; i < allObjects.transform.childCount; i++)
        {
            var objTransform = allObjects.transform.GetChild(i);
            //CmdSyncTransform(i, objTransform );
        }


    }
}
