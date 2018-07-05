using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class ObjectManager : NetworkBehaviour {

    public List<GameObject> list;
    public static ObjectManager manager;

    public GameObject objSelected;

    public GameObject allInteractable;

    public GameObject ghostPrefab;

    public static GameObject Get(int i)
    {
        return manager.list[i];
    }

    public static void Set(int i, GameObject obj)
    {
        manager.list[i] = obj;
    }

    public static GameObject GetSelected()
    {
        return manager.objSelected;
    }

    public static void SetSelected( GameObject obj)
    {
        manager.objSelected = obj;
    }

    public static void DeleteSelected()
    {
        if (manager.objSelected != null)
            manager.objSelected = null;
    }


    // Use this for initialization
    void Start () {
        if (!isLocalPlayer) return;
        allInteractable = GameObject.Find("InteractableObjects");
        var ghosts = gameObject.transform.Find("InteractableGhosts");
        for (int i = 0; i < allInteractable.transform.childCount; i++)
        {
            var obj = allInteractable.transform.GetChild(i).gameObject;
            list.Add(obj);
            ghosts.GetChild(i).GetComponent<ObjReference>().ObjRef = obj.transform;
           
        }
        manager = this;
    }

    public override void OnStartClient()
    {

    }


    // Update is called once per frame
    void Update () {
		
	}
}
