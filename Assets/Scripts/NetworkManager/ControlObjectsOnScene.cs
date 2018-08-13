using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ControlObjectsOnScene : NetworkBehaviour {
    public GameObject VRPlayer;

    GameObject netMan;
    // Use this for initialization
    void Start () {
        netMan = GameObject.Find("NetworkManager");
        var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
        if (refMyNetMan.curPlayer == 0)
        {
            Debug.Log("Activating VR Stuff");
            VRPlayer.SetActive(true);
        }
        else if (refMyNetMan.curPlayer == 1)
            Debug.Log("Activating AR Stuff");
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
