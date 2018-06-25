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
        public GameObject GhostHeadPrefab;
        private GameObject GhostHead;
        Player player = null;


        [Command]
        void CmdSpawn() {
            RpcSpawn();
            //NetworkServer.Spawn(GhostHead);
        }
        [ClientRpc]
        void RpcSpawn()
        {
            GhostHead = (GameObject)Instantiate(GhostHeadPrefab, Head.position, Head.rotation);
        }

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
                
                CmdSpawn();
            }
            else
            {
                gameObject.name = "VRPawn (RemotePlayer)";
            }
            
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
            this.transform.position = player.transform.position;

            if (GhostHead == null) return;
            GhostHead.transform.position = Vector3.Lerp(GhostHead.transform.position, Head.position, 0.01f);
            GhostHead.transform.rotation = Quaternion.Slerp(GhostHead.transform.rotation, Head.rotation, 0.1f);
            //GhostHead.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            GetComponentInChildren<SteamVR_ControllerManager>().enabled = false;
            GetComponentsInChildren<SteamVR_TrackedObject>(true).ToList().ForEach(x => x.enabled = false);
        }
    }
}
