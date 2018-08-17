using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TestController : NetworkBehaviour {

    public static TestController tcontrol;

	public int conditionsToPermute = 3;

    public SyncListInt taskOrder = new SyncListInt();
    
    [SyncVar]
    public int groupID = 0;
    [SyncVar]
    public int sceneIndex = 0;


    void Awake() {

        if (tcontrol == null)
        { 
            DontDestroyOnLoad(gameObject);
            tcontrol = this;
        }
        else if (tcontrol != this)
            Destroy(gameObject);
        
    }
}
	

