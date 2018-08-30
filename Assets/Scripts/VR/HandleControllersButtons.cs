using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HandleControllersButtons : MonoBehaviour
{
    bool toogleAActive = true;
    bool toogleAppActive = true;

    public static HandleControllersButtons handleButtons;
    Hand hand = null;

    // Use this for initialization
    void Start()
    {
        hand = gameObject.GetComponent<Hand>();
        handleButtons = this;
    }

    
    private void Update()
    {
        if (!isControllerActive()) return;
        //bool isShowingHint = !string.IsNullOrEmpty(ControllerButtonHints.GetActiveHintText(hand, EVRButtonId.k_EButton_SteamVR_Trigger)) || ControllerButtonHints.IsButtonHintActive(hand, EVRButtonId.k_EButton_SteamVR_Trigger);

        //if (!isShowingHint){
        //    ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger, "Grab");
        //    Debug.Log("1");
        //}
        //if (hand.controller.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger) ) { 
        //    ControllerButtonHints.ShowButtonHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
        //    ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
        //    Debug.Log("2");
        //}
        //if (hand.controller.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
        //{
        //    Debug.Log("3");
        //    ControllerButtonHints.HideButtonHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
        //    ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger, "Grab");
        //}

    }

    bool isControllerActive()
    {
        if (hand.controller != null)
            return true;
        return false;
    }

    public bool GetADown()
    {
        if (isControllerActive() && hand.controller.GetPressDown(EVRButtonId.k_EButton_A))
            return true;
        return false;
    }
    public bool GetARelease()
    {
        if (isControllerActive() && hand.controller.GetPressUp(EVRButtonId.k_EButton_A))
            return true;
        return false;
    }

    public bool GetAPress()
    {
        if (isControllerActive() && hand.controller.GetPress(EVRButtonId.k_EButton_A))
            return true;
        return false;
    }

    public bool ToogleA()
    {
        if (toogleAActive)
            toogleAActive = false;
        else
            toogleAActive = true;

        return toogleAActive;
    }

    public bool GetToogleA()
    {
        return toogleAActive;
    }

    public bool GetAppDown()
    {
        if (isControllerActive() && hand.controller.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu))
            return true;
        return false;
    }

    public bool GetAppPress()
    {
        if (isControllerActive() && hand.controller.GetPress(EVRButtonId.k_EButton_ApplicationMenu))
            return true;
        return false;
    }

    public bool ToogleApp()
    {
        if (toogleAppActive)
            toogleAppActive = false;
        else
            toogleAppActive = true;

        return toogleAppActive;
    }

    public bool GetToogleApp()
    {
        return toogleAppActive;
    }

    public bool GetGripPress()
    {
        if (isControllerActive() && hand.controller.GetPress(EVRButtonId.k_EButton_Grip))
            return true;
        return false;
    }

    public bool GetGripDown()
    {
        if (isControllerActive() && hand.controller.GetPressDown(EVRButtonId.k_EButton_Grip))
            return true;
        return false;
    }

    public bool GetTriggerPress()
    {
        if (isControllerActive() && hand.controller.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
            return true;
        return false;
    }

    public bool GetTriggerDown()
    {
        if (isControllerActive() && hand.controller.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger))
            return true;
        return false;
    }

    public Vector2 GetJoystickCoord()
    {
        if (!isControllerActive()) 
            return Vector2.zero;

        return hand.controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
    }




}

