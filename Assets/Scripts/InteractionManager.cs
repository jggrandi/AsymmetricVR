using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class InteractionManager : MonoBehaviour {

    public List<ManipulationPoint> manipulationPoints = new List<ManipulationPoint>();


	// Use this for initialization
	void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
        var selected = ObjectManager.GetAllSelected();


	}

    public void RegisterManipulationPoint(ManipulationPoint mp)
    {
        manipulationPoints.Add(mp);
    }
}
