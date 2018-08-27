using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class Log
{

    StreamWriter fUsersActions;
    List<int> playerOrder = new List<int>();

    public Log(int group, int condition, List<GameObject> players)
    {

        Debug.Log(Application.persistentDataPath);
        fUsersActions = File.CreateText(Application.persistentDataPath + "/Group-" + group + "-Condition-" + condition + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Verbose.csv");
        players.Sort((x, y) => x.GetComponent<PlayerStuff>().id.CompareTo(y.GetComponent<PlayerStuff>().id)); // sort players by ID 
        playerOrder.Clear();
        foreach (var p in players)
            playerOrder.Add(p.GetComponent<PlayerStuff>().id);

        string header = "Time;ObjID;TError;RError;RAError;SError";
        foreach (var p in players)
        {
            if (p.GetComponent<PlayerStuff>().type == Utils.PlayerType.VR)
                header += ";PID;PModal;PCamPosX;PCamPosY;PCamPosZ;PCamRotX;P1CamRotY;P1CamRotZ;P1CamRotW;TX;TY;TZ;RX;RY;RZ;RW;Scale;Bimanual;LockCombo";
            else if(p.GetComponent<PlayerStuff>().type == Utils.PlayerType.AR)
                header += ";PID;PModal;PCamPosX;PCamPosY;PCamPosZ;PCamRotX;P1CamRotY;P1CamRotZ;P1CamRotW;TX;TY;TZ;RX;RY;RZ;RW;Scale;CurrentOperation;NOthing"; // last element to match the number of columns
        }

        fUsersActions.WriteLine(header);
    }

    public void Close()
    {
        fUsersActions.Close();
    }

    public void Flush()
    {
        fUsersActions.Flush();
    }

    public void Save(int objId, float tError, float rError, float raError, float sError, List<GameObject> players)
    {
        //if(players.Count > 1) // only sort if there is more than one player
        //    players.Sort((x, y) => x.GetComponent<PlayerStuff>().id.CompareTo(y.GetComponent<PlayerStuff>().id)); // sort players by ID 

        String line = "";
        line += Time.realtimeSinceStartup + ";" + objId + ";" + tError + ";" + rError + ";" + raError + ";" + sError;

        foreach (var c in playerOrder)
        {
            var playerFound = ContainsId(c, players);
            if (playerFound != null)
            {
                var pStuff = playerFound.GetComponent<PlayerStuff>();
                var pTransformStep = playerFound.GetComponent<HandleNetworkTransformations>();
                Vector3 pPos = new Vector3();
                Quaternion pRot = new Quaternion();
                bool vrBimanual = false;
                int vrLockCombo = 0;
                int arOperation = 0;
                if (pStuff.type == Utils.PlayerType.VR)
                {
                    var pTransform = playerFound.GetComponent<VRTransformSync>();
                    var pButton = playerFound.GetComponent<ButtonSync>();
                    pPos = pTransform.headPos;
                    pRot = pTransform.headRot;
                    vrBimanual = pButton.bimanual;
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

                line += ";" + pStuff.id + ";" + pStuff.type;
                line += ";" + pPos.x + ";" + pPos.y + ";" + pPos.z + ";" + pRot.x + ";" + pRot.y + ";" + pRot.z + ";" + pRot.w;
                line += ";" + pTransformStep.tStep.x + ";" + pTransformStep.tStep.y + ";" + pTransformStep.tStep.z + ";" + pTransformStep.rStep.x + ";" + pTransformStep.rStep.y + ";" + pTransformStep.rStep.z + ";" + pTransformStep.rStep.w + ";" + pTransformStep.sStep;
                if (pStuff.type == Utils.PlayerType.VR)
                    line += ";" + vrBimanual + ";" + vrLockCombo;
                else if (pStuff.type == Utils.PlayerType.AR)
                    line += ";" + arOperation + ";";
            }
            else
            {
                line += ";;;;;;;;;;;;;;;;;;;";
            }
        }

        fUsersActions.WriteLine(line);
        fUsersActions.Flush();

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

    //public void saveUserActions(GameObject[] gs)
    //{
    //    String line = "";
    //    line += Time.realtimeSinceStartup + "";

    //    foreach (GameObject player in gs)
    //    {
    //        if (player.GetComponent<NetworkIdentity>().isLocalPlayer) continue;
    //        line += ";" + player.GetComponent<PlayerStuff>().userID;
    //        Vector3 cam = player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().CameraPosition;
    //        line += ";" + cam.x + ";" + cam.y + ";" + cam.z;
    //        if (player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Count <= 0) line += ";;;;;;;;;;;;";
    //        else
    //        {
    //            line += ";" + player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared[0];
    //            line += ";" + player.GetComponent<Lean.Touch.NetHandleTransformations>().modality;
    //            line += ";" + player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().currentOperation;
    //            line += ";" + player.GetComponent<PlayerStuff>().targetsTracked;
    //            Vector3 trans = player.GetComponent<HandleNetworkFunctions>().objTranslateStep;
    //            Quaternion rot = player.GetComponent<HandleNetworkFunctions>().objRotStep;
    //            line += ";" + trans.x + ";" + trans.y + ";" + trans.z;
    //            line += ";" + rot.x + ";" + rot.y + ";" + rot.z + ";" + rot.w;
    //            line += ";" + player.GetComponent<HandleNetworkFunctions>().objScaleStep;
    //        }
    //    }
    //    fUsersActions.WriteLine(line);
    //    fUsersActions.Flush();
    //}


    //public void savePiecesState(List<int> pieces, List<float> timers, List<float> errorTrans, List<float> errorRot, List<float> errorScale)
    //{
    //    String line = "";
    //    line += Time.realtimeSinceStartup + "";

    //    foreach (int piece in pieces)
    //        line += ";" + timers[piece] + ";" + errorTrans[piece] + ";" + errorRot[piece] + ";" + errorScale[piece];

    //    fPiecesState.WriteLine(line);
    //    fPiecesState.Flush();

    //}

    //public void saveResumed(int piece, float time, float timeU1, float timeU2)
    //{
    //    String line = "";
    //    line += Time.realtimeSinceStartup + "";
    //    line += ";" + piece + ";" + time + ";" + timeU1 + ";" + timeU2;
    //    fResumed.WriteLine(line);
    //    fResumed.Flush();
    //}


}

