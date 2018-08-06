﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RaysSelection : NetworkBehaviour {

    Color greyColor = new Color(150 / 255.0f, 150 / 255.0f, 150 / 255.0f);
    Color blueColor = new Color(0 / 255.0f, 118 / 255.0f, 255 / 255.0f);
    public GameObject lines;
    
    int linesUsed = 0;
    [SyncVar]
    public int objSelectedShared = -1; // the user selections visible by other players

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;
        
        lines = GameObject.Find("Lines");
        
        ClearLines();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        CmdSyncSelected();
        linesUsed = 0;
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            var selected = player.GetComponent<RaysSelection>().objSelectedShared;
            if (selected == -1) continue;

            var buttonSync = player.GetComponent<ButtonSync>();
            if (buttonSync == null) return;

            Color color = greyColor;

            var head = player.transform.GetChild(0); // if the order changes in the prefab, it is necessary to update these indexes
            var leftController = player.transform.GetChild(1);
            var rightController = player.transform.GetChild(2);
            var iconMoveLeft = leftController.transform.GetChild(2); // it is the thrird because the other 2 are the controllers (a sphere and a cube)
            var iconMoveRight = rightController.transform.GetChild(2); // it is the thrird because the other 2 are the controllers (a sphere and a cube)

            iconMoveLeft.gameObject.SetActive(false); // disable the icon. Only show when an action is performed.
            iconMoveRight.gameObject.SetActive(false); // disable the icon. Only show when an action is performed.

            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
                color = blueColor;

            if (buttonSync.leftInteractionButtonPressed)
            {
                if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) // add icons only for other player's actions
                    AddIcon(iconMoveLeft, leftController, selected);
                AddLine(leftController.transform.position, ObjectManager.Get(selected).transform.position, color);
            }
            if (buttonSync.rightInteractionButtonPressed)
            {
                if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) // add icons only for other player's actions
                    AddIcon(iconMoveRight, rightController, selected);
                AddLine(rightController.transform.position, ObjectManager.Get(selected).transform.position, color);
            }

        }
        ClearLines();
    }


    void AddSelected(int index)
    {
        objSelectedShared = index;
    }


    void ClearSelected()
    {
        objSelectedShared = -1;
    }

    [Command]
    public void CmdSyncSelected()
    {
        ClearSelected();
        if(ObjectManager.GetSelected() != null)
        AddSelected(ObjectManager.GetSelected().index);
    }


    void AddIcon(Transform icon, Transform controller, int indexObjSelected)
    {
        icon.gameObject.SetActive(true);
        icon.position = controller.transform.position * 0.3f + ObjectManager.Get(indexObjSelected).transform.position * 0.7f;
        icon.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), (Camera.main.transform.position - icon.position).normalized);
        icon.localScale = new Vector3(0.005f, 0.005f, 0.005f);
    }

    void AddLine(Vector3 a, Vector3 b, Color c)
    {
        if(lines == null) lines = GameObject.Find("Lines");
        if (linesUsed >= lines.transform.childCount) return;
        var g = lines.transform.GetChild(linesUsed++).gameObject;
        var line = g.GetComponent<VolumetricLines.VolumetricLineBehavior>();
        //line.SetStartAndEndPoints(a, b);
        line.transform.position = a;
        line.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), (b - a).normalized);
        line.transform.localScale = new Vector3(1f, 1f, (b - a).magnitude);
        line.LineWidth = 0.1f;
        line.LineColor = c;
    }

    void ClearLines()
    {
        for (int i = linesUsed; i < lines.transform.childCount; i++)
        {
            var g = lines.transform.GetChild(i).gameObject;
            var line = g.GetComponent<VolumetricLines.VolumetricLineBehavior>();
            //line.SetStartAndEndPoints(new Vector3(5000.0f, 5000.0f, 5000.0f), new Vector3(5000.0f, 5000.0f, 5000.0f));
            line.transform.position = new Vector3(500, 500, 500);
        }
        linesUsed = 0;
    }

}
