using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandleTestParameters : NetworkBehaviour
{
    public int conditionsToPermute = 3;
    public int groupID = 0;
    public int modalityIndex = 0;

    GameObject panelConnected;
    GameObject panelModality;
    GameObject mainHandler;
    SyncTestParameters syncParameters;
    NetworkManager netMan;
    public List<GameObject> activeInScene;
    public List<GameObject> guiPlayer;

    string[] conditionName = { "VR-VR", "AR-AR", "VR-AR" };

    // Use this for initialization
    void Start()
    {
        if (!isServer) return;

        netMan = NetworkManager.singleton;
        panelConnected = GameObject.Find("ClientsConnected");
        if (panelConnected == null) return;
        mainHandler = GameObject.Find("MainHandler");
        if (mainHandler == null) return;
        syncParameters = mainHandler.GetComponent<SyncTestParameters>();

        guiPlayer = new List<GameObject>();
        for (int i = 0; i < 10; i++) guiPlayer.Add(null);
        activeInScene = new List<GameObject>();

        panelModality = GameObject.Find("PanelModality");
        if (panelModality == null) return;

        GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text = "0";
        UpdateGroupId();


    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;

        var arObjs = GameObject.FindGameObjectsWithTag("PlayerAR");
        var vrObjs = GameObject.FindGameObjectsWithTag("PlayerVR");

        if (arObjs.Length > 0)
        {
            for (int i = 0; i < arObjs.Length; i++) // add all ar players connected
                if(!activeInScene.Contains(arObjs[i]))
                    activeInScene.Add(arObjs[i]); 
        }
        if (vrObjs.Length > 0)
        {
            for (int i = 0; i < vrObjs.Length; i++) //add all vr players connected
                if (!activeInScene.Contains(vrObjs[i]))
                    activeInScene.Add(vrObjs[i]);
        }

        if (activeInScene.Count == 0) //if there is no players connected , remove the text of all UI 
        {
            for (int i = 0; i < panelConnected.transform.childCount; i++)
            {
                var slot = panelConnected.transform.GetChild(i);
                Text playerName = slot.GetComponent<Text>();
                playerName.text = "";
            }
            return; // dont need to do other things
        }

        HandleDisplayPlayer(activeInScene); // handle the display of the player's name on the UI
    }

    void HandleDisplayPlayer(List<GameObject> _bjs)
    {
        for (int i = 0; i < _bjs.Count; i++)
        {
            if (_bjs[i] == null) //if one player disconnected
            {
                _bjs.RemoveAt(i); //remove it
                RemovePlayerOnDisplay(i); //remove the ui text
            }
            else if (!guiPlayer.Contains(_bjs[i])) //if the player is connected but dont have a ui text
                AddPlayerOnDisplay(i, _bjs[i]);
        }
    }

    void AddPlayerOnDisplay(int index, GameObject g )
    {
        guiPlayer[index] = g;
        var slot = panelConnected.transform.GetChild(index);
        Text playerName = slot.GetComponent<Text>();
        playerName.text = g.name;
    }

    void RemovePlayerOnDisplay(int index)
    {
        guiPlayer.RemoveAt(index);
        var slot = panelConnected.transform.GetChild(index);
        Text playerName = slot.GetComponent<Text>();
        playerName.text = "";
    }


    public void UpdateGroupId()
    {
        groupID = int.Parse(GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text);

        int[] order = Utils.selectTaskSequence(groupID, conditionsToPermute);
        syncParameters.conditionsOrder.Clear();

        for (int i = 0; i < order.Length; i++)
            syncParameters.conditionsOrder.Add(order[i]);
        
        var oC = GameObject.Find("OrderConditions");//.GetComponent<InputField>().text = syncParameters.conditionsOrder[modalityIndex].ToString();
        for (int i = 0; i < oC.transform.childCount; i++)
            oC.transform.GetChild(i).gameObject.GetComponent<Text>().text = conditionName[i];

        modalityIndex = 0;
        ReorderConditionNames();
        UpdateSelectionColor();
        
    }

    void ReorderConditionNames()
    {
        for (int i = 0; i < panelModality.transform.childCount; i++)
            panelModality.transform.GetChild(i).gameObject.GetComponentInChildren<Text>().text = conditionName[syncParameters.conditionsOrder[i]];
    }

    void ClearConditionColor()
    {
        for (int i = 0; i < panelModality.transform.childCount; i++)
            panelModality.transform.GetChild(i).gameObject.GetComponentInChildren<Image>().color = Color.white;
    }

    void UpdateSelectionColor()
    {
        ClearConditionColor();
        panelModality.transform.GetChild(modalityIndex).gameObject.GetComponentInChildren<Image>().color = Color.grey;

    }

    public void OnClickVRVR()
    {
        modalityIndex = 0;
        UpdateSelectionColor();
    }

    public void OnClickARAR()
    {
        modalityIndex = 1;
        UpdateSelectionColor();
    }

    public void OnClickVRAR()
    {
        modalityIndex = 2;
        UpdateSelectionColor();
    }

    public void ButtonNextCondition()
    {
        if (modalityIndex < conditionsToPermute-1)
        {
            IncrementSceneID();
            GameObject.Find("InputModality").GetComponent<InputField>().text = syncParameters.conditionsOrder[modalityIndex].ToString();
            //UpdateScene();
        }
    }

    public void ButtonPreviousCondition()
    {
        if (modalityIndex > 0)
        {
            DecrementSceneID();
            GameObject.Find("InputModality").GetComponent<InputField>().text = syncParameters.conditionsOrder[modalityIndex].ToString();
           // UpdateScene();
        }
    }


    void IncrementSceneID()
    {
        modalityIndex++;
    }

    void DecrementSceneID()
    {
        modalityIndex--;
    }


    void UpdateScene()
    {
        modalityIndex = int.Parse(GameObject.Find("InputModality").GetComponent<InputField>().text);
    }


}
