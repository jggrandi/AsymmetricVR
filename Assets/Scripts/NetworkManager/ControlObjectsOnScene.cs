using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlObjectsOnScene : MonoBehaviour {
    public GameObject VRCamera;
    public GameObject ARCamera;
    public GameObject ServerCamera;


    GameObject netMan;
    // Use this for initialization
    void Start () {
        netMan = GameObject.Find("NetworkManager");
        var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
        if (refMyNetMan.playerType == Utils.PlayerType.VR)
        {
            VRCamera.SetActive(true);
            ARCamera.SetActive(false);
            ServerCamera.SetActive(false);
            Debug.Log("Activating VR Stuff");
        }
        else if (refMyNetMan.playerType == Utils.PlayerType.AR)
        {
            ARCamera.SetActive(true);
            VRCamera.SetActive(false);
            ServerCamera.SetActive(false);
            Debug.Log("Activating AR Stuff");
        }
        else 
        {
            refMyNetMan.playerType = Utils.PlayerType.None;
            ARCamera.SetActive(false);
            VRCamera.SetActive(false);
            ServerCamera.SetActive(true);
            Debug.Log("Activating Server Camera");
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
