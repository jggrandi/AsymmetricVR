using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DockController : MonoBehaviour {

    ////hard
    //const float toleranceTrans = 0.005f;
    //const float toleranceRot = 1.0f;
    //const float toleranceScale = 0.003f;

    //midhard
    const float toleranceTrans = 0.01f; // 1cm
    const float toleranceRot = 3.0f; // 3 degrees
    const float toleranceScale = 0.01f; // 1cm


    //medium
    //const float toleranceTrans = 0.05f;
    //const float toleranceRot = 5.0f;
    //const float toleranceScale = 0.03f;

    //const float toleranceTrans = 4f;
    //const float toleranceRot = 120.0f;
    //const float toleranceScale = 1f;

    //const float toleranceTrans = 12f;
    //const float toleranceRot = 190.0f;
    //const float toleranceScale = 1f;


    SyncTestParameters syncParameters;
    
    GameObject interactableObjects; // interactable and ghosts must have the same number of elements
    GameObject ghostObjects;

    HandleTestParameters testParameters;

    public List<float> errorTrans = new List<float>();
    public List<float> errorRot = new List<float>();
    public List<float> errorRotAngle = new List<float>();
    public List<float> errorScale = new List<float>();

    // Use this for initialization
    void Start () {

        GameObject mainHandler = GameObject.Find("MainHandler");
        if (mainHandler == null) return;
        syncParameters = mainHandler.GetComponent<SyncTestParameters>();

        testParameters = this.gameObject.GetComponent<HandleTestParameters>();

        interactableObjects = ObjectManager.manager.allInteractable;
        if (interactableObjects == null) return;

        ghostObjects = ObjectManager.manager.allGhosts;
        if (ghostObjects == null) return;

        ResetErrorDocking();
    }
	
	// Update is called once per frame
	void Update () {
        if (!syncParameters.EVALUATIONSTARTED) return;

        CalculateDocking();
        bool isGoodEnough = EvaluateCurrentDocking();
        if (isGoodEnough)
                testParameters.TrialCompleted();
    }

    public void ResetErrorDocking()
    {
        errorTrans.Clear();
        errorRot.Clear();
        errorRotAngle.Clear();
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
        Transform movingObject = interactableObjects.transform.GetChild(syncParameters.activeTrial);
        Transform staticObject = ghostObjects.transform.GetChild(syncParameters.activeTrial);

        Matrix4x4 movingMatrixTrans = Matrix4x4.TRS(movingObject.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        Matrix4x4 movingMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), movingObject.rotation, new Vector3(1.0f, 1.0f, 1.0f));
        float movingScale = movingObject.localScale.x;

        Matrix4x4 staticMatrixTrans = Matrix4x4.TRS(staticObject.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        Matrix4x4 staticMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), staticObject.rotation, new Vector3(1.0f, 1.0f, 1.0f));
        float staticScale = staticObject.localScale.x;

        errorTrans[syncParameters.activeTrial] = Utils.distMatrices(movingMatrixTrans, staticMatrixTrans);
        errorRot[syncParameters.activeTrial] = Utils.distMatrices(movingMatrixRot, staticMatrixRot);
        errorRotAngle[syncParameters.activeTrial] = Quaternion.Angle(movingObject.rotation, staticObject.rotation);
        errorScale[syncParameters.activeTrial] = Mathf.Abs(movingScale - staticScale);

    }

    bool EvaluateCurrentDocking()
    {
        var tIndex = syncParameters.activeTrial;
        if (errorTrans[tIndex] < toleranceTrans && errorRotAngle[tIndex] < toleranceRot && errorScale[tIndex] < toleranceScale)
            return true;
        return false;
    }

}
