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

    // Use this for initialization
    void Start () {
        //if (!isClient) return;
        interactableObjects = GameObject.Find("InteractableObjects");
        if (interactableObjects == null) return;

        ghostObjects = GameObject.Find("GhostObjects");
        if (ghostObjects == null) return;


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
