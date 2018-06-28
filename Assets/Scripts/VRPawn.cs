using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

namespace Valve.VR.InteractionSystem
{
    public class VRPawn : NetworkBehaviour
    {

        public Transform Head;
        public Transform LeftController;
        public Transform RightController;

        Player player = null;

        void Start()
        {
            if (isLocalPlayer)
            {
                //Debug.Log("VAICARAI");
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
                
                Head.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = false);
                LeftController.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = false);
                RightController.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = false);
                gameObject.name = "VRPawn (LocalPlayer)";

            }
            else
            {
                gameObject.name = "VRPawn (RemotePlayer)";
            }
            
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
            
            this.transform.position = Vector3.Lerp(this.transform.position, player.transform.position, 0.01f);
        }

        void OnDestroy()
        {
            GetComponentInChildren<SteamVR_ControllerManager>().enabled = false;
            GetComponentsInChildren<SteamVR_TrackedObject>(true).ToList().ForEach(x => x.enabled = false);
        }
    }
}
