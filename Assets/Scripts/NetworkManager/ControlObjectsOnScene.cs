using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ControlObjectsOnScene : NetworkBehaviour {
    public GameObject VRCamera;
    public GameObject ARCamera;


    GameObject netMan;
    // Use this for initialization
    void Start () {
        netMan = GameObject.Find("NetworkManager");
        var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
        if (refMyNetMan.curPlayer == 0)
        {
            Debug.Log("Activating VR Stuff");
            VRCamera.SetActive(true);
        }
        else if (refMyNetMan.curPlayer == 1)
        {
            ARCamera.SetActive(true);
            Debug.Log("Activating AR Stuff");
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
