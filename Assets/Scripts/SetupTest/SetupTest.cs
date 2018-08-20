using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupTest : NetworkBehaviour {

    List<GameObject> playersConnected;

    NetworkManager networkManager = NetworkManager.singleton;
    List<PlayerController> pc;


    void Start()
    {

        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerVR"))
            player.gameObject.SetActive(false);

        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerAR"))
            player.gameObject.SetActive(false);


        if (isServer)
        {
            GameObject.Find("PanelServer").GetComponent<Canvas>().enabled = true;
            
            GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text = TestController.tcontrol.groupID.ToString();
            int[] order = Utils.selectTaskSequence(TestController.tcontrol.groupID, TestController.tcontrol.conditionsToPermute);
            TestController.tcontrol.taskOrder.Clear();
            TestController.tcontrol.taskOrder.Add(-1);
            for (int i = 0; i < order.Length; i++)
                TestController.tcontrol.taskOrder.Add(order[i]);

            if (TestController.tcontrol.sceneIndex > TestController.tcontrol.taskOrder.Count - 1)
                MyNetworkManager.singleton.ServerChangeScene("EndTest");

            Debug.Log(TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex]);
            GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
            GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();
        }
        else if(isClient)
        {
            GameObject.Find("PanelClient").GetComponent<Canvas>().enabled = true;
        }
    }

    void Update()
    {

        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerVR"))
            player.gameObject.SetActive(false);


        foreach (var player in GameObject.FindGameObjectsWithTag("PlayerAR"))
            player.gameObject.SetActive(false);


    }

    bool FindPlayer(GameObject player)
    {
        foreach (var p in playersConnected)
        {
            if (string.Compare(p.name, player.name) == 0)
                return true;
        }
        return false;
    }


    public void StartScene()
    {
        if (isServer)
            MyNetworkManager.singleton.ServerChangeScene("AsymEnvironment");
    }

    public void UpdateGroupId()
    {
        if (GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text == "") return;

        CmdUpdateGroup();
        UpdateScene();
    }

    void UpdateScene()
    {
        Debug.Log(TestController.tcontrol.groupID);
        int[] order = Utils.selectTaskSequence(TestController.tcontrol.groupID, TestController.tcontrol.conditionsToPermute);
        TestController.tcontrol.taskOrder.Clear();

        TestController.tcontrol.taskOrder.Add(-1);
        for (int i = 0; i < order.Length; i++)
            TestController.tcontrol.taskOrder.Add(order[i]);


        GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
        GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();
        Debug.Log(TestController.tcontrol.sceneIndex);
        CmdUpdateScene();
    }

    [Command]
    void CmdUpdateGroup()
    {
        TestController.tcontrol.groupID = int.Parse(GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text);
    }

    [Command]
    void CmdUpdateScene()
    {
        TestController.tcontrol.sceneIndex = int.Parse(GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text);
    }

    [Command]
    void CmdIncrementSceneID()
    {
        TestController.tcontrol.sceneIndex++;
    }

    [Command]
    void CmdDecrementSceneID()
    {
        TestController.tcontrol.sceneIndex--;
    }

    public void ButtonNextScene()
    {
        if (TestController.tcontrol.sceneIndex < 3)
        {
            CmdIncrementSceneID();
            GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();
            GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
            CmdUpdateScene();
        }
    }

    public void ButtonPreviousScene()
    {
        if (TestController.tcontrol.sceneIndex > 0)
        {
            CmdDecrementSceneID();
            GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();
            GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
            CmdUpdateScene();
        }
    }
}
