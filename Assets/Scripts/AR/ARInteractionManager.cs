using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Lean.Touch {

    
    public class ARInteractionManager : NetworkBehaviour {
        
        public GameObject lockedObjects;
        public GameObject trackedObjects;
        public GameObject mainHandler;
        public HandleARGUI refGUI;

        public bool isTapForTransform = false;

        //public LayerMask LayerMask = Physics.DefaultRaycastLayers;
        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;
        //Utils.Transformations mode = Utils.Transformations.Translation;

        int translationZ = 0;
        float countFrames = 0;
        Matrix4x4 prevMatrix;
        public int modality = -1;
        //int currentOperation = 0; /* move rotate resize move_cel */

        ObjSelected selected;

        void Start()
        {
            if (!isLocalPlayer) return;
            trackedObjects = GameObject.Find("TrackedObjects");
            mainHandler = GameObject.Find("MainHandler");
            if (mainHandler != null)
                refGUI = mainHandler.GetComponent<HandleARGUI>();
        }

        string log ="";

        void OnGUI() {
            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            centeredStyle.fontSize = 30;
            GUI.Label(new Rect(Screen.width / 2 - 500, Screen.height / 2 - 25, 500, 50), log, centeredStyle);
        }


        void Update() {
            if (!isLocalPlayer) return;

                        selected = ObjectManager.GetSelected();
            if (selected == null) return; // there is no object to interact with


            Matrix4x4 camMatrix = Camera.main.worldToCameraMatrix;

            if (refGUI.lockTransform) {
                CmdUpdateModality(1);
                Matrix4x4 step = prevMatrix * camMatrix.inverse;
                
                var g = ObjectManager.Get(selected.index);

                Matrix4x4 modelMatrix = Matrix4x4.TRS(g.transform.position, g.transform.rotation, new Vector3(1, 1, 1)); // get the object matrix
                modelMatrix = prevMatrix * modelMatrix; // transform the model matrix to the camera space matrix
                modelMatrix = step * modelMatrix; // transform the object's position and orientation
                modelMatrix = prevMatrix.inverse * modelMatrix; // put the object in the world coordinates
                g.transform.position = Utils.GetPositionn(modelMatrix);
                g.transform.rotation = Utils.GetRotationn(modelMatrix);
                this.gameObject.transform.GetComponent<HandleNetworkTransformations>().LockTransform(selected.index, Utils.GetPositionn(modelMatrix), Utils.GetRotationn(modelMatrix));

                setCurrentOperation(Utils.Transformations.Lock);
            }

            prevMatrix = camMatrix;
            //mode = MainController.control.transformationNow;
            //this.gameObject.transform.GetComponent<HandleNetworkFunctions>().SyncCamPosition(Camera.main.transform.position);



            if (countFrames % 35 == 0) {
                if (LeanTouch.Fingers.Count <= 0) {
                    translationZ = 0;
                    isTapForTransform = false;
                    if (!refGUI.lockTransform)
                        CmdUpdateModality(-1);
                }
            }
            countFrames++;
        }

        //const int OPERATION_NONE = 0;
        //const int OPERATION_MOVE = 1;
        //const int OPERATION_ROTATE = 2;
        //const int OPERATION_RESIZE = 3;
        //const int OPERATION_LOCK = 4;

        [SyncVar]
        public int currentOperation = 0;
        [Command]
        public void CmdSetCurrentOperation(int ope)
        {
            currentOperation = ope;
        }

        void setCurrentOperation(Utils.Transformations op) {
            CmdSetCurrentOperation((int)op);
        }

       protected virtual void OnEnable() {
            LeanTouch.OnFingerSet += OnFingerSet; // Hook into the events we need
            LeanTouch.OnGesture += OnGesture;
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerUp += OnFingerUp;
        }

        protected virtual void OnDisable() {
            LeanTouch.OnFingerSet -= OnFingerSet;    // Unhook the events
            LeanTouch.OnGesture -= OnGesture;
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerUp -= OnFingerUp;
        }

        private void OnFingerUp(LeanFinger finger) {
            translationZ = 0;
            gestureOperation = 0;
            isTapForTransform = false;
            setCurrentOperation((int)Utils.Transformations.None);
        }

        //public int GetIndex(GameObject g) {
        //    return g.GetComponent<ObjectGroupId>().index;
        //}


        float transFactor = 0.003f;
        private void OnFingerTap(LeanFinger finger) {
            if (selected == null) return; // there is no object to interact with
            //if (TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex] == 1) return;
            translationZ = 1;
            //if (gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected.Count != 0)
            isTapForTransform = true;

        }

        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (!isLocalPlayer) return;
            if (selected == null) return; // there is no object to interact with
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (LeanTouch.Fingers.Count != 1) return;
            //if (TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex] == 1) return;

            if (translationZ == 1)
                translationZ = 2;
            else
                CmdUpdateModality(0);

            if (translationZ == 2)
                
                isTapForTransform = true;

            //this.gameObject.GetComponent<NetHandleSelectionTouch>().unselectAllCount = 0;


                if (translationZ < 2) {
                    Vector3 right = Camera.main.transform.right.normalized * finger.ScreenDelta.x * transFactor * Utils.ToutchSensibility;
                    Vector3 up = Camera.main.transform.up.normalized * finger.ScreenDelta.y * transFactor * Utils.ToutchSensibility;
                    this.gameObject.transform.GetComponent<HandleNetworkTransformations>().ARTranslate(selected.index, Utils.PowVec3(right + up, 1.2f));
                } else if (translationZ == 2) {
                    //Vector3 avg = avgCenterOfObjects(gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected);
                    Vector3 translate = (selected.gameobject.transform.position - Camera.main.transform.position).normalized * finger.ScreenDelta.y * transFactor * Utils.ToutchSensibility; // obj pos - cam pos

                    //this.gameObject.GetComponent<HandleNetworkFunctions>().Translate(index, Utils.PowVec3(translate, 1.2f));
                    this.gameObject.GetComponent<HandleNetworkTransformations>().ARTranslate(selected.index, translate);
                }

            setCurrentOperation(Utils.Transformations.Translation);
        }

        public int gestureOperation = 0;
       
        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (!isLocalPlayer) return;
            if (selected == null) return; // there is no object to interact with
            if (LeanTouch.Fingers.Count != 2) return;
            //if (TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex] == 1) return;

            if (refGUI.lockTransform)
                CmdUpdateModality(2);
            else
                CmdUpdateModality(0);
            
            //Vector3 avg = avgCenterOfObjects(gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected);
            Vector3 axis, axisTwist;
			float angle = LeanGesture.GetScreenDelta(fingers).magnitude * 0.3f;
            
            float angleTwist = LeanGesture.GetTwistDegrees(fingers) * 0.8f;
            axisTwist = Camera.main.transform.forward.normalized;
            axis = Camera.main.transform.right.normalized * LeanGesture.GetScreenDelta(fingers).y + Camera.main.transform.up.normalized * -LeanGesture.GetScreenDelta(fingers).x;
            float pos = LeanGesture.GetScreenDelta(fingers).magnitude * 0.3f;
            float scale = LeanGesture.GetPinchScale(fingers);
            
            //foreach (int index in gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected) {
                var g = ObjectManager.Get(selected.index);
                float rotationMagnitude = Mathf.Max(Mathf.Abs(angleTwist) * 2, pos); ;
                float scallingMagnitude = Mathf.Abs(scale-1)*100;

                if (gestureOperation == 0 && (rotationMagnitude > 2 || scallingMagnitude > 2)) {
                    //log = angleTwist + "|" + pos + "|" + scallingMagnitude;
                    if (rotationMagnitude > scallingMagnitude) {
                        gestureOperation = 1;
                    } else {
                        gestureOperation = 2;
                    }
                }
                if (gestureOperation == 0) setCurrentOperation(Utils.Transformations.None);
                else if (gestureOperation == 1) setCurrentOperation(Utils.Transformations.Rotation);
                else if (gestureOperation == 2) setCurrentOperation(Utils.Transformations.Scale);

                if (gestureOperation != 2) {
                    Quaternion q = Quaternion.AngleAxis(angleTwist, axisTwist);
                    q *= Quaternion.AngleAxis(pos, axis);
                    //this.gameObject.GetComponent<HandleNetworkTransformations>().CmdRotStep(q);
                    this.gameObject.GetComponent<HandleNetworkTransformations>().ARRotate(selected.index, g.transform.position, axisTwist, angleTwist);
                    this.gameObject.GetComponent<HandleNetworkTransformations>().ARRotate(selected.index, g.transform.position, axis, pos);
                    //this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(index, avg, Vector3.Scale (axis, axisTwist), pos * angleTwist);
                }
                if (gestureOperation != 1) {
                    //Vector3 dir = g.transform.position - avg;
                    this.gameObject.GetComponent<HandleNetworkTransformations>().CmdARScale(selected.index, scale, g.transform.position);
                }
            //}
        }

        //private Vector3 avgCenterOfObjects(List<int> objects) {
        //    Vector3 avg = Vector3.zero;
        //    foreach (var index in objects) {
        //        var g = ObjectManager.Get(index);
        //        avg += g.transform.position;
        //    }
        //    return avg /= objects.Count;
        //}

        private void Scale(GameObject obj, float scale, Vector3 dir) {
            obj.transform.position += dir * (-1 + scale);
            obj.transform.localScale *= scale;
        }

        [ClientRpc]
        void RpcUpdateModality(int m) {
            modality = m;
        }

        [Command]
        void CmdUpdateModality(int m) {
            RpcUpdateModality(m);
        }
            
    }
}