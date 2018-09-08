using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;


public class ButtonSync : MonoBehaviour {


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

    public Utils.Hand whichHand = Utils.Hand.None; // 0=none; 1=left; 2=right; 3=bimanual
    public int lockCombination = 0; // 0=alltransforms, 1=trans, 3=rot, 5=scale, 4=trans+rot, 6=trans+scale, 8=rot+scale, 9=allblocked

    Player player;
    public Hand leftHand, rightHand;
    HandleControllersButtons refLeft, refRight;

    // Use this for initialization
    void Start() {

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

        whichHand = Utils.Hand.None;
        lockCombination = 0;

        if (AnyButtonPressedLeft() && AnyButtonPressedRight())
            whichHand = Utils.Hand.Bimanual;
        else if (AnyButtonPressedLeft())
            whichHand = Utils.Hand.Left ;
        else if (AnyButtonPressedRight())
            whichHand = Utils.Hand.Right;

        //if ((lTrigger && rTrigger) || (lA && rA) || (lApp && rApp) || (lGrip && rGrip))
        //    bimanual = true;

        //if (!(AnyButtonPressedLeft() || AnyButtonPressedRight()))
        //    return;

        if (whichHand == Utils.Hand.Bimanual)
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

}
