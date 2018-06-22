using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

public class VRPawn : NetworkBehaviour {

    public Transform Head;
    public Transform LeftController;
    public Transform RightController;
    public Transform Teleport;


    void Start () {
        if (isLocalPlayer) { 
            GetComponentInChildren<SteamVR_ControllerManager>().enabled = true;
            GetComponentsInChildren<SteamVR_TrackedObject>(true).ToList().ForEach(x => x.enabled = true);
            Head.GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach(x => x.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);
            gameObject.name = "VRPawn (LocalPlayer)";
        } else
        {
            gameObject.name = "VRPawn (RemotePlayer)";
        }
	}

    void OnDestroy()
    {
        GetComponentInChildren<SteamVR_ControllerManager>().enabled = false;
        GetComponentsInChildren<SteamVR_TrackedObject>(true).ToList().ForEach(x => x.enabled = false);
    }
}
