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
            CmdUpdateButtonPressed(leftInteractionButtonPressed, "left");
        }

        if (rightController != null)
        {
            rightInteractionButtonPressed = rightController.triggerPressed;
            CmdUpdateButtonPressed(rightInteractionButtonPressed, "right");
        }

    }

    [Command]
    void CmdUpdateButtonPressed(bool toSyncButton, string hand)
    {
        RpcUpdateButtonPressed(toSyncButton, hand);
    }

    [ClientRpc]
    void RpcUpdateButtonPressed(bool toSyncButton, string hand)
    {
        if(string.Compare(hand,"left") == 0)
            leftInteractionButtonPressed = toSyncButton;
        else if (string.Compare(hand, "right") == 0)
            rightInteractionButtonPressed = toSyncButton;

    }

}
