using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{

    public Utils.PlayerType playerType = Utils.PlayerType.None;
    public string userID;
    bool isInSetup = true;

    //private void Start()
    //{
    //    playerType =
    //}

    private void Update()
    {
        if (!isInSetup) return;

        var tVR = GameObject.Find("ToggleVR").GetComponent<Toggle>();
        var tAR = GameObject.Find("ToggleAR").GetComponent<Toggle>();

        if (!tVR.isOn && !tAR.isOn)
            playerType = Utils.PlayerType.None;


    }

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
        int pT = -1;
        if (extraMessageReader != null) // Read client message and receive index
        {
            var stream = extraMessageReader.ReadMessage<IntegerMessage>();
            pT = (int)(Utils.PlayerType)stream.value;
        }
        var pP = spawnPrefabs[pT]; //Select the prefab from the spawnable objects list
        var player = Instantiate(pP, new Vector3(Random.Range(-2, 2), 0, 0), Quaternion.identity) as GameObject; // Create player object with prefab
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId); // Add player object for connection
        //base.OnServerAddPlayer(conn, playerControllerId, extraMessageReader);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("OnClientSceneChanged");
        IntegerMessage msg = new IntegerMessage((int)playerType); // Create message to set the player
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
        isInSetup = false;
        NetworkManager.singleton.StartServer();
    }

    public void JoinSession()
    {
        if (!SetUserID()) return;
        SetPort();
        SetIpAddress();
        isInSetup = false;
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