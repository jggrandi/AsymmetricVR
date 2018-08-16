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
        if (refMyNetMan.curPlayer == 0)
        {
            VRCamera.SetActive(true);
            ARCamera.SetActive(false);

            Debug.Log("Activating VR Stuff");
        }
        else if (refMyNetMan.curPlayer == 1)
        {
            ARCamera.SetActive(true);
            VRCamera.SetActive(false);
            Debug.Log("Activating AR Stuff");
        }
        else
        {
            refMyNetMan.curPlayer = -1;
            Debug.Log("Fail to activate VR or AR camera.");
        }

        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
