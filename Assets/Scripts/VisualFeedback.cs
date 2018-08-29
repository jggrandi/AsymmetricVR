using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VisualFeedback : NetworkBehaviour {

    Color greyColor = new Color(150 / 255.0f, 150 / 255.0f, 150 / 255.0f);
    Color blueColor = new Color(0 / 255.0f, 118 / 255.0f, 255 / 255.0f);
    public GameObject lines;
    
    int linesUsed = 0;

    int indexObjSelected;
    // Use this for initialization
    void Start () {
        lines = GameObject.Find("Lines");
        ClearLines();
    }

    // Update is called once per frame
    void Update()
    {
        //if(!isServer && isClient)
        //    CmdSyncSelected();
        indexObjSelected = ObjectManager.GetSelected().index;
        linesUsed = 0;
        AddFeedbackVR();
        AddFeedbackAR();
        ClearLines();

    }

    void AddFeedbackVR()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerVR"))
        {
            var head = player.transform.GetChild(0); // if the order changes in the prefab, it is necessary to update these indexes
            var leftController = player.transform.GetChild(1);
            var rightController = player.transform.GetChild(2);
            var icons = player.transform.GetChild(3); // it is the fourth because the other 3 are the head and the controllers 

            var vrTransform = player.GetComponent<VRTransformSync>();
            head.transform.position = vrTransform.headPos; // set the virtual avatar pos and rot
            head.transform.rotation = vrTransform.headRot;
            leftController.transform.position = vrTransform.leftHPos;
            leftController.transform.rotation = vrTransform.leftHRot;
            rightController.transform.position = vrTransform.rightHPos;
            rightController.transform.rotation = vrTransform.rightHRot;

            //var selected = player.GetComponent<VisualFeedback>().objSelectedShared;
            if (indexObjSelected == -1) continue;

            var buttonSync = player.GetComponent<ButtonSync>();
            if (buttonSync == null) return;

            DisableIcons(icons); // disable all icons. Only show when an action is performed.

            Color color = greyColor; // other players' ray are grey
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
                color = blueColor; // localplayer's ray is blue

            var controllersCenter = Vector3.zero;
            if (buttonSync.bimanual) // the rays drawn are different from one hand.
            {
                AddLine(leftController.transform.position, rightController.transform.position, color); // line between controllers
                controllersCenter = (leftController.transform.position - rightController.transform.position);
                controllersCenter = controllersCenter.normalized * (controllersCenter.magnitude / -2f) + leftController.transform.position;
            }
            else
            {
                if (buttonSync.lTrigger)
                    controllersCenter = leftController.transform.position;
                else if (buttonSync.rTrigger)
                    controllersCenter = rightController.transform.position;
            }

            if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) // other player
            {
                if (buttonSync.bimanual || buttonSync.lTrigger || buttonSync.rTrigger) //only show the line if user is interacting
                {
                    AddLine(controllersCenter, ObjectManager.Get(indexObjSelected).transform.position, color); // line from the center of the controllers to the object
                    icons.position = controllersCenter * 0.7f + ObjectManager.Get(indexObjSelected).transform.position * 0.3f;
                    icons.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), (Camera.main.transform.position - icons.position).normalized);
                    if (vrTransform.isTranslating) icons.GetChild(0).gameObject.SetActive(true);
                    if (vrTransform.isRotating) icons.GetChild(1).gameObject.SetActive(true);
                    if (vrTransform.isScaling) icons.GetChild(2).gameObject.SetActive(true);
                }
            }
            else
            {
                UpdateIconsPosition(icons, controllersCenter);
                ShowIcons(buttonSync, icons); // add icons on the ray that hits the object
            }

        }
    }

    void AddFeedbackAR()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerAR"))
        {
            var tablet = player.transform.GetChild(0);
            var icons = player.transform.GetChild(1);

            //var selected = player.GetComponent<VisualFeedback>().;

            if (indexObjSelected == -1) continue;

            DisableIcons(icons);
            var arTransform = player.GetComponent<ARTransformSync>();

            tablet.transform.position = Vector3.Lerp(tablet.transform.position, arTransform.position, 0.01f);
            tablet.transform.rotation = Quaternion.Slerp(tablet.transform.rotation, arTransform.rotation, 0.01f);
            //tablet.transform.position = arTransform.position; // set the virtual tablet pos and rot
            //tablet.transform.rotation = arTransform.rotation;

            var rayAdjust = tablet.transform.position;

            Color color = greyColor; // other players' ray are grey
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                //rayAdjust = tablet.transform.position - tablet.transform.up.normalized * 0.4f + new Vector3(0.031f, 0.021f, 0.01f);
                color = blueColor; // localplayer's ray is blue
            }

            int operation = player.GetComponent<ARInteractionManager>().currentOperation;

            if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) // other player
                if(operation != (int)Utils.Transformations.None) //only show the line if user is interacting
                    AddLine(rayAdjust, ObjectManager.Get(indexObjSelected).transform.position, color); // add line 

            if (operation > 0 && !player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                var OperationObj = icons.transform.GetChild(operation - 1);
                OperationObj.gameObject.SetActive(true);
                OperationObj.position = rayAdjust * 0.7f + ObjectManager.Get(indexObjSelected).transform.position * 0.3f;

                OperationObj.rotation = Quaternion.LookRotation((Camera.main.transform.position - OperationObj.position).normalized, new Vector3(0, 1, 0));
                OperationObj.localRotation = OperationObj.localRotation * Quaternion.Euler(90, 0, 0);
            }


        }
    }

    void UpdateIconsPosition(Transform icons, Vector3 pos)
    {
        icons.position = pos;
        icons.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), (Camera.main.transform.position - icons.position).normalized);
    }

    void ShowIcons(ButtonSync bsync, Transform icons)
    {
        switch (bsync.lockCombination)
        {
            case 0: case 9:
                icons.GetChild(0).gameObject.SetActive(true);
                icons.GetChild(1).gameObject.SetActive(true);
                icons.GetChild(2).gameObject.SetActive(true);
                break;
            case 1:
                icons.GetChild(0).gameObject.SetActive(true);
                break;
            case 3:
                icons.GetChild(1).gameObject.SetActive(true);
                break;
            case 4:
                icons.GetChild(0).gameObject.SetActive(true);
                icons.GetChild(1).gameObject.SetActive(true);
                break;
            case 5:
                icons.GetChild(2).gameObject.SetActive(true);
                break;
            case 6:
                icons.GetChild(0).gameObject.SetActive(true);
                icons.GetChild(2).gameObject.SetActive(true);
                break;
            case 8:
                icons.GetChild(1).gameObject.SetActive(true);
                icons.GetChild(2).gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    void AddLine(Vector3 a, Vector3 b, Color c)
    {
        if(lines == null) lines = GameObject.Find("Lines");
        if (linesUsed >= lines.transform.childCount) return;
        var g = lines.transform.GetChild(linesUsed++).gameObject;
        var line = g.GetComponent<VolumetricLines.VolumetricLineBehavior>();
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
            line.transform.position = new Vector3(500, 500, 500);
        }
        linesUsed = 0;
    }

    void DisableIcons(Transform icons)
    {
        for (int i = 0; i < icons.transform.childCount; i++)
            icons.transform.GetChild(i).gameObject.SetActive(false);
    }

}
