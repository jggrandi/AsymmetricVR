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
    DockController dockController;
    SpawnInformation spawnInfo;
    HandleLog handleLog;

    public List<GameObject> activeInScene;

    GameObject interactableObjects;
    GameObject ghostObjects;

    string[] conditionName = { "VR-VR", "AR-AR", "VR-AR" };

    public List<int> activeTrialOrder = new List<int>();

    public List<Vector3> spawnPos = new List<Vector3>();
    public List<Quaternion> spawnRot = new List<Quaternion>();
    public List<float> spawnScale = new List<float>();

    public int conditionIndex;
    public int trialIndex;
    public int ghostOrder;

    public override void OnStartServer()
    {

        mainHandler = GameObject.Find("MainHandler");
        if (mainHandler == null) return;
        syncParameters = mainHandler.GetComponent<SyncTestParameters>();

        spawnInfo = this.gameObject.GetComponent<SpawnInformation>();
        dockController = this.gameObject.GetComponent<DockController>();
        handleLog = this.gameObject.GetComponent<HandleLog>();

        panelConnected = GameObject.Find("ClientsConnected");
        if (panelConnected == null) return;

        panelModality = GameObject.Find("PanelModality");
        if (panelModality == null) return;

        panelTrial = GameObject.Find("PanelTrial");
        if (panelTrial == null) return;

        GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text = "0";

        interactableObjects = ObjectManager.manager.allInteractable;
        if (interactableObjects == null) return;

        ghostObjects = ObjectManager.manager.allGhosts;
        if (ghostObjects == null) return;

        activeInScene = new List<GameObject>();

        UpdateParameters();

    }


    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;

        RetrieveAllPlayersConnected();

        if (activeInScene.Count == 0) //if there is no players connected , remove the text of all UI 
        {
            for (int i = 0; i < panelConnected.transform.childCount; i++)
                RemovePlayerOnDisplay(i);
            return; // dont need to do other things
        }

        DisplayPlayersInUI(activeInScene); // handle the display of the player's name on the UI
    }

    public void RetrieveAllPlayersConnected()
    {
        var arObjs = GameObject.FindGameObjectsWithTag("PlayerAR");
        var vrObjs = GameObject.FindGameObjectsWithTag("PlayerVR");
        activeInScene.Clear();
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
    }

    public void UpdateParameters()
    {
        groupID = int.Parse(GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text);

        int[] order = Utils.selectTaskSequence(groupID, conditionsToPermute);
        int[] ghostPermutations = Utils.selectTaskSequence(groupID, 2);
        ghostOrder = ghostPermutations[0]; 

        conditionsOrder.Clear();

        for (int i = 0; i < order.Length; i++)
            conditionsOrder.Add(order[i]);

        condition0TrialOrder = RandomizeTrialOrder(); //three because we have 3 conditions....
        condition1TrialOrder = RandomizeTrialOrder();
        condition2TrialOrder = RandomizeTrialOrder();
        conditionIndex = 0;

        ResetConditionsCompleted();
        ReorderConditionNames();
        UpdateConditionColor();
        handleLog.StopLogRecording();
        SetGhostManipulation(false);
        UpdateTrials();
        syncParameters.EVALUATIONSTARTED = false;
        syncParameters.TESTFINISHED = false;
    }

    public void OnClickUpdate()
    {
        UpdateParameters();
        syncParameters.SYNC();
    }

    void UpdateTrials()
    {
        RandomizeTrialsSpawnTransform();
        SetActiveTrialOrder();
        ResetTrialsCompleted();
        dockController.ResetErrorDocking();
        UpdateTrialColor();
        ResetContributionTime();
        SetObjectsTransform();
        SetGhostsTransform();
        SetGhostsTransform();
        SetCurrentTrialIndex(trialIndex);
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
        panelModality.transform.GetChild(conditionIndex).gameObject.GetComponentInChildren<Image>().color = Color.green;

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

        panelTrial.transform.GetChild(trialIndex).gameObject.GetComponentInChildren<Image>().color = Color.green;
    }

    void ResetSpawnOrder()
    {
        spawnPos.Clear();
        spawnRot.Clear();
        spawnScale.Clear();
    }

    void SetCurrentTrialIndex(int index)
    {
        trialIndex = index;
        syncParameters.activeTrial = activeTrialOrder[trialIndex];
        ObjectManager.SetSelected(syncParameters.activeTrial); // set only for the server. the sync var hook will set for clients
        if (ghostOrder == 0)
        {
            if (trialIndex >= qntTraining && trialIndex < qntTraining + (qntTrials / 2))
                SetGhostManipulation(false);
            else
                SetGhostManipulation(true);
        }
        else if (ghostOrder == 1)
        {
            if (trialIndex >= qntTraining  && trialIndex < qntTraining + (qntTrials / 2))
                SetGhostManipulation(true);
            else
                SetGhostManipulation(false);
        }

        if (trialIndex == 1) // if the training piece index is 1, player 1 train manipulating the ghost piece.
            SetGhostManipulation(true);

    }

    void SetGhostManipulation(bool _isghost)
    {
        GameObject g;
        g = activeInScene.Find(obj => obj.GetComponent<PlayerStuff>().id == 1);
        if (g == null) return;

        g.GetComponent<PlayerStuff>().isGhost = _isghost; 

        RpcSyncGhostManipulation(_isghost); //sync clients.
    }

    [ClientRpc]
    void RpcSyncGhostManipulation(bool isghost)
    {
        GameObject g;
        RetrieveAllPlayersConnected();
        g = activeInScene.Find(obj => obj.GetComponent<PlayerStuff>().id == 1);
        if (g == null) return;

        g.GetComponent<PlayerStuff>().isGhost = isghost;
    }

    void ResetTrialsCompleted()
    {
        SetCurrentTrialIndex(0);
        trialsCompleted.Clear();
        for (int i = 0; i < activeTrialOrder.Count; i++)
            trialsCompleted.Add(false);
        ClearTrialColor();
    }

    void ResetConditionsCompleted()
    {
        conditionIndex = 0;
        conditionsCompleted.Clear();
        for (int i = 0; i < conditionsToPermute; i++)
            conditionsCompleted.Add(false);
        ClearConditionColor();
    }

    public void TrialCompleted()
    {
        trialsCompleted[trialIndex] = true;
        handleLog.SaveResumed(syncParameters.activeTrial, Time.realtimeSinceStartup, activeInScene);
        ResetContributionTime();

        if (!trialsCompleted.Contains(false))
        {
            ConditionCompleted();
            return;
        }
        SetCurrentTrialIndex(trialsCompleted.IndexOf(false));
        UpdateTrialColor();
    }

    public void ConditionCompleted()
    {
        conditionsCompleted[conditionIndex] = true;

        if (!conditionsCompleted.Contains(false))
        {
            TestFinished();
            return;
        }

        conditionIndex = conditionsCompleted.IndexOf(false);

        GreyTrialCompleted();
        handleLog.StopLogRecording();
        
        UpdateConditionColor();
        UpdateTrials();
        SetGhostManipulation(false);
        syncParameters.SYNC();
    }

    public void TestFinished()
    {
        GreyConditionCompleted();
        GreyTrialCompleted();
        syncParameters.TESTFINISHED = true;
        handleLog.StopLogRecording();
    }

    public void OnClickCondition(int newIndex)
    {
        if (newIndex == conditionIndex) return;
        handleLog.StopLogRecording();
        ConditonChange(newIndex);
    }

    public void ConditonChange(int newIndex)
    {
        if (conditionsCompleted[newIndex] == true) return;
        
        conditionIndex = newIndex;
        UpdateConditionColor();
        UpdateTrials();
        SetGhostManipulation(false);
        syncParameters.SYNC();
    }

    public void OnClickTrial(int newIndex)
    {
        if (newIndex == trialIndex) return; // dont allow to select the same trial that is active
        if (conditionsCompleted[conditionIndex] == true) return; //dont allow to select trial if the condition was already finished
        SetObjectTransform(trialIndex); // the trial was not completed, so it is necessary to reset the position of this object
        SetGhostsTransform(); // and its ghost (in this case the function resets all ghost positions)
        TrialChange(newIndex); 
    }

    void TrialChange(int newIndex)
    {
        if (trialsCompleted[newIndex] == true) trialsCompleted[newIndex] = false;
        SetObjectTransform(activeTrialOrder[newIndex]);
        SetGhostsTransform();
        SetCurrentTrialIndex(newIndex);
        handleLog.previousTime = Time.realtimeSinceStartup;
        ResetContributionTime();
        UpdateTrialColor();
        syncParameters.SYNC();
    }

    public void OnClickPause()
    {
        if (syncParameters.TESTFINISHED) return;
        handleLog.StopLogRecording();
    }

    public void OnClickStartRecording()
    {
        if (syncParameters.TESTFINISHED) return;
        handleLog.StartLogRecording();
        syncParameters.SYNC();
    }

    void DisplayTrialOrder()
    {
        for (int i = 0; i < panelTrial.transform.childCount; i++)
            panelTrial.transform.GetChild(i).gameObject.GetComponentInChildren<Text>().text = activeTrialOrder[i].ToString();
    }

    List<int> RandomizeTrialOrder()
    {
        List<int> trainingPlusTrials = new List<int>() { 0, 1, 2 };
        List<int> randomizedList = Utils.randomizeVector(qntTraining, qntTraining + qntTrials); // the numbers start in 3 and finishes in 11. The # 0,1 and 2 are indexes for training;
        trainingPlusTrials.AddRange(randomizedList);
        return trainingPlusTrials;
    }

    void RandomizeTrialsSpawnTransform()
    {
        CreateTrialsTransform<Vector3>(spawnInfo.pos, spawnPos);
        CreateTrialsTransform<Quaternion>(spawnInfo.rot, spawnRot);
        CreateTrialsTransform<float>(spawnInfo.scale, spawnScale);
    }

    public void CreateTrialsTransform <T>(List<T> _transform, List<T> stored)
    {
        stored.Clear();
        var list = Utils.FitVectorIntoAnother(qntTrials + qntTraining, _transform.Count);
        list = Utils.RandomizeList(list);
        foreach (var item in list)
            stored.Add(_transform[item]);
    }

    public void SetObjectsTransform()
    {
        for (int i = 0; i < interactableObjects.transform.childCount; i++)
        {
            var obj = interactableObjects.transform.GetChild(i);
            obj.position = spawnPos[i];
            obj.rotation = spawnRot[i];
            var uniformScale = spawnScale[i];
            obj.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
        }
    }

    public void SetObjectTransform(int index)
    {
        var obj = interactableObjects.transform.GetChild(index);
        obj.position = spawnPos[index];
        obj.rotation = spawnRot[index];
        var uniformScale = spawnScale[index];
        obj.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
    }

    public void SetGhostsTransform()
    {
        var halfTrials = qntTrials / 2;
        var centerTable = new Vector3(spawnInfo.table.transform.position.x, 1.0f, spawnInfo.table.transform.position.z);

        for (int i = 0; i < qntTraining; i++)
        {
            var obj = ghostObjects.transform.GetChild(activeTrialOrder[i]);
            obj.position = centerTable;
            obj.rotation = Quaternion.identity;
        }

        if (ghostOrder == 0)
        {
            for (int i = qntTraining; i < halfTrials + qntTraining; i++)
                StaticGhostPositioning(i, centerTable);
            for (int i = qntTraining + halfTrials; i < ghostObjects.transform.childCount; i++)
                MovingGhostPositioning(i, centerTable);
        }
        else if (ghostOrder == 1)
        {
            for (int i = qntTraining; i < halfTrials + qntTraining; i++)
                MovingGhostPositioning(i, centerTable);
            for (int i = qntTraining + halfTrials; i < ghostObjects.transform.childCount; i++)
                StaticGhostPositioning(i, centerTable);
        }

        
        MovingGhostPositioning(1, centerTable);

    }

    public void StaticGhostPositioning(int i, Vector3 centertable)
    {
        var obj = ghostObjects.transform.GetChild(activeTrialOrder[i]);
        obj.position = centertable;
        obj.rotation = CreateGhostRotation(i);
    }

    public void MovingGhostPositioning(int i, Vector3 centertable)
    {
        var obj = ghostObjects.transform.GetChild(activeTrialOrder[i]);
        var intObj = interactableObjects.transform.GetChild(activeTrialOrder[i]);
        var centerPos = new Vector3(-(intObj.transform.position.x - centertable.x) + 0.2f, 1.0f, -(intObj.transform.position.z - centertable.z) - 0.2f);
        obj.position = centerPos;
        obj.rotation = CreateGhostRotation(i);
    }

    Quaternion CreateGhostRotation(int i)
    {
        float angle = 0f;
        Vector3 axis = new Vector3();
        spawnRot[i].ToAngleAxis(out angle, out axis);
        return Quaternion.AngleAxis(angle, -axis);
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
        switch (conditionIndex)
        {
            case 0:
                activeTrialOrder = condition0TrialOrder;
                break;
            case 1:
                activeTrialOrder = condition1TrialOrder;
                break;
            case 2:
                activeTrialOrder = condition2TrialOrder;
                break;
            default:
                break;
        }
        DisplayTrialOrder();
    }

    public void ResetContributionTime()
    {
        if (activeInScene.Count <= 0) return;
        foreach (var player in activeInScene)
            player.GetComponent<PlayerStuff>().activeTime = 0f;
    }

}
