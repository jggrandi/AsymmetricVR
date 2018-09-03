using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStuff : NetworkBehaviour {

    [SyncVar]
    public int id = -1;
    [SyncVar]
    public Utils.PlayerType type = Utils.PlayerType.None;
    [SyncVar]
    public float activeTime = 0f;

    public bool isGhost = false;

    [Command]
    public void CmdSetIsGhost(bool isIt)
    {
        isGhost = isIt;
        RpcSetIsGhost(isIt);
    }

    [ClientRpc]
    public void RpcSetIsGhost(bool isIt)
    {
        isGhost = isIt;
    }

}
