using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandleLog : NetworkBehaviour
{
    Log log;
    int countFrames = 0;
    public bool confirmed = false;

    GameObject mainHandler;
    SyncTestParameters syncParameters;
    HandleTestParameters testParameters;
    DockController dockParameters;

    void Start () {
        if (!isServer) return;
        mainHandler = GameObject.Find("MainHandler");
        if (mainHandler == null) return;
        syncParameters = mainHandler.GetComponent<SyncTestParameters>();

        testParameters = this.gameObject.GetComponent<HandleTestParameters>();
        dockParameters = this.gameObject.GetComponent<DockController>();

 
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isServer) return;
        if (!confirmed) return;

        if(countFrames % 5 == 0)
        {
            var objId = syncParameters.trialIndex;
            log.Save(objId, dockParameters.errorTrans[objId], dockParameters.errorRot[objId], dockParameters.errorRotAngle[objId], dockParameters.errorScale[objId], testParameters.activeInScene);
            //SAVE LOG
        }

    }

    public void ConfirmInitialParameters()
    {
        if (confirmed) return;
        log = new Log(testParameters.groupID, syncParameters.conditionIndex, 2);
        confirmed = true;
    }

    void OnApplicationQuit()
    {
        log.Close();

    }
}
