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

            var head = player.transform.GetChild(0); // if the order changes in the prefab, it is necessary to update these indexes
            var leftController = player.transform.GetChild(1);
            var rightController = player.transform.GetChild(2);
            var iconsLeftHand = leftController.transform.GetChild(2); // it is the thrird because the other 2 are the controllers (a sphere and a cube)
            var iconsRightHand = rightController.transform.GetChild(2); // it is the thrird because the other 2 are the controllers (a sphere and a cube)

            DisableIcons(iconsLeftHand); // disable all icons. Only show when an action is performed.
            DisableIcons(iconsRightHand); // disable all icons. Only show when an action is performed.

            Color color = greyColor; // other players' ray are grey
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
                color = blueColor; // localplayer's ray is blue


            if (buttonSync.bimanual) // the rays drawn are different from one hand.
            {
                AddLine(leftController.transform.position, rightController.transform.position, color); // line between controllers
                var controllersCenter = (leftController.transform.position - rightController.transform.position);
                controllersCenter = controllersCenter.normalized * (controllersCenter.magnitude / -2f) + leftController.transform.position;
                AddLine(controllersCenter, ObjectManager.Get(selected).transform.position, color); // line from the center of the controllers to the object
                AddIcon(iconsLeftHand.transform.GetChild(3), controllersCenter, selected, 0.1f);

                ////if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) // add icons only for other player's actions
                //// {
                //if (buttonSync.lockedRot)
                //        AddIcon(iconsLeftHand.transform.GetChild(1), controllersCenter, selected, 0f);
                //    else if (buttonSync.lockedTrans)
                //        AddIcon(iconsLeftHand.transform.GetChild(2), controllersCenter, selected, 0f);
                //    else
                //        AddIcon(iconsLeftHand.transform.GetChild(0), controllersCenter, selected, 0f);
                //    if (buttonSync.scale) // scale icon is added apart
                //        AddIcon(iconsLeftHand.transform.GetChild(3), controllersCenter, selected, 0.1f);

                ////}

            }
            //else // only one hand is interacting
            //{
            //    if(buttonSync.lTrigger) // draw a ray directly from the hand to the object
            //        AddLine(leftController.transform.position, ObjectManager.Get(selected).transform.position, color);
            //    else if (buttonSync.rTrigger)
            //        AddLine(rightController.transform.position, ObjectManager.Get(selected).transform.position, color);

            //}

            //if (buttonSync.lTrigger)
            //{
            //    if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) // add icons only for other player's actions
            //    {
            //        if (buttonSync.lockedRot)
            //            AddIcon(iconsLeftHand.transform.GetChild(1), leftController, selected, 0f);
            //        else if (buttonSync.lockedTrans)
            //            AddIcon(iconsLeftHand.transform.GetChild(2), leftController, selected, 0f);
            //        else
            //            AddIcon(iconsLeftHand.transform.GetChild(0), leftController, selected, 0f);
            //        if (buttonSync.scale) // scale icon is added apart
            //            AddIcon(iconsLeftHand.transform.GetChild(3), leftController, selected, 0.1f);

            //    }
            //    AddLine(leftController.transform.position, ObjectManager.Get(selected).transform.position, color);
            //}
            //if (buttonSync.rTrigger)
            //{
            //    if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) // add icons only for other player's actions
            //    {
            //        if (buttonSync.lockedRot)
            //            AddIcon(iconsRightHand.transform.GetChild(1), rightController, selected, 0f);
            //        else if (buttonSync.lockedTrans)
            //            AddIcon(iconsRightHand.transform.GetChild(2), rightController, selected, 0f);
            //        else
            //            AddIcon(iconsRightHand.transform.GetChild(0), rightController, selected, 0f);
            //        if (buttonSync.scale) // scale icon is added apart
            //            AddIcon(iconsRightHand.transform.GetChild(3), rightController, selected, 0.1f);
            //    }
            //    AddLine(rightController.transform.position, ObjectManager.Get(selected).transform.position, color);
            //}

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


    void AddIcon(Transform icon, Vector3 initialPos, int indexObjSelected, float offset)
    {
        icon.gameObject.SetActive(true);
        var obj = ObjectManager.Get(indexObjSelected);
        //var pos = obj.GetComponent<Renderer>().bounds.extents.sqrMagnitude;
        //Debug.Log(pos);
        icon.position = initialPos * (0.3f+offset) + obj.transform.position * (0.7f-offset) ;
        //icon.position = (controller.transform.position * 0.3f) + ((obj.transform.position - new Vector3(pos, pos, pos)) * 0.7f);
        icon.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), (Camera.main.transform.position - icon.position).normalized);
        icon.localScale = new Vector3(0.01f, 0.01f, 0.01f);
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

    void DisableIcons(Transform handIcons)
    {
        for (int i = 0; i < handIcons.transform.childCount; i++)
            handIcons.transform.GetChild(i).gameObject.SetActive(false);
    }

}
