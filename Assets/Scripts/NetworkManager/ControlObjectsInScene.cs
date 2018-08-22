using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlObjectsInScene : MonoBehaviour {
    public GameObject VRStuff;
    public GameObject ARStuff;
    public GameObject ServerStuff;


    GameObject netMan;
    // Use this for initialization
    void Start () {
        netMan = GameObject.Find("NetworkManager");
        var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
        if (refMyNetMan.playerType == Utils.PlayerType.VR)
        {
            VRStuff.SetActive(true);
            ARStuff.SetActive(false);
            ServerStuff.SetActive(false);
            Debug.Log("Activating VR Stuff");
        }
        else if (refMyNetMan.playerType == Utils.PlayerType.AR)
        {
            ARStuff.SetActive(true);
            VRStuff.SetActive(false);
            ServerStuff.SetActive(false);
            Debug.Log("Activating AR Stuff");
        }
        else 
        {
            refMyNetMan.playerType = Utils.PlayerType.None;
            ARStuff.SetActive(false);
            VRStuff.SetActive(false);
            ServerStuff.SetActive(true);
            Debug.Log("Activating Server Stuff");
        }


        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerVR"))
            player.gameObject.SetActive(true);


        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerAR"))
            player.gameObject.SetActive(true);


    }
	
	// Update is called once per frame
	void Update () {

        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerVR"))
            player.gameObject.SetActive(true);


        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerAR"))
            player.gameObject.SetActive(true);
    }
}
