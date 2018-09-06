using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandleLog : NetworkBehaviour
{
    Log log;
    int countFrames = 0;
    public bool isRecording = false;
    public bool isPaused = false;
    public float timePaused = 0f;

    public float previousTime = 0f;
    float timeWhenStartPause = 0f;

    GameObject mainHandler;
    SyncTestParameters syncParameters;
    HandleTestParameters testParameters;
    DockController dockParameters;

    GameObject startLogRecording;
    GameObject pauseRecord;

    void Start () {
        if (!isServer) return;
        mainHandler = GameObject.Find("MainHandler");
        if (mainHandler == null) return;
        syncParameters = mainHandler.GetComponent<SyncTestParameters>();

        startLogRecording = GameObject.Find("Recording");
        if (startLogRecording == null) return;

        pauseRecord = GameObject.Find("PauseRecord");
        if (pauseRecord == null) return;

        testParameters = this.gameObject.GetComponent<HandleTestParameters>();
        dockParameters = this.gameObject.GetComponent<DockController>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isServer) return;
        if (!isRecording) return;
        if (isPaused) return;

        RecordActiveTime();

        if (countFrames % 5 == 0)
        {
            var objId = syncParameters.activeTrial;
            log.SaveFull(objId, dockParameters.errorTrans[objId], dockParameters.errorRot[objId], dockParameters.errorRotAngle[objId], dockParameters.errorScale[objId], testParameters.activeInScene);
        }
    }

    public void StartLogRecording()
    {
        if (isRecording) return;
        log = new Log(testParameters.groupID, testParameters.conditionsOrder[testParameters.conditionIndex], testParameters.activeInScene);
        syncParameters.EVALUATIONSTARTED = true;
        isRecording = true;
        isPaused = false;
        SetPauseButtonColor();
        SetRecordingButtonColor();
        
    }

    public void StopLogRecording()
    {
        if (!isRecording) return;
        syncParameters.EVALUATIONSTARTED = false;
        previousTime = 0f;
        isRecording = false;
        isPaused = false;
        SetPauseButtonColor();
        SetRecordingButtonColor();
        log.Close();
    }

    public void PauseLogRecording()
    {
        if (isPaused)
        {
            isPaused = false;
            timePaused += (Time.realtimeSinceStartup - timeWhenStartPause);
            SetPauseButtonColor();
        }
        else
        {
            isPaused = true;
            timeWhenStartPause = Time.realtimeSinceStartup;
            SetPauseButtonColor();
        }
    }

    public void SetRecordingButtonColor()
    {
        if(isRecording)
            startLogRecording.GetComponent<Image>().color = Color.grey;
        else
            startLogRecording.GetComponent<Image>().color = Color.white;
    }

    public void SetPauseButtonColor()
    {
        if (isPaused)
        {
            pauseRecord.GetComponentInChildren<Text>().text = "Resume";
            pauseRecord.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            pauseRecord.GetComponentInChildren<Text>().text = "Pause";
            pauseRecord.GetComponent<Image>().color = Color.white;
        }
    }

    public void SaveResumed(int objId, float time, List<GameObject> players)
    {
        if (!isRecording) return;
        if (isPaused) return;
        var timeWithoutPause = time - timePaused;
        var objTime = timeWithoutPause - previousTime;

        log.SaveResumed(objId, objTime, players);
        previousTime = timeWithoutPause;
    }

    public void RecordActiveTime()
    {
        foreach (var player in testParameters.activeInScene)
        {
            var pStuff = player.GetComponent<PlayerStuff>();
            if(pStuff.type == Utils.PlayerType.AR)
            {
                var arInteraction = player.GetComponent<Lean.Touch.ARInteractionManager>();
                if(arInteraction.currentOperation > 0)
                    pStuff.activeTime += Time.deltaTime;
            }
            else if (pStuff.type == Utils.PlayerType.VR)
            {
                var bSync = player.GetComponent<ButtonSync>();
                if (bSync.AnyButtonPressedLeft() || bSync.AnyButtonPressedRight())
                    pStuff.activeTime += Time.deltaTime;
            }
        }
    }

    void OnApplicationQuit()
    {
        //TODO: NEED TO VERIFY IF LOG WAS CREATED BEFORE CLOSING
        log.Close();

    }
}
