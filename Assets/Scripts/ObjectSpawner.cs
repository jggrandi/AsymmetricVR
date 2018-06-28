using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectSpawner : NetworkBehaviour {

    public GameObject objPrefab;
    public int numberOfObjects;

    public override void OnStartServer()
    {
        for(int i = 0; i< numberOfObjects; i++)
        {
            var obj = (GameObject)Instantiate(objPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(obj);

        }
        base.OnStartServer();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
