using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncTestParameters : NetworkBehaviour {

    public SyncListInt activeTrialOrder = new SyncListInt();

    public List<int> spawnPosItem = new List<int>();
    public List<int> spawnRotItem = new List<int>();
    public List<int> spawnScaleItem = new List<int>();

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


    public override void OnStartLocalPlayer()
    {
        if (isServer) return;
            CmdSyncAll(); // sync all test parameters when connected
    }

    [Command]
    void CmdSyncAll()
    {
        if (interactableObjects == null) interactableObjects = ObjectManager.manager.allInteractable;
        bool isghost = false;
        for (int i = 0; i < interactableObjects.transform.childCount; i++)
            SyncAll(i,isghost);

        isghost = true;
        if (ghostObjects == null) ghostObjects = ObjectManager.manager.allGhosts;
        for (int i = 0; i < ghostObjects.transform.childCount; i++)
            SyncAll(i, isghost);

    }

    public void SyncAll(int index,bool isghost, bool pos = true, bool rot = true, bool scale = true)
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
        RpcSyncAll(index,isghost, p, r, s);
    }

    [ClientRpc]
    public void RpcSyncAll(int index, bool isghost, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        GameObject g = null;
        if (isghost) g = ObjectManager.GetGhost(index);
        else g = ObjectManager.Get(index);

        if (pos != Vector3.zero) g.transform.position = pos;
        if (rot != new Quaternion(0, 0, 0, 0)) g.transform.rotation = rot;
        if (scale != Vector3.zero) g.transform.localScale = scale;
    }

    public override void OnStartServer()
    {
        

        interactableObjects = ObjectManager.manager.allInteractable;
        if (interactableObjects == null) return;

        ghostObjects = ObjectManager.manager.allGhosts;
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
            obj.transform.position = spawnInfo.pos[spawnPosItem[i]];
            obj.transform.rotation = spawnInfo.rot[spawnRotItem[i]];
            var uniformScale = spawnInfo.scale[spawnScaleItem[i]];
            obj.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
        }
    }

    public void UpdateSpawnInfo(int index)
    {
        var obj = interactableObjects.transform.GetChild(index);
        obj.transform.position = spawnInfo.pos[spawnPosItem[index]];
        obj.transform.rotation = spawnInfo.rot[spawnRotItem[index]];
        var uniformScale = spawnInfo.scale[spawnScaleItem[index]];
        obj.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
    }

    [ClientRpc]
    public void RpcUpdateSpawnInfo()
    {
        UpdateSpawnInfo();
    }

    void UpdateGhost()
    {
        for (int i = 0; i < ghostObjects.transform.childCount; i++)
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
                var centerPos = new Vector3(-(intObj.transform.position.x - centerTable.x) + 0.2f, 1.0f, -(intObj.transform.position.z - centerTable.z) - 0.2f);
                obj.transform.position = centerPos;
                obj.transform.rotation = Quaternion.Euler(-spawnInfo.rot[spawnRotItem[i]].eulerAngles); //pega a rotação oposta
            }
        }
    }

    [ClientRpc]
    public void RpcUpdateGhost()
    {
        UpdateGhost();
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
