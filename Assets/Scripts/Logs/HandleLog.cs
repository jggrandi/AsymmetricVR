using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandleLog : NetworkBehaviour
{
    Log log;
    int countFrames = 0;
    public bool recording = false;
    public bool paused = false;

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
        if (!recording) return;
        if (paused) return;

        if(countFrames % 5 == 0)
        {
            var objId = syncParameters.trialIndex;
            log.Save(objId, dockParameters.errorTrans[objId], dockParameters.errorRot[objId], dockParameters.errorRotAngle[objId], dockParameters.errorScale[objId], testParameters.activeInScene);
            //SAVE LOG
        }

    }

    public void StartLogRecording()
    {
        if (recording) return;
        log = new Log(testParameters.groupID, syncParameters.conditionIndex, 2);
        syncParameters.serverReady = true;
        recording = true;
        paused = false;
        startLogRecording.GetComponent<Image>().color = Color.grey;
    }

    public void StopLogRecording()
    {
        if (recording == false) return;
        syncParameters.serverReady = false;
        PauseLogRecording();
        recording = false;
        paused = false;
        log.Close();
        startLogRecording.GetComponent<Image>().color = Color.white;
    }

    public void PauseLogRecording()
    {
        if (!recording) return;
        if (paused)
        {
            paused = false;
            pauseRecord.GetComponentInChildren<Text>().text = "Pause";
            pauseRecord.GetComponent<Image>().color = Color.white;
        }
        else
        {
            paused = true;
            pauseRecord.GetComponentInChildren<Text>().text = "Resume";
            pauseRecord.GetComponent<Image>().color = Color.grey;
        }
    }

    void OnApplicationQuit()
    {
        log.Close();

    }
}
