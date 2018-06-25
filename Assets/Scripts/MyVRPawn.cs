using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
namespace Valve.VR.InteractionSystem
{
    public class MyVRPawn : NetworkBehaviour
    {

        public Transform Head;
        public Transform LeftController;
        public Transform RightController;

        Player player = null;


        void Start()
        {
            if (isLocalPlayer)
            {
                player = InteractionSystem.Player.instance;

                if (player == null)
                {
                    Debug.LogError("Teleport: No Player instance found in map.");
                    Destroy(this.gameObject);
                    return;
                }

                GetComponentInChildren<SteamVR_ControllerManager>().enabled = true;
                GetComponentsInChildren<SteamVR_TrackedObject>(true).ToList().ForEach(x => x.enabled = true);
                Head.GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach(x => x.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);
                gameObject.name = "VRPawn (LocalPlayer)";
            }
            else
            {
                gameObject.name = "VRPawn (RemotePlayer)";
            }
        }

        private void Update()
        {
            this.transform.position = player.transform.position;

        }

        void OnDestroy()
        {
            GetComponentInChildren<SteamVR_ControllerManager>().enabled = false;
            GetComponentsInChildren<SteamVR_TrackedObject>(true).ToList().ForEach(x => x.enabled = false);
        }
    }
}