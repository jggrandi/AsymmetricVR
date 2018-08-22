﻿using UnityEngine;
using System.Collections;

public class ControlPanelToogle : MonoBehaviour
{
    private Canvas CanvasObject; // Assign in inspector

    void Start()
    {
        CanvasObject = GetComponent<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            CanvasObject.enabled = !CanvasObject.enabled;
        }
    }
}
