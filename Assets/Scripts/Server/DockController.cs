using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DockController : NetworkBehaviour {

    //original
    //const float toleranceTrans = 0.05f;
    //const float toleranceRot = 8.0f;
    //const float toleranceScale = 0.03f;

    const float toleranceTrans = 12f;
    const float toleranceRot = 190.0f;
    const float toleranceScale = 1f;


    SyncTestParameters syncParamRef;
    
    GameObject interactableObjects; // interactable and ghosts must have the same number of elements
    GameObject ghostObjects;
    GameObject spawnInfo;
    HandleTestParameters testParameters;

    public List<float> errorTrans = new List<float>();
    public List<float> errorRot = new List<float>();
    public List<float> errorRotAngle = new List<float>();
    public List<float> errorScale = new List<float>();

    // Use this for initialization
    void Start () {
        if (!isServer) return;

        GameObject mainHandler = GameObject.Find("MainHandler");
        if (mainHandler == null) return;
        syncParamRef = mainHandler.GetComponent<SyncTestParameters>();

        interactableObjects = GameObject.Find("InteractableObjects");
        if (interactableObjects == null) return;

        ghostObjects = GameObject.Find("GhostObjects");
        if (ghostObjects == null) return;

        testParameters = this.gameObject.GetComponent<HandleTestParameters>();

        ResetErrorDocking();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isServer) return;
        if (testParameters.allConditionsCompleted) return;
        CalculateDocking();
        bool isGoodEnough = EvaluateCurrentDocking();
        if (isGoodEnough)
        {
            var nextTrial = syncParamRef.trialIndex + 1;
            testParameters.TrialCompleted(nextTrial);
            //StartCoroutine("CoolDown");
            //Time.timeScale = 1;
            //if (syncParamRef.restTime) return;


            //VERIFY IF IT IS THE LAST TRIAL, AND IF THERE IS TRIALS THAT WERE NOT COMPLETED

        }
    }

    IEnumerator CoolDown()
    {
        syncParamRef.restTime = true;
        yield return new WaitForSeconds(5);
        syncParamRef.restTime = false;

        yield return null;
    }

        public void ResetErrorDocking()
    {
        errorTrans.Clear();
        errorRot.Clear();
        errorScale.Clear();

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
        Transform movingObject = interactableObjects.transform.GetChild(syncParamRef.activeTrialOrder[syncParamRef.trialIndex]);
        Transform staticObject = ghostObjects.transform.GetChild(syncParamRef.activeTrialOrder[syncParamRef.trialIndex]);

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

}
