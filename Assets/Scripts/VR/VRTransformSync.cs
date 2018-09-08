using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class VRTransformSync : MonoBehaviour
{

    public Vector3 headPos;
    public Quaternion headRot;
    public Vector3 leftHPos;
    public Quaternion leftHRot;
    public Vector3 rightHPos;
    public Quaternion rightHRot;

    public bool isTranslating = false;
    public bool isRotating = false;
    public bool isScaling = false;

    Player player;

    private void Start()
    {
        if (string.Compare(SceneManager.GetActiveScene().name, "SetupTest") == 0) return;

        player = Player.instance;

        if (player == null)
        {
            Debug.LogError("No Player instance found in map.");
            Destroy(this.gameObject);
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (player == null) player = Player.instance;

        headPos = player.hmdTransform.position;
        headRot = player.hmdTransform.rotation;


        leftHPos = player.leftHand.transform.position;
        leftHRot = player.leftHand.transform.rotation;

        rightHPos = player.rightHand.transform.position;
        rightHRot = player.rightHand.transform.rotation;

    }

}
