using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class HandleLocalTransformations : MonoBehaviour
    {

        public GameObject logic;
        GetTransformStep step;

        // Use this for initialization
        void Start()
        {
            step = gameObject.transform.parent.GetComponent<GetTransformStep>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {

            logic.transform.position += step.positionStep;
            logic.transform.rotation = step.rotationStep * logic.transform.rotation;
            var hand = this.transform.parent.GetComponent<Hand>();
            if (hand.controller.GetPressDown(EVRButtonId.k_EButton_A) && hand.otherHand.controller.GetPressDown(EVRButtonId.k_EButton_A)){ Debug.Log("Nothing yet"); }

        }
       
    }
}

