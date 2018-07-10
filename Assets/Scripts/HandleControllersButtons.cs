﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{

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
        private void Awake()
        {

        }

        //public bool GetBothControllersGraspPressed()
        //{
        //    leftHand = player.leftHand;
        //    rightHand = player.rightHand;
        //    if (leftHand == null || rightHand == null) return false;
        //    if (leftHand.controller.GetPress(EVRButtonId.k_EButton_Grip) && rightHand.controller.GetPress(EVRButtonId.k_EButton_Grip))
        //        return true;
        //    return false;
        //}

        public bool GetADown()
        {
            if (hand.controller.GetPressDown(EVRButtonId.k_EButton_A))
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
            if (hand.controller.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu))
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



    }
}
