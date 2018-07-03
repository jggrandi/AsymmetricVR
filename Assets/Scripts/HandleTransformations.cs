using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandleTransformations : NetworkBehaviour {

    public GameObject allObjects;

    [Command]
    void CmdSyncTransform(int index, Vector3 objPos, Quaternion objRot, bool gravity)
    {
        //var g = ObjectManager.Get(index);
        //g.transform.position = objPos;
        //g.transform.rotation = objRot;
        //g.transform.GetComponent<Rigidbody>().useGravity = gravity;


        //g.transform.position = Vector3.Lerp(g.transform.position, objPos, 0.02f);
        //g.transform.rotation = Quaternion.Slerp(g.transform.rotation, objRot, 0.02f);
        RpcSyncTransform(index, objPos, objRot, gravity);
        //Debug.Log("Server");
    }

    [ClientRpc]
    void RpcSyncTransform(int index, Vector3 objPos, Quaternion objRot, bool gravity)
    {
        if (isLocalPlayer) return;
        var g = ObjectManager.Get(index);
        var selected = ObjectManager.GetSelected();
        
        if(selected != null && g.name == selected.name)
        {
            g.transform.position = Vector3.Lerp(g.transform.position, objPos, 0.02f);
            g.transform.rotation = Quaternion.Lerp(g.transform.rotation, objRot, 0.02f);

        }
        else
        {
            g.transform.position = Vector3.Lerp(g.transform.position, objPos, 0.1f);
            g.transform.rotation = Quaternion.Lerp(g.transform.rotation, objRot, 0.1f);

        }



            //g.transform.GetComponent<Rigidbody>().useGravity = gravity;
            //g.transform.position = objPos;
            //g.transform.rotation = objRot;
            //Debug.Log("Client");

       // }

    }


    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;
        allObjects = GameObject.Find("InteractableObjects");
	}


    // Update is called once per frame
    void FixedUpdate () {
        if (!isLocalPlayer) return;
        if (ObjectManager.GetSelected() == null) return; // if localplayer is not selecting an object

        for (int i = 0; i < allObjects.transform.childCount; i++)
        {
            var objTransform = allObjects.transform.GetChild(i);
            
            if (objTransform.name == ObjectManager.GetSelected().name)
            CmdSyncTransform(i, objTransform.position, objTransform.rotation, objTransform.GetComponent<Rigidbody>().useGravity);
        }


    }
}
