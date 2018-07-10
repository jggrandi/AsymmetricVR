using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Valve.VR.InteractionSystem
{
    public class HandleNetworkTransformations : NetworkBehaviour
    {
        Player player = null;

        public Hand handLeft, handRight;

        public GameObject allObjects;

        [Command]
        void CmdSyncTransform(int index, Vector3 objtransstep, Quaternion objrotstep)
        {
            RpcSyncTransform(index, objtransstep, objrotstep);
        }

        [ClientRpc]
        void RpcSyncTransform(int index, Vector3 objetransstep, Quaternion objrotstep)
        {
            if (isLocalPlayer) return;
            var g = ObjectManager.Get(index);
            
            g.transform.position += objetransstep;
            g.transform.rotation = objrotstep * g.transform.rotation;

        }

        // Use this for initialization
        void Start()
        {
            if (!isLocalPlayer) return;

            player = InteractionSystem.Player.instance;

            if (player == null)
            {
                Debug.LogError("No Player instance found in map.");
                Destroy(this.gameObject);
                return;
            }

            //handLeft = player.leftHand.att;
            //handRight = player.leftHand;
            
            allObjects = GameObject.Find("InteractableObjects");
        }

        public override void OnStartClient()
        {
            Debug.Log("Aqui1");
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            var go = GameObject.Find("Imaginary(Clone)");
            if (go == null) return;
           
            if (ObjectManager.GetSelected() == null) return; // if localplayer is not selecting an object

            for (int i = 0; i < allObjects.transform.childCount; i++)
            {
                var objTransform = allObjects.transform.GetChild(i);

                if (objTransform.name == ObjectManager.GetSelected().name)
                {
                    var transformantionStep = go.transform.parent.GetComponent<GetTransformStep>();
                    var posStep = transformantionStep.positionStep;
                    var rotStep = transformantionStep.rotationStep;
                    CmdSyncTransform(i, posStep,rotStep);
                }

            }


        }
    }
}