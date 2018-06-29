using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> list;
    public static ObjectManager manager;

    public GameObject objSelected;

    public GameObject allObjects;

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
        allObjects = GameObject.Find("InteractableObjects");

        for(int i=0; i < allObjects.transform.childCount; i++)
        {
            list.Add(allObjects.transform.GetChild(i).gameObject);
        }

        manager = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
