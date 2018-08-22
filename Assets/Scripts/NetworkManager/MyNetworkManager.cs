using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{

    public Utils.PlayerType playerType = Utils.PlayerType.None;
    string userID;

    ////Called on client when connect
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect");
        //base.OnClientConnect(conn);
    }

    // Server
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        Debug.Log("OnServerAddPlayer");
        int pType = -1;
        int pId = -1;
        string[] parameters = new string[2];
        if (extraMessageReader != null) // Read client message and receive index
        {
            var stream = extraMessageReader.ReadMessage<StringMessage>();
            parameters = stream.value.Split(';');
            pType = int.Parse(parameters[0]);
            pId = int.Parse(parameters[1]);
        }
        var pPrefab = spawnPrefabs[pType]; //Select the prefab from the spawnable objects list
        var pStuff = pPrefab.GetComponent<PlayerStuff>();
        pStuff.id = pId;
        pStuff.type = (Utils.PlayerType)pType;

        var player = Instantiate(pPrefab, new Vector3(Random.Range(-2, 2), 0, 0), Quaternion.identity) as GameObject; // Create player object with prefab
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId); // Add player object for connection
        //base.OnServerAddPlayer(conn, playerControllerId, extraMessageReader);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("OnClientSceneChanged");
        string message = (int)playerType + ";" + userID; //cast to int because otherwise it will get the string of playertype
        StringMessage msg = new StringMessage(message); // Create message to set the player
        ClientScene.AddPlayer(conn, 0, msg); // Call Add player and pass the message
        //base.OnClientSceneChanged(conn);
    }

    public void SelectVR()
    {
        playerType = Utils.PlayerType.VR;
    }

    public void SelectAR()
    {
        playerType = Utils.PlayerType.AR;
    }

    public void StartMyHost()
    {
        SetPort();
        userID = "0";
        NetworkManager.singleton.StartServer();
    }

    public void JoinSession()
    {
        if (!SetUserID()) return; // only connect if it has user id
        if (playerType == Utils.PlayerType.None) return; // and player type is selected
        SetPort();
        SetIpAddress();
        NetworkManager.singleton.StartClient();
    }

    bool SetUserID()
    {
        string uID = GameObject.Find("UserID").transform.Find("Text").GetComponent<Text>().text;
        if (uID.Length != 0)
        {
            userID = uID;
            return true;
        }
        return false;
    }

    public void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    void SetIpAddress()
    {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

}