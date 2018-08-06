using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HandleControllersButtons : MonoBehaviour
{
    bool toogleAActive = true;
    bool toogleAppActive = true;

    public GameObject playerObject;
    public string handType;
    public static HandleControllersButtons handleButtons;
    Hand hand = null;

    // Use this for initialization
    void Start()
    {
        hand = gameObject.GetComponent<Hand>();
        handleButtons = this;
    }
    private void Awake()
    {

    }

    private void Update()
    {
       // Debug.Log(hand.GuessCurrentHandType().ToString());
    }

    public bool GetADown()
    {
        if (hand.controller.GetPressDown(EVRButtonId.k_EButton_A))
            return true;
        return false;
    }

    public bool GetAPress()
    {
        if (hand.controller.GetPress(EVRButtonId.k_EButton_A))
        {
            if (string.Compare(handType, "left") == 0)
                playerObject.GetComponent<ButtonSync>().lA = true;
            else if (string.Compare(handType, "right") == 0)
                playerObject.GetComponent<ButtonSync>().rA = true;

            return true;
        }
        if (string.Compare(handType, "left") == 0)
            playerObject.GetComponent<ButtonSync>().lA = false;
        else if (string.Compare(handType, "right") == 0)
            playerObject.GetComponent<ButtonSync>().rA = false;


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
        if (hand.controller.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu))
            return true;
        return false;
    }

    public bool GetAppPress()
    {
        if (hand.controller.GetPress(EVRButtonId.k_EButton_ApplicationMenu))
        {
            if (string.Compare(handType, "left") == 0)
                playerObject.GetComponent<ButtonSync>().lApp = true;
            else if (string.Compare(handType, "right") == 0)
                playerObject.GetComponent<ButtonSync>().rApp = true;

            return true;
        }
        if (string.Compare(handType, "left") == 0)
            playerObject.GetComponent<ButtonSync>().lApp = false;
        else if (string.Compare(handType, "right") == 0)
            playerObject.GetComponent<ButtonSync>().rApp = false;


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

    public bool GetGrip()
    {
        if (hand.controller.GetPress(EVRButtonId.k_EButton_Grip))
        {
            if (string.Compare(handType, "left") == 0)
                playerObject.GetComponent<ButtonSync>().lGrip = true;
            else if (string.Compare(handType, "right") == 0)
                playerObject.GetComponent<ButtonSync>().rGrip = true;

            return true;
        }
        if (string.Compare(handType, "left") == 0)
            playerObject.GetComponent<ButtonSync>().lGrip = false;
        else if (string.Compare(handType, "right") == 0)
            playerObject.GetComponent<ButtonSync>().rGrip = false;


        return false;
    }

    public bool GetGripDown()
    {
        if (hand.controller.GetPressDown(EVRButtonId.k_EButton_Grip))
            return true;
        return false;
    }

    public bool GetTrigger()
    {
        if (hand.controller.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            if (string.Compare(handType, "left") == 0)
                playerObject.GetComponent<ButtonSync>().lTrigger = true;
            else if (string.Compare(handType, "right") == 0)
                playerObject.GetComponent<ButtonSync>().rTrigger = true;

            return true;
        }
        if (string.Compare(handType, "left") == 0)
            playerObject.GetComponent<ButtonSync>().lTrigger = false;
        else if (string.Compare(handType, "right") == 0)
            playerObject.GetComponent<ButtonSync>().rTrigger = false;


        return false;
    }

    public bool GetTriggerDown()
    {
        if (hand.controller.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger))
            return true;
        return false;
    }


}

