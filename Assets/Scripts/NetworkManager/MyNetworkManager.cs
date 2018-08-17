using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class MyNetworkManager : NetworkManager
{

    public int curPlayer = (int)Utils.PlayerType.None;


    //Called on client when connect
    public override void OnClientConnect(NetworkConnection conn)
    {

        // Create message to set the player
        IntegerMessage msg = new IntegerMessage(curPlayer);

        // Call Add player and pass the message
        ClientScene.AddPlayer(conn, 0, msg);

    }

    // Server
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        // Read client message and receive index
        if (extraMessageReader != null)
        {
            var stream = extraMessageReader.ReadMessage<IntegerMessage>();
            curPlayer = stream.value;
        }
        //Select the prefab from the spawnable objects list
        var playerPrefab = spawnPrefabs[curPlayer];

        // Create player object with prefab
        var player = Instantiate(playerPrefab, new Vector3(Random.Range(-2,2),0,0), Quaternion.identity) as GameObject;

        // Add player object for connection
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        //base.OnClientSceneChanged(conn);



        //by overriding this function and commenting the base we are removing the functionality of this function 
        //so we dont have conflict with  OnClientConnect

    }

    public void btn0()
    {
        curPlayer = (int)Utils.PlayerType.VR;
    }

    public void btn1()
    {
        curPlayer = (int)Utils.PlayerType.AR;
    }

}