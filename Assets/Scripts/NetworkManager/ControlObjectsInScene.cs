using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ControlObjectsInScene : NetworkBehaviour {
    public GameObject vrStuff;
    public GameObject arStuff;
    public GameObject serverStuff;
    public GameObject sceneObjects;

    NetworkManager netMan;
    // Use this for initialization

    public override void OnStartServer()
    {
        netMan = NetworkManager.singleton;
        var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
        refMyNetMan.playerType = Utils.PlayerType.None;
        arStuff.SetActive(false);
        vrStuff.SetActive(false);
        serverStuff.SetActive(true);
        Debug.Log("Activating Server Stuff");
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        netMan = NetworkManager.singleton;
        var refMyNetMan = netMan.GetComponent<MyNetworkManager>();
        if (refMyNetMan.playerType == Utils.PlayerType.VR)
        {
            vrStuff.SetActive(true);
            arStuff.SetActive(false);
            serverStuff.SetActive(false);
            Debug.Log("Activating VR Stuff");
        }
        else if (refMyNetMan.playerType == Utils.PlayerType.AR)
        {
            arStuff.SetActive(true);
            vrStuff.SetActive(false);
            serverStuff.SetActive(false);
            sceneObjects.SetActive(false);
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
