using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjReference : NetworkBehaviour {

    public Transform ObjRef { get; set; }
    public bool IsSelected { get; set; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        if (gameObject.transform.parent.transform.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            gameObject.transform.position = ObjRef.position;
            gameObject.transform.rotation = ObjRef.rotation;
        
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<NetworkIdentity>().isLocalPlayer) continue;
                var otherPlayerObjects = player.GetComponentsInChildren<ObjReference>();
                foreach (ObjReference objR in otherPlayerObjects)
                {
                    if (objR.name == this.name)
                    {
                        ObjRef.position = objR.transform.position;
                        ObjRef.rotation = objR.transform.rotation;
                    }

                }
            }
        }
    }  
}
