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
        
        Player player = null;

        public GameObject allObjects;

        [Command]
        void CmdSyncTransform(int index, Vector3 objPos, Quaternion objRot)
        {
            var g = ObjectManager.Get(index);
            g.transform.position = objPos;
            g.transform.rotation = objRot;
            RpcSyncTransform(index, objPos, objRot);
            //Debug.Log("Server");
        }

        [ClientRpc]
        void RpcSyncTransform(int index, Vector3 objPos, Quaternion objRot)
        {
            var g = ObjectManager.Get(index);
            g.transform.position = objPos;
            g.transform.rotation = objRot;
            //Debug.Log("Client");
        }



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
                allObjects = GameObject.Find("InteractableObjects");

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
            //Debug.Log("AAA");
            for (int i = 0; i < allObjects.transform.childCount; i++)
            {
                var objTransform = allObjects.transform.GetChild(i);
                CmdSyncTransform(i, objTransform.position, objTransform.rotation );
            }

            //if (GhostHead == null) return;
            //GhostHead.transform.position = Vector3.Lerp(GhostHead.transform.position, Head.position, 0.01f);
            //GhostHead.transform.rotation = Quaternion.Slerp(GhostHead.transform.rotation, Head.rotation, 0.1f);
            //GhostHead.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            GetComponentInChildren<SteamVR_ControllerManager>().enabled = false;
            GetComponentsInChildren<SteamVR_TrackedObject>(true).ToList().ForEach(x => x.enabled = false);
        }
    }
}
