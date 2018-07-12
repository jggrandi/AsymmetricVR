using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class ObjSelected
{
    public int index;
    public GameObject obj;
    public List<Hand> hands;
}

public class ObjectManager : MonoBehaviour
{

    public List<GameObject> list;
    public static ObjectManager manager;
    public List<ObjSelected> objsSelected;
    public GameObject allObjects;

    public static GameObject Get(int i)
    {
        return manager.list[i];
    }

    public static void Set(int i, GameObject obj)
    {
        manager.list[i] = obj;
    }

    public static ObjSelected FindObject(GameObject obj)
    {
        foreach(ObjSelected selected in manager.objsSelected)
        {
            if (GameObject.ReferenceEquals(selected.obj, obj))
                return selected;
        }
        return null;
    }

    public static void AddToSelectedList(GameObject obj, Hand h)
    {
        var index = DetermineIndexSelected(obj);
        ObjSelected objS = new ObjSelected();
        objS.index = index;
        objS.obj = obj;
        objS.hands = new List<Hand>();
        objS.hands.Add(h);
        manager.objsSelected.Add(objS);
    }


    public static void SetSelected(GameObject obj, Hand h)
    {
        var found = FindObject(obj);
        if (found == null) // if it is not selected, add to list
        {
            AddToSelectedList(obj,h); // add the object and the hand whitch is selecting it
        }
        else // it is already selected, we need to verify if the hand that is selecting is other hand
        {
            if(!found.hands.Contains(h)) // if is not the same hand, add this hand to the obj
                found.hands.Add(h);  
        }
    }


    public static int DetermineIndexSelected(GameObject obj)
    {
        for(int i = 0; i < manager.list.Count; i++)
        {
            if (GameObject.ReferenceEquals(manager.list[i], obj))
                return i;
        }
        return -1;
    }

    public static void RemoveObjFromSelectedList(ObjSelected obj)
    {
        if (manager.objsSelected.Contains(obj))
            manager.objsSelected.Remove(obj);
    }

    public static void DetachObjectFromHand(GameObject obj, Hand h)
    {
        var found = FindObject(obj);
        if (found == null) return; // if it is not selected, nothing to do, return

        if (found.hands.Contains(h)) // if the object is being selected by this hand
            found.hands.Remove(h);

        if (found.hands.Count == 0) // there is no hand selecting the object, remove object from the selected list
            RemoveObjFromSelectedList(found);
    }

    public static List<ObjSelected> GetAllSelected()
    {
        if (manager.objsSelected != null)
            return manager.objsSelected;
        return null;
    }

    public static ObjSelected GetSelected(int index)
    {
        if(index < manager.objsSelected.Count && index >= 0 )
            return manager.objsSelected[index];
        return null;
    }



    // Use this for initialization
    void Start()
    {
        allObjects = GameObject.Find("InteractableObjects");

        objsSelected = new List<ObjSelected>();

        for (int i = 0; i < allObjects.transform.childCount; i++)
        {
            list.Add(allObjects.transform.GetChild(i).gameObject);
        }
        
        manager = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}