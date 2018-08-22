using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DockController : NetworkBehaviour {

    const float toleranceTrans = 0.05f;
    const float toleranceRot = 8.0f;
    const float toleranceScale = 0.03f;

    SyncTestParameters syncParamRef;
    
    GameObject interactableObjects;
    GameObject ghostObjects;
    HandleTestParameters testParameters;

    public List<float> errorTrans = new List<float>();
    public List<float> errorRot = new List<float>();
    public List<float> errorRotAngle = new List<float>();
    public List<float> errorScale = new List<float>();

    // Use this for initialization
    void Start () {
        if (!isServer) return;

        ResetErrorDocking();

        GameObject mainHandler = GameObject.Find("MainHandler");
        if (mainHandler == null) return;
        syncParamRef = mainHandler.GetComponent<SyncTestParameters>();

        testParameters = this.gameObject.GetComponent<HandleTestParameters>();

        interactableObjects = GameObject.Find("InteractableObjects");
        if (interactableObjects == null) return;

        ghostObjects = GameObject.Find("GhostObjects");
        if (ghostObjects == null) return;

    }
	
	// Update is called once per frame
	void Update () {
        if (!isServer) return;

        DeactivateAllObjects(interactableObjects);
        DeactivateAllObjects(ghostObjects);
        ActivateObject(syncParamRef.trialIndex, interactableObjects);
        ActivateObject(syncParamRef.trialIndex, ghostObjects);

        CalculateDocking();
        bool isGoodEnough = EvaluateCurrentDocking();
        if (isGoodEnough)
        {
            syncParamRef.trialIndex++;
        }
    }

    void ResetErrorDocking()
    {
        for (int i = 0; i < HandleTestParameters.qntTrials + HandleTestParameters.qntTraining; i++)
        {
            errorTrans.Add(Mathf.Infinity);
            errorRot.Add(Mathf.Infinity);
            errorRotAngle.Add(Mathf.Infinity);
            errorScale.Add(Mathf.Infinity);
        }
    }

    void CalculateDocking()
    {
        Transform movingObject = interactableObjects.transform.GetChild(syncParamRef.trialIndex);
        Transform staticObject = ghostObjects.transform.GetChild(syncParamRef.trialIndex);

        Matrix4x4 movingMatrixTrans = Matrix4x4.TRS(movingObject.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        Matrix4x4 movingMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), movingObject.rotation, new Vector3(1.0f, 1.0f, 1.0f));
        float movingScale = movingObject.localScale.x;

        Matrix4x4 staticMatrixTrans = Matrix4x4.TRS(staticObject.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        Matrix4x4 staticMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), staticObject.rotation, new Vector3(1.0f, 1.0f, 1.0f));
        float staticScale = staticObject.localScale.x;

        errorTrans[syncParamRef.trialIndex] = Utils.distMatrices(movingMatrixTrans, staticMatrixTrans);
        errorRot[syncParamRef.trialIndex] = Utils.distMatrices(movingMatrixRot, staticMatrixRot);
        errorRotAngle[syncParamRef.trialIndex] = Quaternion.Angle(movingObject.rotation, staticObject.rotation);
        errorScale[syncParamRef.trialIndex] = Mathf.Abs(movingScale - staticScale);

    }

    bool EvaluateCurrentDocking()
    {
        var tIndex = syncParamRef.trialIndex;
        if (errorTrans[tIndex] < toleranceTrans && errorRotAngle[tIndex] < toleranceRot && errorScale[tIndex] < toleranceScale)
            return true;
        return false;
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

}
