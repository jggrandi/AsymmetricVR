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
    public bool bimanual = false;
    public bool lockedTrans = false;
    public bool lockedRot = false;
    public bool lockedScale = false;


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

        refLeft = GetHandReference("left"); // Send to the "HandleControllerButtons" script the reference of this player. "HandleControllerButtons" script updates th
        refRight = GetHandReference("right");

    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;

        if (refLeft == null) refLeft = GetHandReference("left"); 
        if (refRight == null) refRight = GetHandReference("right"); 
        
        lTrigger = refLeft.GetTriggerPress();
        lA = refLeft.GetAPress();
        lApp = refLeft.GetAppPress();
        lGrip = refLeft.GetGripPress();

        rTrigger = refRight.GetTriggerPress();
        rA = refRight.GetAPress();
        rApp = refRight.GetAppPress();
        rGrip = refRight.GetGripPress();

        bimanual = false;
        lockedScale = false;
        lockedTrans = false;
        lockedRot = false;

        if (lTrigger && rTrigger)
        {
            bimanual = true;
        }

        if (bimanual)
        {
            if (lA && rA) lockedRot = true;
            if (lApp && rApp) lockedTrans = true;
            if (lGrip && rGrip) lockedScale = true;

        }
        else
        {
            if(lTrigger)
            {
                if (lA) lockedRot = true;
                if (lApp) lockedTrans = true;
                if (lGrip) lockedScale = true;
            }
            else if (rTrigger)
            {
                if (rA) lockedRot = true;
                if (rApp) lockedTrans = true;
                if (rGrip) lockedScale = true;
            }
        }


        CmdUpdateTriggerPressed(lTrigger,"left");
        CmdUpdateTriggerPressed(rTrigger, "right");
        CmdUpdateAPressed(lA, "left");
        CmdUpdateAPressed(rA, "right");
        CmdUpdateAppPressed(lApp, "left");
        CmdUpdateAppPressed(rApp, "right");
        CmdUpdateGripPressed(lGrip, "left");
        CmdUpdateGripPressed(rGrip, "right");
        CmdUpdateActions(bimanual, lockedScale, lockedRot, lockedTrans);
    }

    private HandleControllersButtons GetHandReference(string side)
    {
        HandleControllersButtons hReference;
        if (string.Compare(side, "left") == 0)
            hReference = player.leftHand.gameObject.GetComponent<HandleControllersButtons>();
        else if (string.Compare(side, "right") == 0)
            hReference = player.rightHand.gameObject.GetComponent<HandleControllersButtons>();
        else
            hReference = null;

        if (hReference != null)
        {
            hReference.playerObject = this.gameObject; // the VR Hands update this script. we need to send a reference of the NetPlayer.
            return hReference;
        }

        return null;
    }

    [Command]
    void CmdUpdateActions(bool biman, bool scal, bool lockedrot, bool lockedtrans)
    {
        RpcUpdateActions(biman, scal, lockedrot, lockedtrans);
    }
    [ClientRpc]
    void RpcUpdateActions(bool biman, bool scal, bool lockedrot, bool lockedtrans)
    {
        bimanual = biman;
        lockedScale = scal;
        lockedTrans = lockedtrans;
        lockedRot = lockedrot;
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
