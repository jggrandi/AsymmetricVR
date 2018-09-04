using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncTestParameters : NetworkBehaviour {

    public SyncListInt activeTrialOrder = new SyncListInt();

    public SyncListInt spawnPosition = new SyncListInt();
    public SyncListInt spawnRotation = new SyncListInt();
    public SyncListInt spawnScale = new SyncListInt();

    [SyncVar]// (hook = "OnConditionChange")] // hooks dont syncronize in server, so it is not useful here.
    public int conditionIndex;
    public int prevConditionIndex;

    [SyncVar]// (hook = "OnTrialChanged")]
    public int trialIndex;
    public int prevTrialIndex;

    [SyncVar]
    public bool EVALUATIONSTARTED = false;

    [SyncVar]
    public bool isPaused = false;

    GameObject interactableObjects;
    GameObject ghostObjects;

    SpawnInformation spawnInfo;


    // Use this for initialization
    void Start () {
        //if (!isClient) return;
        
        interactableObjects = GameObject.Find("InteractableObjects");
        if (interactableObjects == null) return;

        ghostObjects = GameObject.Find("GhostObjects");
        if (ghostObjects == null) return;

        spawnInfo = this.gameObject.GetComponent<SpawnInformation>();

        prevConditionIndex = conditionIndex;
        prevTrialIndex = trialIndex;

        UpdateGhost();
        UpdateSpawnInfo();

        ObjectManager.SetSelected(activeTrialOrder[trialIndex]);

    }
	
	// Update is called once per frame
	void Update () {
        //if (!isClient) return;
        DeactivateAllObjects(interactableObjects);
        DeactivateAllObjects(ghostObjects);

        if (!EVALUATIONSTARTED) return; // dont do an


        ActivateObject(trialIndex, interactableObjects);
        ActivateObject(trialIndex, ghostObjects);

        if (prevConditionIndex != conditionIndex)
        {
            UpdateSpawnInfo();
            UpdateGhost();
            prevConditionIndex = conditionIndex;
        }

        if (prevTrialIndex != trialIndex)
        {
            UpdateGhost();
            ObjectManager.SetSelected(activeTrialOrder[trialIndex]);
            prevTrialIndex = trialIndex;
        }
    }

    public void UpdateSpawnInfo()
    {
        for (int i = 0; i < interactableObjects.transform.childCount; i++)
        {
            var obj = interactableObjects.transform.GetChild(i);
            obj.transform.position = spawnInfo.pos[spawnPosition[i]];
            obj.transform.rotation = spawnInfo.rot[spawnRotation[i]];
            var uniformScale = spawnInfo.scale[spawnScale[i]];
            obj.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
        }
    }

    public void UpdateSpawnInfo(int index)
    {
        var obj = interactableObjects.transform.GetChild(index);
        obj.transform.position = spawnInfo.pos[spawnPosition[index]];
        obj.transform.rotation = spawnInfo.rot[spawnRotation[index]];
        var uniformScale = spawnInfo.scale[spawnScale[index]];
        obj.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
    }


    void UpdateGhost()
    {
        for(int i = 0; i < ghostObjects.transform.childCount; i++)
        {
            var obj = ghostObjects.transform.GetChild(activeTrialOrder[i]);
            var centerTable = new Vector3(spawnInfo.table.transform.position.x, 1.0f, spawnInfo.table.transform.position.z);
            if (i < 7)
            {
                
                obj.transform.position = centerTable;
            }
            else
            {
                var intObj = interactableObjects.transform.GetChild(activeTrialOrder[i]);
                var centerPos = new Vector3 (-(intObj.transform.position.x - centerTable.x) + 0.2f, 1.0f, -(intObj.transform.position.z - centerTable.z) - 0.2f);
                obj.transform.position = centerPos;
                obj.transform.rotation = Random.rotationUniform;
            }
        }
    }


    void DeactivateAllObjects(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
            parent.transform.GetChild(i).gameObject.SetActive(false);
    }

    void ActivateObject(int index, GameObject parent)
    {
        if (index > parent.transform.childCount) return;
        parent.transform.GetChild(activeTrialOrder[index]).gameObject.SetActive(true);
    }

}
