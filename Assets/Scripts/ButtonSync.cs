using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ButtonSync : NetworkBehaviour {
    
    public bool leftInteractionButtonPressed;
    public bool rightInteractionButtonPressed;

    SteamVR_ControllerManager controllersRef;
    SteamVR_TrackedController leftController;
    SteamVR_TrackedController rightController;
    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;
        controllersRef = this.gameObject.GetComponent<SteamVR_ControllerManager>();
        leftController = controllersRef.left.GetComponent<SteamVR_TrackedController>();
        rightController = controllersRef.right.GetComponent<SteamVR_TrackedController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;
        if (controllersRef == null) return;

        if (leftController != null)
        {
            leftInteractionButtonPressed = leftController.triggerPressed;
            CmdUpdateLeftButtonPressed(leftInteractionButtonPressed);
        }

        if (rightController != null)
        {
            rightInteractionButtonPressed = rightController.triggerPressed;
            CmdUpdateRightButtonPressed(rightInteractionButtonPressed);
        }

    }

    [Command]
    void CmdUpdateLeftButtonPressed(bool toSyncButton)
    {
        RpcUpdateLeftButtonPressed(toSyncButton);
    }

    [ClientRpc]
    void RpcUpdateLeftButtonPressed(bool toSyncButton)
    {
        leftInteractionButtonPressed = toSyncButton;
    }

    [Command]
    void CmdUpdateRightButtonPressed(bool toSyncButton)
    {
        RpcUpdateRightButtonPressed(toSyncButton);
    }

    [ClientRpc]
    void RpcUpdateRightButtonPressed(bool toSyncButton)
    {
        rightInteractionButtonPressed = toSyncButton;
    }

}
