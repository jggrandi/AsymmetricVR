using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;


public class ButtonSync : NetworkBehaviour {


    public bool lTrigger = false;
    public bool lApp = false;
    public bool lA = false;
    public bool lGrip = false;
    public bool rTrigger = false;
    public bool rApp = false;
    public bool rA = false;
    public bool rGrip = false;

    Hand leftHand;
    Hand rightHand;

    Player player;

    HandleControllersButtons refLeft, refRight;

    // Use this for initialization
    void Start() {
        if (!isLocalPlayer) return;

        player = Player.instance;
        if (player == null)
        {
            Debug.LogError("No Player instance found in map.");
            Destroy(this.gameObject);
            return;
        }

        leftHand = player.leftHand;
        rightHand = player.rightHand;

        refLeft = leftHand.gameObject.GetComponent<HandleControllersButtons>();
        if (refLeft != null)
        {
            refLeft.playerObject = this.gameObject; // the VR Hands update this script. we need to send a reference of the NetPlayer.
            refLeft.handType = "left";
        }

        refRight = rightHand.gameObject.GetComponent<HandleControllersButtons>();
        if (refRight != null)
        {
            refRight.playerObject = this.gameObject;
            refRight.handType = "right";
            
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;
        if (leftHand == null) leftHand = player.leftHand;
        if (rightHand == null) rightHand = player.rightHand;
        if (refLeft == null) refLeft = leftHand.gameObject.GetComponent<HandleControllersButtons>();
        if (refRight == null) refRight = rightHand.gameObject.GetComponent<HandleControllersButtons>();
        
        lTrigger = refLeft.GetTrigger();
        lA = refLeft.GetAPress();
        lApp = refLeft.GetAppPress();
        lGrip = refLeft.GetGrip();

        rTrigger = refRight.GetTrigger();
        rA = refRight.GetAPress();
        rApp = refRight.GetAppPress();
        rGrip = refRight.GetGrip();

        CmdUpdateTriggerPressed(lTrigger,"left");
        CmdUpdateTriggerPressed(rTrigger, "right");
        CmdUpdateAPressed(lA, "left");
        CmdUpdateAPressed(rA, "right");
        CmdUpdateAppPressed(lApp, "left");
        CmdUpdateAppPressed(rApp, "right");
        CmdUpdateGripPressed(lGrip, "left");
        CmdUpdateGripPressed(rGrip, "right");

    }

    [Command]
    void CmdUpdateTriggerPressed(bool button, string side)
    {
        RpcUpdateTriggerPressed(button, side);
    }

    [ClientRpc]
    void RpcUpdateTriggerPressed(bool button, string side)
    {
        if(string.Compare(side, "left") == 0)
            lTrigger = button;
        else if (string.Compare(side, "right") == 0)
            rTrigger = button;

    }

    [Command]
    void CmdUpdateAPressed(bool button, string side)
    {
        RpcUpdateAPressed(button, side);
    }

    [ClientRpc]
    void RpcUpdateAPressed(bool button, string side)
    {
        if (string.Compare(side, "left") == 0)
            lA = button;
        else if (string.Compare(side, "right") == 0)
            rA = button;

    }

    [Command]
    void CmdUpdateAppPressed(bool button, string side)
    {
        RpcUpdateAppPressed(button, side);
    }

    [ClientRpc]
    void RpcUpdateAppPressed(bool button, string side)
    {
        if (string.Compare(side, "left") == 0)
            lApp = button;
        else if (string.Compare(side, "right") == 0)
            rApp = button;

    }

    [Command]
    void CmdUpdateGripPressed(bool button, string side)
    {
        RpcUpdateGripPressed(button, side);
    }

    [ClientRpc]
    void RpcUpdateGripPressed(bool button, string side)
    {
        if (string.Compare(side, "left") == 0)
            lGrip = button;
        else if (string.Compare(side, "right") == 0)
            rGrip = button;

    }




}
