using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStuff : NetworkBehaviour {

    public int id = -1;
    public Utils.PlayerType type = Utils.PlayerType.None;
    public float activeTime = 0f;

}
