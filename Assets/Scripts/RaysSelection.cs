using System.Collections;
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
            var icons = player.transform.GetChild(3); // it is the fourth because the other 3 are the head and the controllers 
            

            DisableIcons(icons); // disable all icons. Only show when an action is performed.

            Color color = greyColor; // other players' ray are grey
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
                color = blueColor; // localplayer's ray is blue

            if (buttonSync.bimanual) // the rays drawn are different from one hand.
            {
                AddLine(leftController.transform.position, rightController.transform.position, color); // line between controllers
                var controllersCenter = (leftController.transform.position - rightController.transform.position);
                controllersCenter = controllersCenter.normalized * (controllersCenter.magnitude / -2f) + leftController.transform.position;
                if (player.GetComponent<NetworkIdentity>().isLocalPlayer) // this player
                {
                    UpdateIconsPosition(icons, controllersCenter, Vector3.zero);
                 
                }
                //Icons(icons,controllersCenter, Vector3.zero, 0f); // add the action icons in the center of the two controllers
                else //other players
                {
                    AddLine(controllersCenter, ObjectManager.Get(selected).transform.position, color); // line from the center of the controllers to the object // line from the center of the controllers to the object
                    UpdateIconsPosition(icons, controllersCenter, ObjectManager.Get(selected).transform.position);
                }
                ShowIcons(buttonSync, icons); // add icons on the ray that hits the object



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


    void UpdateIconsPosition(Transform icons, Vector3 firstPos, Vector3 secondPos)
    {
        float offset = -0.1f;
        for(int i = 0; i < icons.childCount; i++)
        {
            var icon = icons.GetChild(i);
            icon.position = firstPos * (0.3f + offset) + secondPos * (0.7f - offset);
            icon.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), (Camera.main.transform.position - icon.position).normalized);
            icon.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            offset += 0.1f;
        }
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

    //void AddIcon(Transform icon, string transform, Vector3 firstPos, Vector3 secondPos)
    //{
    //    icon.gameObject.SetActive(true);
    //    if (string.Compare(icon.gameObject.name, "trans") == 0)
    //        icon.position = firstPos * (0.3f + offset) + secondPos * (0.7f - offset);
    //    if (string.Compare(icon.gameObject.name, "rotate") == 0)
    //        icon.position = firstPos * (0.3f + offset) + secondPos * (0.7f - offset);
    //    if (string.Compare(icon.gameObject.name, "scale") == 0)
    //        icon.position = firstPos * (0.3f + offset) + secondPos * (0.7f - offset);


    //    icon.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), (Camera.main.transform.position - icon.position).normalized);
    //    icon.localScale = new Vector3(0.01f, 0.01f, 0.01f);


    //}

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

    void DisableIcons(Transform icons)
    {
        for (int i = 0; i < icons.transform.childCount; i++)
            icons.transform.GetChild(i).gameObject.SetActive(false);
    }

}
