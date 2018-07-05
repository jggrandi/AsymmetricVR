using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Valve.VR.InteractionSystem
{
    public class HandleTransformations : NetworkBehaviour
    {
        Player player = null;

        public GameObject allObjects;


        [Command]
        void CmdSyncTransform(int index, Vector3 objtransstep)
        {
            //var g = ObjectManager2.Get(index);
            //g.transform.localPosition += objtransstep;
            //g.transform.rotation = objRot;
            //g.transform.GetComponent<Rigidbody>().useGravity = gravity;


            //g.transform.position = Vector3.Lerp(g.transform.position, objPos, 0.02f);
            //g.transform.rotation = Quaternion.Slerp(g.transform.rotation, objRot, 0.02f);
            RpcSyncTransform(index, objtransstep);
            //Debug.Log("Server");
        }

        [ClientRpc]
        void RpcSyncTransform(int index, Vector3 objetransstep)
        {
            //if (isLocalPlayer) return;
            var g = ObjectManager2.Get(index);
            var selected = ObjectManager2.GetSelected();

            g.transform.localPosition += objetransstep;

            //if (selected != null && g.name == selected.name)
            //{
            //    g.transform.position = Vector3.Lerp(g.transform.position, objPos, 0.02f);
            //    g.transform.rotation = Quaternion.Lerp(g.transform.rotation, objRot, 0.02f);

            //}
            //else
            //{
            //    g.transform.position = Vector3.Lerp(g.transform.position, objPos, 0.1f);
            //    g.transform.rotation = Quaternion.Lerp(g.transform.rotation, objRot, 0.1f);

            //}



            //g.transform.GetComponent<Rigidbody>().useGravity = gravity;
            //g.transform.position = objPos;
            //g.transform.rotation = objRot;
            //Debug.Log("Client");

            // }

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
            //foreach (Hand hand in player.hands)
            //{
            //    if (hand.currentAttachedObject != null) continue;
            //    //Debug.Log(hand.name + " " + hand.currentAttachedObject);
            //}
            //var obj = ObjectManager2.Get(0);

            //gameObject.AddComponent<NetworkTransformChild>();

           
            if (ObjectManager2.GetSelected() == null) return; // if localplayer is not selecting an object

            for (int i = 0; i < allObjects.transform.childCount; i++)
            {
                var objTransform = allObjects.transform.GetChild(i);

                if (objTransform.name == ObjectManager2.GetSelected().name)
                {
                    var transformantionStep = go.transform.parent.GetComponent<GetTransformStep>();
                    var posStep = transformantionStep.positionStep;
                    CmdSyncTransform(i, posStep);
                }

            }


        }
    }
}