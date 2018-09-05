using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ControlObjectsInScene : NetworkBehaviour {
    public GameObject VRStuff;
    public GameObject ARStuff;
    public GameObject ServerStuff;
    public GameObject SceneObjects;

    GameObject netMan;
    // Use this for initialization

    public override void OnStartServer()
    {
        netMan = GameObject.Find("NetworkManager");
        var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
        refMyNetMan.playerType = Utils.PlayerType.None;
        ARStuff.SetActive(false);
        VRStuff.SetActive(false);
        ServerStuff.SetActive(true);
        Debug.Log("Activating Server Stuff");
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
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
            SceneObjects.SetActive(false);
            Debug.Log("Activating AR Stuff");
        }

        base.OnStartClient();
    }

    //void Start () {
    //    netMan = GameObject.Find("NetworkManager");
    //    var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
    //    if (refMyNetMan.playerType == Utils.PlayerType.VR)
    //    {
    //        VRStuff.SetActive(true);
    //        ARStuff.SetActive(false);
    //        ServerStuff.SetActive(false);
    //        Debug.Log("Activating VR Stuff");
    //    }
    //    else if (refMyNetMan.playerType == Utils.PlayerType.AR)
    //    {
    //        ARStuff.SetActive(true);
    //        VRStuff.SetActive(false);
    //        ServerStuff.SetActive(false);
    //        SceneObjects.SetActive(false);
    //        Debug.Log("Activating AR Stuff");
    //    }
    //    else 
    //    {
    //        refMyNetMan.playerType = Utils.PlayerType.None;
    //        ARStuff.SetActive(false);
    //        VRStuff.SetActive(false);
    //        ServerStuff.SetActive(true);
    //        Debug.Log("Activating Server Stuff");
    //    }

    //}
}
