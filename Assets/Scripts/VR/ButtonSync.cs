﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;


public class ButtonSync : NetworkBehaviour {


    public bool lTrigger = false;
    public bool rTrigger = false;

    public bool lA = false;
    public bool rA = false;

    public bool lApp = false;
    public bool rApp = false;

    public bool lGrip = false;
    public bool rGrip = false;

    public Vector2 lJoystick = new Vector2();
    public Vector2 rJoystick = new Vector2();

    public bool bimanual = false;
    public int lockCombination = 0; // 0=alltransforms, 1=trans, 3=rot, 5=scale, 4=trans+rot, 6=trans+scale, 8=rot+scale, 9=allblocked

    Player player;
    public Hand leftHand, rightHand;
    HandleControllersButtons refLeft, refRight;

    SyncTestParameters syncParameters;

    // Use this for initialization
    void Start() {
        if (string.Compare(SceneManager.GetActiveScene().name, "SetupTest") == 0) return;
        if (!isLocalPlayer) return;

        var mainHandler = GameObject.Find("MainHandler");
        if (mainHandler == null) Debug.Log("WHAATTTTTTT");
        syncParameters = mainHandler.GetComponent<SyncTestParameters>();

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

        refLeft = leftHand.gameObject.GetComponent<HandleControllersButtons>();
        refRight = rightHand.gameObject.GetComponent<HandleControllersButtons>();

        if (refLeft == null) return;
        if (refRight == null) return;

        lTrigger = refLeft.GetTriggerPress();
        lA = refLeft.GetAPress();
        lApp = refLeft.GetAppPress();
        lGrip = refLeft.GetGripPress();
        lJoystick = refLeft.GetJoystickCoord();

        rTrigger = refRight.GetTriggerPress();
        rA = refRight.GetAPress();
        rApp = refRight.GetAppPress();
        rGrip = refRight.GetGripPress();
        rJoystick = refRight.GetJoystickCoord();

        bimanual = false;
        lockCombination = 0;

        if (AnyButtonPressedLeft() && AnyButtonPressedRight())
            bimanual = true;
        //if ((lTrigger && rTrigger) || (lA && rA) || (lApp && rApp) || (lGrip && rGrip))
        //    bimanual = true;

        //if (!(AnyButtonPressedLeft() || AnyButtonPressedRight()))
        //    return;

        if (bimanual)
        {
            //if (lTrigger && rTrigger) lockCombination = 0;
            if (lA && rA) lockCombination += 1;
            if (lApp && rApp) lockCombination += 3;
            if (lGrip && rGrip) lockCombination += 5;
        }
        else
        {
            if (!lTrigger && !rTrigger)
            {
              //  if (lTrigger || rTrigger) lockCombination = 0;
                if (lA || rA) lockCombination += 1;
                if (lApp || rApp) lockCombination += 3;
                if (lGrip || rGrip) lockCombination += 5;
            }
            //else if (rTrigger)
            //{
                //if (rA) lockCombination += 1;
                //if (rApp) lockCombination += 3;
                //if (rGrip) lockCombination += 5;
            //}
        }
        CmdSyncButtons(lTrigger, rTrigger, lA, rA, lApp, rApp, lGrip, rGrip, lJoystick, rJoystick, syncParameters.isPaused ); 
        CmdUpdateActions(bimanual, lockCombination);
    }

    public bool AnyButtonPressedLeft()
    {
        if (lTrigger || lA || lApp || lGrip)
            return true;
        return false;
    }

    public bool AnyButtonPressedRight()
    {
        if (rTrigger || rA || rApp || rGrip)
            return true;
        return false;
    }


    [Command]
    void CmdSyncButtons(bool ltrigger, bool rtrigger, bool la, bool ra, bool lapp, bool rapp, bool lgrip, bool rgrip, Vector2 ljoystick, Vector2 rjoystick, bool isPaused)
    {
        lTrigger = ltrigger;
        rTrigger = rtrigger;
        lA = la;
        rA = ra;
        lApp = lapp;
        rApp = rapp;
        lGrip = lgrip;
        rGrip = rgrip;
        lJoystick = ljoystick;
        rJoystick = rjoystick;

        if((AnyButtonPressedLeft() || AnyButtonPressedRight()) && !isPaused)
            this.gameObject.GetComponent<PlayerStuff>().activeTime += Time.deltaTime;

        RpcSyncButtons(ltrigger, rtrigger, la, ra, lapp, rapp, lgrip, rgrip, ljoystick, rjoystick);
    }

    [ClientRpc]
    void RpcSyncButtons(bool ltrigger, bool rtrigger, bool la, bool ra, bool lapp, bool rapp, bool lgrip, bool rgrip, Vector2 ljoystick, Vector2 rjoystick)
    {
        lTrigger = ltrigger;
        rTrigger = rtrigger;
        lA = la;
        rA = ra;
        lApp = lapp;
        rApp = rapp;
        lGrip = lgrip;
        rGrip = rgrip;
        lJoystick = ljoystick;
        rJoystick = rjoystick;

    }

    [Command]
    void CmdUpdateActions(bool biman, int lockcomb)
    {
        bimanual = biman;
        lockCombination = lockcomb;
        RpcUpdateActions(biman, lockcomb);
    }

    [ClientRpc]
    void RpcUpdateActions(bool biman, int lockcomb)
    {
        bimanual = biman;
        lockCombination = lockcomb;
    }
}
