using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class ObjSelected
{
    public int index;
    public GameObject gameobject;
    //public List<Hand> hands;
}

public class ObjectManager : MonoBehaviour
{
    //[SyncVar]
    //public int objSelectedShared = 1; // the user selections visible by other players

    public List<GameObject> list;
    public List<GameObject> listGhost;
    public static ObjectManager manager;
    public ObjSelected objSelected;
    public GameObject allInteractable;
    public GameObject allGhosts;

    // Use this for initialization
    void Awake()
    {
        allInteractable = GameObject.Find("InteractableObjects");
        allGhosts = GameObject.Find("GhostObjects");

        for (int i = 0; i < allInteractable.transform.childCount; i++)
            list.Add(allInteractable.transform.GetChild(i).gameObject);

        for (int i = 0; i < allGhosts.transform.childCount; i++)
            listGhost.Add(allGhosts.transform.GetChild(i).gameObject);


        manager = this;

    }

    public static GameObject Get(int i)
    {
        if( i < manager.list.Count && i >= 0)
            return manager.list[i];
        return null;
    }

    public static GameObject GetGhost(int i)
    {
        if (i < manager.listGhost.Count && i >= 0)
            return manager.listGhost[i];
        return null;
    }


    public static void Set(int i, GameObject obj)
    {
        manager.list[i] = obj;
    }

    public static void AddToSelected(GameObject obj)
    {
        var index = DetermineIndexSelected(obj);
        ObjSelected objS = new ObjSelected();
        objS.index = index;
        objS.gameobject = obj;
        //objS.hands = new List<Hand>();
        //objS.hands.Add(h);
        //if(h.otherHand != null)
        //    objS.hands.Add(h.otherHand); // add the other hand
        manager.objSelected = objS;
    }

    public static void SetSelected(GameObject obj)
    {
        if (manager.objSelected == null) // if it is not selected, add to list
            AddToSelected(obj); // add the object and the hand whitch is selecting it
    }

    public static void SetSelected(int index)
    {
        var g = Get(index);
        ClearSelected();
        if (manager.objSelected == null) // if it is not selected, add to list
            AddToSelected(g); // add the object and the hand whitch is selecting it
    }

    public static void ClearSelected()
    {
        if (manager.objSelected != null)
            manager.objSelected = null;
    }

    public static int DetermineIndexSelected(GameObject obj)
    {
        for (int i = 0; i < manager.list.Count; i++)
        {
            if (GameObject.ReferenceEquals(manager.list[i], obj))
                return i;
        }
        return -1;
    }

    public static void RemoveObjFromSelected(ObjSelected obj)
    {
        if (GameObject.ReferenceEquals(manager.objSelected, obj))
            manager.objSelected = null;

    }

    public static ObjSelected GetSelected()
    {
        if (manager.objSelected != null)
            return manager.objSelected;
        return null;
    }
}