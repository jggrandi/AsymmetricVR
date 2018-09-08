using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class Log
{

    StreamWriter logFull;
    StreamWriter logResumed;
    List<int> playerOrder = new List<int>();

    public Log(int uId, int condition)
    {
        Debug.Log(Application.persistentDataPath);
        logFull = File.CreateText(Application.persistentDataPath + "/User-" + uId + "-Condition-" + condition + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Verbose.csv");
        logResumed = File.CreateText(Application.persistentDataPath + "/User-" + uId + "-Condition-" + condition + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Resumed.csv");

        string header = "Time;ObjID;TError;RError;RAError;SError";
        header += ";PID;Modal;CamPosX;CamPosY;CamPosZ;CamRotX;CamRotY;CamRotZ;CamRotW;TX;TY;TZ;RX;RY;RZ;RW;Scale;Hand;LockCombo";

        logFull.WriteLine(header);

        header = "ObjID;Time";
        header += ";PID;PieceTime";

        logResumed.WriteLine(header);

    }

    public void Close()
    {
        logFull.Close();
        logResumed.Close();
    }

    public void Flush()
    {
        logFull.Flush();
        logResumed.Flush();
    }

    public void SaveFull(int objId, float tError, float rError, float raError, float sError, GameObject player)
    {
        String line = "";

        var pStuff = player.GetComponent<PlayerStuff>();

        var pTransformStep = player.GetComponent<VRInteractionManager>();
        Vector3 pPos = new Vector3();
        Quaternion pRot = new Quaternion();
        Utils.Hand vrBimanual = Utils.Hand.None;
        int vrLockCombo = 0;

        var pTransform = player.GetComponent<VRTransformSync>(); // NEED IT BACK
        var pButton = player.GetComponent<ButtonSync>();
        pPos = pTransform.headPos;
        pRot = pTransform.headRot;
        vrBimanual = pButton.whichHand;
        vrLockCombo = pButton.lockCombination;

        line += Time.realtimeSinceStartup + ";" + objId + ";" + tError + ";" + rError + ";" + raError + ";" + sError;

        line += ";" + pStuff.id + ";" + pStuff.type;
        line += ";" + pPos.x + ";" + pPos.y + ";" + pPos.z + ";" + pRot.x + ";" + pRot.y + ";" + pRot.z + ";" + pRot.w;
        line += ";" + pTransformStep.tStep.x + ";" + pTransformStep.tStep.y + ";" + pTransformStep.tStep.z + ";" + pTransformStep.rStep.x + ";" + pTransformStep.rStep.y + ";" + pTransformStep.rStep.z + ";" + pTransformStep.rStep.w + ";" + pTransformStep.sStep;
        line += ";" + vrBimanual + ";" + vrLockCombo;

        logFull.WriteLine(line);
        logFull.Flush();

    }

    public void SaveResumed(int objId, float time, GameObject player)
    {

        var pStuff = player.GetComponent<PlayerStuff>();

        String line = "";
        line += objId + ";" + time;
        line += ";" + pStuff.id + ";" + pStuff.type + ";" + pStuff.activeTime;

        logResumed.WriteLine(line);
        logResumed.Flush();
    }

}

