using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncTestParameters : NetworkBehaviour {

    //[SyncVar]// (hook = "OnConditionChange")] // hooks dont syncronize in server, so it is not useful here.
    //public int activeCondition;

    [SyncVar (hook = "UpdateSelected")]  //NEED TO SETSELECTED WHEN ACTIVETRIAL CHANGES
    public int activeTrial;

    [SyncVar]
    public bool EVALUATIONSTARTED = false;

    GameObject interactableObjects;
    GameObject ghostObjects;

    public void SYNC()
    {
        if (interactableObjects == null) interactableObjects = ObjectManager.manager.allInteractable;
        bool isghost = false;
        for (int i = 0; i < interactableObjects.transform.childCount; i++)
            SyncObj(i, isghost);

        isghost = true;
        if (ghostObjects == null) ghostObjects = ObjectManager.manager.allGhosts;
        for (int i = 0; i < ghostObjects.transform.childCount; i++)
            SyncObj(i, isghost);
    }

    public void SyncObj(int index,bool isghost, bool pos = true, bool rot = true, bool scale = true)
    {
        Vector3 p = Vector3.zero;
        Quaternion r = new Quaternion(0, 0, 0, 0);
        Vector3 s = Vector3.zero;

        GameObject g = null;
        if (isghost) g = ObjectManager.GetGhost(index);
        else g = ObjectManager.Get(index);

        if (pos) p = g.transform.position;
        if (rot) r = g.transform.rotation;
        if (scale) s = g.transform.localScale;
        RpcSyncObj(index,isghost, p, r, s);
    }

    [ClientRpc]
    public void RpcSyncObj(int index, bool isghost, Vector3 pos, Quaternion rot, Vector3 scale)
    {

        GameObject g = null;
        if (isghost) g = ObjectManager.GetGhost(index);
        else g = ObjectManager.Get(index);

        if (pos != Vector3.zero) g.transform.position = pos;
        if (rot != new Quaternion(0, 0, 0, 0)) g.transform.rotation = rot;
        if (scale != Vector3.zero) g.transform.localScale = scale;
    }

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
