using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControlObjectsOnScene : MonoBehaviour {
    public GameObject VRCamera;
    public GameObject ARCamera;


    GameObject netMan;
    // Use this for initialization
    void Start () {
        netMan = GameObject.Find("NetworkManager");
        var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
        if (refMyNetMan.curPlayer == (int)Utils.PlayerType.VR)
        {
            VRCamera.SetActive(true);
            ARCamera.SetActive(false);

            Debug.Log("Activating VR Stuff");
        }
        else if (refMyNetMan.curPlayer == (int)Utils.PlayerType.AR)
        {
            ARCamera.SetActive(true);
            VRCamera.SetActive(false);
            Debug.Log("Activating AR Stuff");
        }
        else
        {
            refMyNetMan.curPlayer = (int)Utils.PlayerType.None;
            Debug.Log("Fail to activate VR or AR camera.");
        }

        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
