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

    public Log(int group, int condition, List<GameObject> players)
    {
        Debug.Log(Application.persistentDataPath);
        logFull = File.CreateText(Application.persistentDataPath + "/Group-" + group + "-Condition-" + condition + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Verbose.csv");
        logResumed = File.CreateText(Application.persistentDataPath + "/Group-" + group + "-Condition-" + condition + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Resumed.csv");

        players.Sort((x, y) => x.GetComponent<PlayerStuff>().id.CompareTo(y.GetComponent<PlayerStuff>().id)); // sort players by ID 
        playerOrder.Clear();

        foreach (var p in players)
            playerOrder.Add(p.GetComponent<PlayerStuff>().id);

        string header = "Time;ObjID;DiffObj;TError;RError;RAError;SError";
        foreach (var p in players)
        {
            if (p.GetComponent<PlayerStuff>().type == Utils.PlayerType.VR)
                header += ";PID;Modal;Ghost;CamPosX;CamPosY;CamPosZ;CamRotX;CamRotY;CamRotZ;CamRotW;TX;TY;TZ;RX;RY;RZ;RW;Scale;Bimanual;LockCombo";
            else if (p.GetComponent<PlayerStuff>().type == Utils.PlayerType.AR)
                header += ";PID;Modal;Ghost;CamPosX;CamPosY;CamPosZ;CamRotX;CamRotY;CamRotZ;CamRotW;TX;TY;TZ;RX;RY;RZ;RW;Scale;CurrentOperation;NOthing"; // last element to match the number of columns
        }

        logFull.WriteLine(header);

        header = "ObjID;Time;DiffObj";
        foreach (var p in players)
            header += ";PID;Modal;PieceTime";

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

    public void SaveFull(int objId, float tError, float rError, float raError, float sError, List<GameObject> players)
    {
        String line = "";
        String lineBuff = "";

        var differentObj = false;

        foreach (var c in playerOrder)
        {
            var playerFound = ContainsId(c, players);
            if (playerFound != null)
            {
                var pStuff = playerFound.GetComponent<PlayerStuff>();

                if (pStuff.isGhost)
                    differentObj = true;

                var pTransformStep = playerFound.GetComponent<HandleNetworkTransformations>();
                Vector3 pPos = new Vector3();
                Quaternion pRot = new Quaternion();
                Utils.Hand vrBimanual = Utils.Hand.None;
                int vrLockCombo = 0;
                int arOperation = 0;
                if (pStuff.type == Utils.PlayerType.VR)
                {
                    var pTransform = playerFound.GetComponent<VRTransformSync>();
                    var pButton = playerFound.GetComponent<ButtonSync>();
                    pPos = pTransform.headPos;
                    pRot = pTransform.headRot;
                    vrBimanual = pButton.whichHand;
                    vrLockCombo = pButton.lockCombination;
                }
                else if (pStuff.type == Utils.PlayerType.AR)
                {
                    var pTransform = playerFound.GetComponent<ARTransformSync>();
                    var pOperation = playerFound.GetComponent<Lean.Touch.ARInteractionManager>();
                    pPos = pTransform.position;
                    pRot = pTransform.rotation;
                    arOperation = pOperation.currentOperation;
                }

                lineBuff += ";" + pStuff.id + ";" + pStuff.type + ";" + pStuff.isGhost;
                lineBuff += ";" + pPos.x + ";" + pPos.y + ";" + pPos.z + ";" + pRot.x + ";" + pRot.y + ";" + pRot.z + ";" + pRot.w;
                lineBuff += ";" + pTransformStep.tStep.x + ";" + pTransformStep.tStep.y + ";" + pTransformStep.tStep.z + ";" + pTransformStep.rStep.x + ";" + pTransformStep.rStep.y + ";" + pTransformStep.rStep.z + ";" + pTransformStep.rStep.w + ";" + pTransformStep.sStep;
                if (pStuff.type == Utils.PlayerType.VR)
                    lineBuff += ";" + vrBimanual + ";" + vrLockCombo;
                else if (pStuff.type == Utils.PlayerType.AR)
                    lineBuff += ";" + arOperation + ";";
            }
            else
            {
                lineBuff += ";;;;;;;;;;;;;;;;;;;;";
            }
        }

        line += Time.realtimeSinceStartup + ";" + objId + ";" + differentObj + ";" + tError + ";" + rError + ";" + raError + ";" + sError;
        line += lineBuff;


        logFull.WriteLine(line);
        logFull.Flush();

    }

    public void SaveResumed(int objId, float time, List<GameObject> players)
    {
        String line = "";
        line += objId + ";" + time;

        var differentObj = false;
        String buffLines = "";
        foreach (var p in players)
        {
            var pStuff = p.GetComponent<PlayerStuff>();
            if (pStuff.isGhost)
                differentObj = true;
            buffLines += ";" + pStuff.id + ";" + pStuff.type + ";" + pStuff.activeTime;
        }

        line += ";" + differentObj;
        line += buffLines;

        logResumed.WriteLine(line);
        logResumed.Flush();
    }


    GameObject ContainsId(int _index, List<GameObject> _players)
    {
        foreach(var p in _players)
        {
            if (p == null) continue;
            if (p.GetComponent<PlayerStuff>().id == _index)
                return p;
        }
        return null;
    }

    


}

