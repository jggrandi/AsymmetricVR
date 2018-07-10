using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class HandleLocalTransformations : MonoBehaviour
    {

        public GameObject logic;
        GetTransformStep step;
        HandleControllersButtons handleButtons;
        Hand hand;

        // Use this for initialization
        void Start()
        {
            step = gameObject.transform.parent.GetComponent<GetTransformStep>();
            handleButtons = gameObject.transform.parent.GetComponent<HandleControllersButtons>();
            hand = gameObject.transform.parent.GetComponent<Hand>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            
            if (handleButtons.GetADown())
                handleButtons.ToogleA();
            if (handleButtons.GetAppDown())
                handleButtons.ToogleApp();

            if (handleButtons.GetToogleA())
            {
                //ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_A, " Translate ON"); //memory leak...
                logic.transform.position += step.positionStep;
            }
           // else
                //ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_A, "Translate OFF");

            if (handleButtons.GetToogleApp())
            {
                //ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_ApplicationMenu, "Rotation ON");
                logic.transform.rotation = step.rotationStep * logic.transform.rotation;
            }
           // else
                //ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_ApplicationMenu, "Rotation OFF");

        }
       
    }
}

