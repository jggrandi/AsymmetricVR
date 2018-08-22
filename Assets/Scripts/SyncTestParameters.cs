using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncTestParameters : NetworkBehaviour {

    public SyncListInt conditionsOrder = new SyncListInt();
    public SyncListInt condition0TrialOrder = new SyncListInt();
    public SyncListInt condition1TrialOrder = new SyncListInt();
    public SyncListInt condition2TrialOrder = new SyncListInt();

    [SyncVar]
    public int trialIndex;
    
    // Use this for initialization
    void Start () {
        if (!isClient) return;

        // ACTIVATE OBJECTS ON THE INTERFACE AND CONTROL WHITCH OBJECT IS SELECTED


	}
	
	// Update is called once per frame
	void Update () {
        if (!isClient) return;



    }
}
