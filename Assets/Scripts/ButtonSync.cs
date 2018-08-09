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
    public int lockCombination = 0; // 0=alltransforms, 1=trans, 3=rot, 5=scale, 4=trans+rot, 6=trans+scale, 8=rot+scale, 9=allblocked


    Player player;
    public Hand leftHand, rightHand;
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
        if(leftHand != null) refLeft = leftHand.gameObject.GetComponent<HandleControllersButtons>(); // Get the reference of "HandleControllerButtons" script. "HandleControllerButtons" script updates the buttons state.
        if(rightHand != null) refRight = rightHand.gameObject.GetComponent<HandleControllersButtons>();

    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;

        if (leftHand == null) leftHand = player.leftHand; 
        if (rightHand == null) rightHand = player.rightHand; 

        refLeft = player.leftHand.gameObject.GetComponent<HandleControllersButtons>();
        refRight = player.rightHand.gameObject.GetComponent<HandleControllersButtons>();

        lTrigger = refLeft.GetTriggerPress();
        lA = refLeft.GetAPress();
        lApp = refLeft.GetAppPress();
        lGrip = refLeft.GetGripPress();

        rTrigger = refRight.GetTriggerPress();
        rA = refRight.GetAPress();
        rApp = refRight.GetAppPress();
        rGrip = refRight.GetGripPress();

        bimanual = false;
        lockCombination = 0;

        if (lTrigger && rTrigger)
            bimanual = true;
        if (bimanual)
        {
            if (lA && rA) lockCombination += 1;
            if (lApp && rApp) lockCombination += 3;
            if (lGrip && rGrip) lockCombination += 5;
        }
        else
        {
            if (lTrigger)
            {
                if (lA) lockCombination += 1;
                if (lApp) lockCombination += 3;
                if (lGrip) lockCombination += 5;
            }
            else if (rTrigger)
            {
                if (rA) lockCombination += 1;
                if (rApp) lockCombination += 3;
                if (rGrip) lockCombination += 5;
            }
        }

        CmdSyncButtons(lTrigger, rTrigger, lA, rA, lApp, rApp, lGrip, rGrip); 
        CmdUpdateActions(bimanual, lockCombination);
    }

    [Command]
    void CmdSyncButtons(bool ltrigger, bool rtrigger, bool la, bool ra, bool lapp, bool rapp, bool lgrip, bool rgrip)
    {
        RpcSyncButtons(ltrigger, rtrigger, la, ra, lapp, rapp, lgrip, rgrip);
    }

    [ClientRpc]
    void RpcSyncButtons(bool ltrigger, bool rtrigger, bool la, bool ra, bool lapp, bool rapp, bool lgrip, bool rgrip)
    {
        lTrigger = ltrigger;
        rTrigger = rtrigger;
        lA = la;
        rA = ra;
        lApp = lapp;
        rApp = rapp;
        lGrip = lgrip;
        rGrip = rgrip;

    }

    [Command]
    void CmdUpdateActions(bool biman, int lockcomb)
    {
        RpcUpdateActions(biman, lockcomb);
    }

    [ClientRpc]
    void RpcUpdateActions(bool biman, int lockcomb)
    {
        bimanual = biman;
        lockCombination = lockcomb;
    }
}
