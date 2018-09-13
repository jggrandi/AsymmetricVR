using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[NetworkSettings(channel = 1, sendInterval = 0.001f)]
public class VisualFeedback : MonoBehaviour {


    Color blueColor = new Color(0 / 255.0f, 118 / 255.0f, 255 / 255.0f);
    public GameObject lines;
    private Utils.PlayerType playerType;
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
        var obj = ObjectManager.GetSelected();
        if (obj == null) return;
        indexObjSelected = obj.index;
        linesUsed = 0;
        AddFeedbackVR();

        ClearLines();

    }

    void AddFeedbackVR()
    {

        var head = this.transform.GetChild(0); // if the order changes in the prefab, it is necessary to update these indexes
        var leftController = this.transform.GetChild(1);
        var rightController = this.transform.GetChild(2);
        var icons = this.transform.GetChild(3); // it is the fourth because the other 3 are the head and the controllers 

        DisableIcons(icons); // disable all icons. Only show when an action is performed.

        var vrTransform = this.GetComponent<VRTransformSync>();

        head.transform.position = Vector3.Lerp(head.transform.position, vrTransform.headPos, 0.4f); // set the virtual avatar pos and rot
        head.transform.rotation = vrTransform.headRot;
        leftController.transform.position = Vector3.Lerp(leftController.transform.position, vrTransform.leftHPos, 0.4f);
        leftController.transform.rotation = vrTransform.leftHRot;
        rightController.transform.position = Vector3.Lerp(rightController.transform.position, vrTransform.rightHPos, 0.4f);
        rightController.transform.rotation = vrTransform.rightHRot;

        //var selected = player.GetComponent<VisualFeedback>().objSelectedShared;
        if (indexObjSelected == -1) return;

        var buttonSync = this.GetComponent<ButtonSync>();
        if (buttonSync == null) return;

        Color color = blueColor; 

        var controllersCenter = Vector3.zero;
        if (buttonSync.whichHand == Utils.Hand.Bimanual) // the rays drawn are different from one hand.
        {
            AddLine(leftController.transform.position, rightController.transform.position, color); // line between controllers
            controllersCenter = (leftController.transform.position - rightController.transform.position);
            controllersCenter = controllersCenter.normalized * (controllersCenter.magnitude / -2f) + leftController.transform.position;
        }
        else
        {
            if (buttonSync.AnyButtonPressedLeft())
                controllersCenter = leftController.transform.position;
            else if (buttonSync.AnyButtonPressedRight())
                controllersCenter = rightController.transform.position;
        }


        if (buttonSync.whichHand == Utils.Hand.Bimanual || buttonSync.AnyButtonPressedLeft() || buttonSync.AnyButtonPressedRight()) //only show the line if user is interacting
        {
            GameObject obj = ObjectManager.Get(indexObjSelected);

            AddLine(controllersCenter, obj.transform.position, color); // line from the center of the controllers to the object

            UpdateIconsPosition(icons, controllersCenter);
            ShowIcons(buttonSync, icons); // add icons on the ray that hits the object

        }
        
    }

    void UpdateIconsPosition(Transform icons, Vector3 pos)
    {
        //icons.position = Vector3.Lerp(icons.position, pos, 0.3f);
        icons.position = pos;
        icons.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), (Camera.main.transform.position - icons.position).normalized);
    }

    void ShowIcons(ButtonSync bsync, Transform icons)
    {
        if (bsync.lTrigger || bsync.rTrigger || bsync.lockCombination == 9)
        {
            icons.GetChild(0).gameObject.SetActive(true);
            icons.GetChild(1).gameObject.SetActive(true);
            icons.GetChild(2).gameObject.SetActive(true);
        }

        switch (bsync.lockCombination)
        {
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

    public void ClearLines()
    {
        for (int i = linesUsed; i < lines.transform.childCount; i++)
        {
            var g = lines.transform.GetChild(i).gameObject;
            var line = g.GetComponent<VolumetricLines.VolumetricLineBehavior>();
            line.transform.position = new Vector3(500, 500, 500);
        }
        linesUsed = 0;
    }

    public void DisableIcons(Transform icons)
    {
        for (int i = 0; i < icons.transform.childCount; i++)
            icons.transform.GetChild(i).gameObject.SetActive(false);
    }

}
