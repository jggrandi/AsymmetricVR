using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HandleARGUI : MonoBehaviour {

    public bool lockTransform = false;

    GameObject netMan;
    // Use this for initialization
    void Start()
    {
        netMan = GameObject.Find("NetworkManager");
        var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
        if(refMyNetMan.playerType == Utils.PlayerType.AR)
            this.gameObject.GetComponent<Canvas>().enabled = true;

    }

    public void buttonLock()
    {
        lockTransform = true;
    }

    public void buttonUnlock() {
        lockTransform = false;
    }

}
