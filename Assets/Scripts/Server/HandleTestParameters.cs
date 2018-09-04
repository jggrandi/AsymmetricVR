using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandleTestParameters : NetworkBehaviour
{
    public const int conditionsToPermute = 3;
    public const int qntTraining = 3;
    public const int qntTrials = 8; // it is the defauld, interactableObjects.transform.childCount in start can change this value
    public int groupID = 0;
    //public int conditionIndex = 0;

    public List<int> conditionsOrder = new List<int>();
    
    public List<bool> trialsCompleted = new List<bool>();
    public List<bool> conditionsCompleted = new List<bool>();

    public List<int> condition0TrialOrder = new List<int>();
    public List<int> condition1TrialOrder = new List<int>();
    public List<int> condition2TrialOrder = new List<int>();
    
    GameObject panelConnected;
    GameObject panelModality;
    GameObject panelTrial;

    GameObject mainHandler;
    SyncTestParameters syncParameters;
    NetworkManager netMan;
    DockController dockController;
    SpawnInformation spawnInfo;
    HandleLog handleLog;

    public List<GameObject> activeInScene;

    GameObject interactableObjects;

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
        spawnInfo = mainHandler.GetComponent<SpawnInformation>();

        activeInScene = new List<GameObject>();

        panelModality = GameObject.Find("PanelModality");
        if (panelModality == null) return;

        panelTrial = GameObject.Find("PanelTrial");
        if (panelTrial == null) return;

        interactableObjects = GameObject.Find("InteractableObjects");
        if (interactableObjects == null) return;
        //qntTrials = interactableObjects.transform.childCount; // set the number of trials depending on the number of elements child of interactableObjects

        dockController = this.gameObject.GetComponent<DockController>();
        handleLog = this.gameObject.GetComponent<HandleLog>();

        GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text = "0";

        UpdateParameters();

    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;

        var arObjs = GameObject.FindGameObjectsWithTag("PlayerAR");
        var vrObjs = GameObject.FindGameObjectsWithTag("PlayerVR");
        activeInScene.Clear();
        for (int i = 0; i < panelConnected.transform.childCount; i++)
            RemovePlayerOnDisplay(i);

        if (arObjs.Length > 0)
        {
            for (int i = 0; i < arObjs.Length; i++) // add all ar players connected
                activeInScene.Add(arObjs[i]); 
        }
        if (vrObjs.Length > 0)
        {
            for (int i = 0; i < vrObjs.Length; i++) //add all vr players connected
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

        DisplayPlayersInUI(activeInScene); // handle the display of the player's name on the UI
    }

    void DisplayPlayersInUI(List<GameObject> _bjs)
    {
        if(_bjs.Count > 1)
            _bjs.Sort((x, y) => x.GetComponent<PlayerStuff>().id.CompareTo(y.GetComponent<PlayerStuff>().id)); //sort

        for (int i = 0; i < _bjs.Count; i++)
            AddPlayerOnDisplay(i, _bjs[i]);
    }

    void AddPlayerOnDisplay(int index, GameObject g )
    {
        var slot = panelConnected.transform.GetChild(index);
        Text playerName = slot.GetComponent<Text>();
        var playerInfo = g.GetComponent<PlayerStuff>();
        playerName.text = playerInfo.type + " " + playerInfo.id;
    }

    void RemovePlayerOnDisplay(int index)
    {
        var slot = panelConnected.transform.GetChild(index);
        Text playerName = slot.GetComponent<Text>();
        playerName.text = "";
    }

    public void UpdateParameters()
    {
        groupID = int.Parse(GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text);

        int[] order = Utils.selectTaskSequence(groupID, conditionsToPermute);
        conditionsOrder.Clear();

        for (int i = 0; i < order.Length; i++)
            conditionsOrder.Add(order[i]);

        condition0TrialOrder = RandomizeTrialOrder(); //three because we have 3 conditions....
        condition1TrialOrder = RandomizeTrialOrder();
        condition2TrialOrder = RandomizeTrialOrder();
        syncParameters.conditionIndex = 0;

        ResetConditionsCompleted();
        ReorderConditionNames();
        UpdateConditionColor();
        handleLog.StopLogRecording();


        //syncParameters.RpcUpdateSpawnInfo(); //NEED TO CHECK THIS
        //syncParameters.RpcUpdateGhost();

        UpdateTrial();
        syncParameters.EVALUATIONSTARTED = false;
    }

    void UpdateTrial()
    {
        RandomizeTrialSpawn();
        SetActiveTrialOrder();
        ResetTrialsCompleted();
        dockController.ResetErrorDocking();
        UpdateTrialColor();
        handleLog.ResetContributionTime();
    }


    void ReorderConditionNames()
    {
        for (int i = 0; i < panelModality.transform.childCount; i++)
            panelModality.transform.GetChild(i).gameObject.GetComponentInChildren<Text>().text = conditionName[conditionsOrder[i]];
    }

    void ClearConditionColor()
    {
        for (int i = 0; i < panelModality.transform.childCount; i++)
            panelModality.transform.GetChild(i).gameObject.GetComponentInChildren<Image>().color = Color.white;
    }

    void UpdateConditionColor()
    {
        ClearConditionColor();
        GreyConditionCompleted();
        panelModality.transform.GetChild(syncParameters.conditionIndex).gameObject.GetComponentInChildren<Image>().color = Color.green;

    }
    void GreyConditionCompleted()
    {
        for (int i = 0; i < conditionsCompleted.Count; i++)
            if (conditionsCompleted[i])
                panelModality.transform.GetChild(i).gameObject.GetComponentInChildren<Image>().color = Color.grey;

    }

    void ClearTrialColor()
    {
        for (int i = 0; i < panelTrial.transform.childCount; i++)
            panelTrial.transform.GetChild(i).gameObject.GetComponentInChildren<Image>().color = Color.white;
    }

    void GreyTrialCompleted()
    {
        for (int i = 0; i < trialsCompleted.Count; i++)
        {
            if (trialsCompleted[i])
                panelTrial.transform.GetChild(i).gameObject.GetComponentInChildren<Image>().color = Color.gray;
        }
    }

    void UpdateTrialColor()
    {
        ClearTrialColor();
        GreyTrialCompleted();

        panelTrial.transform.GetChild(syncParameters.trialIndex).gameObject.GetComponentInChildren<Image>().color = Color.green;
    }

    void ResetSpawnOrder()
    {
        syncParameters.spawnPosItem.Clear();
        syncParameters.spawnRotItem.Clear();
        syncParameters.spawnScaleItem.Clear();
    }

    void ResetTrialsCompleted()
    {
        syncParameters.trialIndex = 0;
        trialsCompleted.Clear();
        for (int i = 0; i < syncParameters.activeTrialOrder.Count; i++)
            trialsCompleted.Add(false);
        ClearTrialColor();
    }

    void ResetConditionsCompleted()
    {
        syncParameters.conditionIndex = 0;
        conditionsCompleted.Clear();
        for (int i = 0; i < conditionsToPermute; i++)
            conditionsCompleted.Add(false);
        ClearConditionColor();

    }



    public void TrialCompleted()
    {
        trialsCompleted[syncParameters.trialIndex] = true;
        if (!trialsCompleted.Contains(false))
        {
            ConditionCompleted();
            return;
        }
        handleLog.SaveResumed(syncParameters.activeTrialOrder[syncParameters.trialIndex], Time.realtimeSinceStartup, activeInScene);
        handleLog.ResetContributionTime();
        syncParameters.trialIndex = trialsCompleted.IndexOf(false);
        UpdateTrialColor();
    }

    public void ConditionCompleted()
    {
        conditionsCompleted[syncParameters.conditionIndex] = true;

        if (!conditionsCompleted.Contains(false))
        {
            TestFinished();
            return;
        }

        syncParameters.conditionIndex = conditionsCompleted.IndexOf(false);

        GreyTrialCompleted();
        handleLog.StopLogRecording();
        
        UpdateConditionColor();
        UpdateTrial();
    }

    public void TestFinished()
    {
        GreyConditionCompleted();
        GreyTrialCompleted();
        handleLog.StopLogRecording();
    }

    public void OnClickCondition(int newIndex)
    {
        if (newIndex == syncParameters.conditionIndex) return;
        handleLog.StopLogRecording();
        ConditonChange(newIndex);
    }

    public void ConditonChange(int newIndex)
    {
        if (conditionsCompleted[newIndex] == true) return;
        
        syncParameters.conditionIndex = newIndex;
        UpdateConditionColor();
        UpdateTrial();

    }

    public void OnClickTrial(int newIndex)
    {
        if (newIndex == syncParameters.trialIndex) return;
        TrialChange(newIndex);
    }

    void TrialChange(int newIndex)
    {
        if (trialsCompleted[newIndex] == true) trialsCompleted[newIndex] = false;
        syncParameters.UpdateSpawnInfo(syncParameters.activeTrialOrder[newIndex]);
        syncParameters.trialIndex = newIndex;
        handleLog.previousTime = Time.realtimeSinceStartup;
        handleLog.ResetContributionTime();
        UpdateTrialColor();
    }


    public void OnClickStartRecording()
    {
        handleLog.StartLogRecording();
    }

    void DisplayTrialOrder()
    {
        for (int i = 0; i < panelTrial.transform.childCount; i++)
            panelTrial.transform.GetChild(i).gameObject.GetComponentInChildren<Text>().text = syncParameters.activeTrialOrder[i].ToString();
    }

    List<int> RandomizeTrialOrder()
    {
        List<int> trainingPlusTrials = new List<int>() { 0, 1, 2 };
        List<int> randomizedList = Utils.randomizeVector(qntTraining, qntTraining + qntTrials); // the numbers start in 3 and finishes in 11. The # 0,1 and 2 are indexes for training;
        trainingPlusTrials.AddRange(randomizedList);
        return trainingPlusTrials;
    }

    void RandomizeTrialSpawn()
    {
        RandomizeTrialSpawnTransform(syncParameters.spawnPosItem, spawnInfo.pos.Count);
        RandomizeTrialSpawnTransform(syncParameters.spawnRotItem, spawnInfo.rot.Count);
        RandomizeTrialSpawnTransform(syncParameters.spawnScaleItem, spawnInfo.scale.Count);
    }

    void RandomizeTrialSpawnTransform(SyncListInt _spawnT, int size)
    {
        if (_spawnT.Count != 0)
            _spawnT.Clear();

        var fullList = Utils.FitVectorIntoAnother(qntTrials + qntTraining, size);
        fullList = Utils.RandomizeList(fullList);
        ListToSyncList(ref fullList, ref _spawnT);
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
        switch (syncParameters.conditionIndex)
        {
            case 0:
                ListToSyncList(ref condition0TrialOrder, ref syncParameters.activeTrialOrder);
                break;
            case 1:
                ListToSyncList(ref condition1TrialOrder, ref syncParameters.activeTrialOrder);
                break;
            case 2:
                ListToSyncList(ref condition2TrialOrder, ref syncParameters.activeTrialOrder);
                break;
            default:
                break;
        }
        DisplayTrialOrder();
    }

}
