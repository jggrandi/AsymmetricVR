using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncTestParameters : NetworkBehaviour {

    public SyncListInt activeTrialOrder = new SyncListInt();

    public SyncListInt spawnPosition = new SyncListInt();
    public SyncListInt spawnRotation = new SyncListInt();
    public SyncListInt spawnScale = new SyncListInt();

    [SyncVar]
    public int trialIndex;

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

        UpdateGhostPos();
        UpdateSpawnInfo();

    }
	
	// Update is called once per frame
	void Update () {
        //if (!isClient) return;
        DeactivateAllObjects(interactableObjects);
        DeactivateAllObjects(ghostObjects);
        ActivateObject(trialIndex, interactableObjects);
        ActivateObject(trialIndex, ghostObjects);

        ObjectManager.SetSelected(activeTrialOrder[trialIndex]);
    }

    void UpdateSpawnInfo()
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

    void UpdateGhostPos()
    {
        for(int i=0; i < ghostObjects.transform.childCount; i++)
        {
            var obj = ghostObjects.transform.GetChild(i);
            obj.transform.position = spawnInfo.ghostPos;
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
