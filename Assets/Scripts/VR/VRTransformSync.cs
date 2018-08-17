using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class VRTransformSync : NetworkBehaviour
{

    public Vector3 headPos;
    public Quaternion headRot;
    public Vector3 leftHPos;
    public Quaternion leftHRot;
    public Vector3 rightHPos;
    public Quaternion rightHRot;

    Player player;

    private void Start()
    {
        if (string.Compare(SceneManager.GetActiveScene().name, "SetupTest") == 0) return;

        if (!isLocalPlayer) return;

        player = Player.instance;

        if (player == null)
        {
            Debug.LogError("No Player instance found in map.");
            Destroy(this.gameObject);
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        if (player == null) player = Player.instance;

        headPos = player.hmdTransform.position;
        headRot = player.hmdTransform.rotation;


        leftHPos = player.leftHand.transform.position;
        leftHRot = player.leftHand.transform.rotation;

        rightHPos = player.rightHand.transform.position;
        rightHRot = player.rightHand.transform.rotation;

        CmdHeadPos(headPos);
        CmdHeadRot(headRot);
        CmdLeftHPos(leftHPos);
        CmdLeftHRot(leftHRot);
        CmdRightHPos(rightHPos);
        CmdRightHRot(rightHRot);


    }

    [Command]
    void CmdHeadPos(Vector3 pos)
    {
        RpcHeadPos(pos);
    }

    [ClientRpc]
    void RpcHeadPos(Vector3 pos)
    {
        headPos = pos;
    }

    [Command]
    void CmdHeadRot(Quaternion rot)
    {
        RpcHeadRot(rot);
    }

    [ClientRpc]
    void RpcHeadRot(Quaternion rot)
    {
        headRot = rot;
    }
///
    [Command]
    void CmdLeftHPos(Vector3 pos)
    {
        RpcLeftHPos(pos);
    }

    [ClientRpc]
    void RpcLeftHPos(Vector3 pos)
    {
        leftHPos = pos;
    }

    [Command]
    void CmdLeftHRot(Quaternion rot)
    {
        RpcLeftHRot(rot);
    }

    [ClientRpc]
    void RpcLeftHRot(Quaternion rot)
    {
        leftHRot = rot;
    }
    ///

    [Command]
    void CmdRightHPos(Vector3 pos)
    {
        RpcRightHPos(pos);
    }

    [ClientRpc]
    void RpcRightHPos(Vector3 pos)
    {
        rightHPos = pos;
    }

    [Command]
    void CmdRightHRot(Quaternion rot)
    {
        RpcRightHRot(rot);
    }

    [ClientRpc]
    void RpcRightHRot(Quaternion rot)
    {
        rightHRot = rot;
    }

}
