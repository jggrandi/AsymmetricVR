using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoulderReference : MonoBehaviour {

    public static GameObject virtualShoulder;
    Vector3 shoulderOffset = new Vector3(0f, -0.1f, 0f);

    private void Awake()
    {
        virtualShoulder = new GameObject();
        virtualShoulder.name = "VirtualShoulder";
        virtualShoulder.transform.position = Camera.main.transform.position + shoulderOffset;
        virtualShoulder.transform.rotation = Quaternion.identity;

    }

	void Update () {
        virtualShoulder.transform.position = Camera.main.transform.position + shoulderOffset;
    }
}
