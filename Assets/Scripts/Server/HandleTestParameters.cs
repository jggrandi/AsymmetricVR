using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandleTestParameters : NetworkBehaviour
{

    public int conditionsToPermute = 3;
    public SyncListInt taskOrder = new SyncListInt();

    [SyncVar]
    public int groupID = 0;
    [SyncVar]
    public int modalityIndex = 0;


    GameObject panelConnected;
    NetworkManager netMan;
    public List<GameObject> activeInScene;
    public List<GameObject> guiPlayer;

    // Use this for initialization
    void Start()
    {

        if (!isServer) return;
        panelConnected = GameObject.Find("ClientsConnected");
        netMan = NetworkManager.singleton;

        guiPlayer = new List<GameObject>();
        for (int i = 0; i < 10; i++) guiPlayer.Add(null);
        activeInScene = new List<GameObject>();

        GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text = groupID.ToString();
        int[] order = Utils.selectTaskSequence(groupID, conditionsToPermute);
        taskOrder.Clear();

        for (int i = 0; i < order.Length; i++)
            taskOrder.Add(order[i]);
        GameObject.Find("InputModality").GetComponent<InputField>().text = taskOrder[modalityIndex].ToString();


    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;

        var arObjs = GameObject.FindGameObjectsWithTag("PlayerAR");
        var vrObjs = GameObject.FindGameObjectsWithTag("PlayerVR");

        if (arObjs.Length > 0)
        {
            for (int i = 0; i < arObjs.Length; i++) // add all ar players connected
                if(!activeInScene.Contains(arObjs[i]))
                    activeInScene.Add(arObjs[i]); 
        }
        if (vrObjs.Length > 0)
        {
            for (int i = 0; i < vrObjs.Length; i++) //add all vr players connected
                if (!activeInScene.Contains(vrObjs[i]))
                    activeInScene.Add(vrObjs[i]);
        }

        if (activeInScene.Count == 0) //if there is no players connected , remove the text of all UI 
        {
            for (int i = 0; i < panelConnected.transform.childCount; i++)
            {
                var slot = panelConnected.transform.GetChild(i);
                Text playerName = slot.GetComponent<Text>();
                playerName.text = "";
            }
            return; // dont need to do other things
        }

        HandleDisplayPlayer(activeInScene); // handle the display of the player's name on the UI
    }

    void HandleDisplayPlayer(List<GameObject> _bjs)
    {
        for (int i = 0; i < _bjs.Count; i++)
        {
            if (_bjs[i] == null) //if one player disconnected
            {
                _bjs.RemoveAt(i); //remove it
                RemovePlayerOnDisplay(i); //remove the ui text
            }
            else if (!guiPlayer.Contains(_bjs[i])) //if the player is connected but dont have a ui text
                AddPlayerOnDisplay(i, _bjs[i]);
        }
    }

    void AddPlayerOnDisplay(int index, GameObject g )
    {
        guiPlayer[index] = g;
        var slot = panelConnected.transform.GetChild(index);
        Text playerName = slot.GetComponent<Text>();
        playerName.text = g.name;
    }

    void RemovePlayerOnDisplay(int index)
    {
        guiPlayer.RemoveAt(index);
        var slot = panelConnected.transform.GetChild(index);
        Text playerName = slot.GetComponent<Text>();
        playerName.text = "";
    }


    public void UpdateGroupId()
    {
        groupID = int.Parse(GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text);

        int[] order = Utils.selectTaskSequence(groupID, conditionsToPermute);
        taskOrder.Clear();

        for (int i = 0; i < order.Length; i++)
            taskOrder.Add(order[i]);
        modalityIndex = 0;
        GameObject.Find("InputModality").GetComponent<InputField>().text = taskOrder[modalityIndex].ToString();
    }

    public void ButtonNextCondition()
    {
        if (modalityIndex < conditionsToPermute-1)
        {
            IncrementSceneID();
            GameObject.Find("InputModality").GetComponent<InputField>().text = taskOrder[modalityIndex].ToString();
            //UpdateScene();
        }
    }

    public void ButtonPreviousCondition()
    {
        if (modalityIndex > 0)
        {
            DecrementSceneID();
            GameObject.Find("InputModality").GetComponent<InputField>().text = taskOrder[modalityIndex].ToString();
           // UpdateScene();
        }
    }


    void IncrementSceneID()
    {
        modalityIndex++;
    }

    void DecrementSceneID()
    {
        modalityIndex--;
    }


    void UpdateScene()
    {
        modalityIndex = int.Parse(GameObject.Find("InputModality").GetComponent<InputField>().text);
    }


}
