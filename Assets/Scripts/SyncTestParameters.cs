using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTestParameters : MonoBehaviour {



    public int activeTrial;

    public bool EVALUATIONSTARTED = false;
    public bool TESTFINISHED = false;

    GameObject interactableObjects;
    GameObject ghostObjects;

    private void Start()
    {
        interactableObjects = ObjectManager.manager.allInteractable;
        if (interactableObjects == null) return;

        ghostObjects = ObjectManager.manager.allGhosts;
        if (ghostObjects == null) return;

        UpdateSelected(activeTrial);

    }

    void Update () {

        DeactivateAllObjects(interactableObjects);
        DeactivateAllObjects(ghostObjects);

        if (!EVALUATIONSTARTED) return; // dont do an

        UpdateSelected(activeTrial);
        ActivateObject(activeTrial, interactableObjects);
        ActivateObject(activeTrial, ghostObjects);
    }

    void DeactivateAllObjects(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
            parent.transform.GetChild(i).gameObject.SetActive(false);
    }

    void ActivateObject(int index, GameObject parent)
    {
        if (index > parent.transform.childCount) return;
        parent.transform.GetChild(index).gameObject.SetActive(true);
    }

    void UpdateSelected( int trial)
    {
        activeTrial = trial;
        ObjectManager.SetSelected(activeTrial);
    }

}
