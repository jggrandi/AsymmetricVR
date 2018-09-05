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
        interactableObjects = ObjectManager.manager.allInteractable;
        if (interactableObjects == null) return;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;
        CmdSetARCameraPosition(interactableObjects.transform.InverseTransformPoint(Camera.main.transform.position));
        CmdSetARCameraRotation(Camera.main.transform.rotation);
    }

    void FindInteractable()
    {
        if (interactableObjects == null) interactableObjects = ObjectManager.manager.allInteractable;
    }

    [ClientRpc]
    public void RpcSetARCameraPosition(Vector3 p)
    {
        FindInteractable();
        p = interactableObjects.transform.TransformPoint(p);
        position = p;
    }
    [Command]
    public void CmdSetARCameraPosition(Vector3 p)
    {
        FindInteractable();
        p = interactableObjects.transform.TransformPoint(p);
        position = p;
        RpcSetARCameraPosition(p);
    }

    [ClientRpc]
    void RpcSetARCameraRotation(Quaternion q)
    {
        FindInteractable();
        rotation = q;
    }



    [Command]
    public void CmdSetARCameraRotation(Quaternion q)
    {
        FindInteractable();
        rotation = q;
        RpcSetARCameraRotation(q);
    }

}
