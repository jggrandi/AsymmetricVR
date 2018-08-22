using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandleTestParameters : NetworkBehaviour
{
    public const int qntTraining = 3;
    public const int conditionsToPermute = 3;
    public const int qntTrials = 8;
    public int groupID = 0;
    public int modalityIndex = 0;
    public int trialIndex = 0;

    List<int> activeTrialOrder = new List<int>();
    public List<int> trialsCompleted = new List<int>();

    GameObject panelConnected;
    GameObject panelModality;
    GameObject panelTrial;

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

        panelTrial = GameObject.Find("PanelTrial");
        if (panelTrial == null) return;

        GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text = "0";

        UpdateParameters();

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


    public void UpdateParameters()
    {
        groupID = int.Parse(GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text);

        int[] order = Utils.selectTaskSequence(groupID, conditionsToPermute);
        syncParameters.conditionsOrder.Clear();

        for (int i = 0; i < order.Length; i++)
            syncParameters.conditionsOrder.Add(order[i]);

        RandomizeTrialOrder(syncParameters.condition0TrialOrder); //three because we have 3 conditions....
        RandomizeTrialOrder(syncParameters.condition1TrialOrder);
        RandomizeTrialOrder(syncParameters.condition2TrialOrder);

        modalityIndex = 0;

        ResetTrialsCompleted();
        SetActiveTrialOrder();
        ReorderConditionNames();
        UpdateConditionColor();
        UpdateTrialColor();

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

    void UpdateConditionColor()
    {
        ClearConditionColor();
        panelModality.transform.GetChild(modalityIndex).gameObject.GetComponentInChildren<Image>().color = Color.grey;

    }

    void ClearTrialColor()
    {
        for (int i = 0; i < panelTrial.transform.childCount; i++)
            panelTrial.transform.GetChild(i).gameObject.GetComponentInChildren<Image>().color = Color.white;
    }


    void UpdateTrialColor()
    {
        ClearTrialColor();
        panelTrial.transform.GetChild(trialIndex).gameObject.GetComponentInChildren<Image>().color = Color.green;
        for (int i = 0; i < trialsCompleted.Count; i++)
            panelTrial.transform.GetChild(trialsCompleted[i]).gameObject.GetComponentInChildren<Image>().color = Color.grey;
    }

    void ResetTrialsCompleted()
    {
        trialIndex = 0;
        trialsCompleted.Clear();
        ClearTrialColor();
    }

    public void OnClickCondition(int newIndex)
    {
        if (newIndex == modalityIndex) return;
        ResetTrialsCompleted();
        UpdateTrialColor();
        SetActiveTrialOrder();
        modalityIndex = newIndex;
        UpdateConditionColor();
    }

    public void OnClickTrial(int newIndex)
    {
        if (newIndex == trialIndex) return;
        UpdateTrialsCompleted(newIndex);
        trialIndex = newIndex;
        UpdateTrialColor();
    }

    void UpdateTrialsCompleted(int newIndex) {
        if (trialsCompleted.Contains(newIndex)) trialsCompleted.Remove(newIndex);
        trialsCompleted.Add(trialIndex);
    }
    
    void DisplayTrialOrder()
    {
        for (int i = 0; i < panelTrial.transform.childCount; i++)
            panelTrial.transform.GetChild(i).gameObject.GetComponentInChildren<Text>().text = activeTrialOrder[i].ToString();
    }

    void RandomizeTrialOrder(SyncListInt _trials)
    {
        if (_trials.Count != 0)
            _trials.Clear();

        List<int> trainingPlusTrials = new List<int>() { 0, 1, 2 };
        List<int> randomizedList = Utils.randomizeVector(qntTraining, qntTraining+ qntTrials); // the numbers start in 3 and finishes in 11. The # 0,1 and 2 are indexes for training;
        trainingPlusTrials.AddRange(randomizedList);
        ListToSyncList(ref trainingPlusTrials, ref _trials);
    }

    public void ListToSyncList(ref List<int> list, ref SyncListInt syncList)
    {
        syncList.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            syncList.Add(list[i]);
        }
    }

    public void SyncListToList(ref SyncListInt syncList, ref List<int> list)
    {
        list.Clear();
        for (int i = 0; i < syncList.Count; i++)
        {
            list.Add(syncList[i]);
        }
    }

    void SetActiveTrialOrder()
    {
        switch (modalityIndex)
        {
            case 0:
                SyncListToList(ref syncParameters.condition0TrialOrder, ref activeTrialOrder);
                break;
            case 1:
                SyncListToList(ref syncParameters.condition1TrialOrder, ref activeTrialOrder);
                break;
            case 2:
                SyncListToList(ref syncParameters.condition2TrialOrder, ref activeTrialOrder);
                break;
            default:
                break;
        }
        DisplayTrialOrder();
    }

}
