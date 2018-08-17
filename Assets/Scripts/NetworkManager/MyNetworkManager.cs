using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class MyNetworkManager : NetworkManager
{

    public Utils.PlayerType curPlayer;
    public string userID;
    bool isInSetup = true;

    private void Start()
    {
        curPlayer = Utils.PlayerType.None;
    }

    private void Update()
    {
        if (!isInSetup) return;

        var tVR = GameObject.Find("ToggleVR").GetComponent<Toggle>();
        var tAR = GameObject.Find("ToggleAR").GetComponent<Toggle>();

        if (!tVR.isOn && !tAR.isOn)
            curPlayer = Utils.PlayerType.None;


    }

    //Called on client when connect
    public override void OnClientConnect(NetworkConnection conn)
    {
        IntegerMessage msg = new IntegerMessage((int)curPlayer); // Create message to set the player
        ClientScene.AddPlayer(conn, 0, msg); // Call Add player and pass the message
    }

    // Server
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        if (extraMessageReader != null) // Read client message and receive index
        {
            var stream = extraMessageReader.ReadMessage<IntegerMessage>();
            curPlayer = (Utils.PlayerType)stream.value;
        }
        var playerPrefab = spawnPrefabs[(int)curPlayer]; //Select the prefab from the spawnable objects list
        var player = Instantiate(playerPrefab, new Vector3(Random.Range(-2,2),0,0), Quaternion.identity) as GameObject; // Create player object with prefab
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId); // Add player object for connection
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        //base.OnClientSceneChanged(conn);
        //by overriding this function and commenting the base we are removing the functionality of this function 
        //so we dont have conflict with  OnClientConnect
    }

    public void SelectVR()
    {
        curPlayer = Utils.PlayerType.VR;
    }

    public void SelectAR()
    {
        curPlayer = Utils.PlayerType.AR;
    }

    public void StartMyHost()
    {
        SetPort();
        userID = "0";
        isInSetup = false;
        NetworkManager.singleton.StartHost();
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