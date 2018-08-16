using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ARTransformSync : NetworkBehaviour {

    public GameObject interactableObjects;
    public Vector3 position;
    public Quaternion rotation;

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;
        interactableObjects = GameObject.Find("InteractableObjects");
        if (interactableObjects == null) return;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;
        CmdSetARCameraPosition(interactableObjects.transform.InverseTransformPoint(Camera.main.transform.position));
        CmdSetARCameraRotation(Camera.main.transform.rotation);
        
    }

    [ClientRpc]
    public void RpcSetARCameraPosition(Vector3 p)
    {
        if (interactableObjects == null) interactableObjects = GameObject.Find("InteractableObjects");
        p = interactableObjects.transform.TransformPoint(p);
        position = p;
    }
    [Command]
    public void CmdSetARCameraPosition(Vector3 p)
    {
        RpcSetARCameraPosition(p);
    }

    [ClientRpc]
    void RpcSetARCameraRotation(Quaternion q)
    {
        if (interactableObjects == null) interactableObjects = GameObject.Find("InteractableObjects");
        rotation = q;
    }



    [Command]
    public void CmdSetARCameraRotation(Quaternion q)
    {
        RpcSetARCameraRotation(q);
    }

}
