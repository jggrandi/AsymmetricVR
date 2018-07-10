using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class GetTransformStep : MonoBehaviour
    {

        private Matrix4x4 rotationMatrix;
        private Matrix4x4 rotationMatrixPrev;
        private Matrix4x4 rotationMatrixStep;
        private Vector4 translation;
        private Matrix4x4 translationMatrix;
        private Matrix4x4 translationMatrixStep;
        private Matrix4x4 translationMatrixPrev;
        private float scale;

        public Vector3 positionStep;
        public Quaternion rotationStep;
        public float scaleStep;

        void Start()
        {
            rotationMatrix = Matrix4x4.identity;
            rotationMatrixPrev = Matrix4x4.identity;
            rotationMatrixStep = Matrix4x4.identity;
            translationMatrix = Matrix4x4.identity;
            translationMatrixStep = Matrix4x4.identity;
            translationMatrixPrev = Matrix4x4.identity;
            translation = new Vector4(0f, 0f, 0f, 1f);
            scale = 0f;
        }

        void Update()
        {

            rotationMatrix = Matrix4x4.Rotate(this.gameObject.transform.rotation);
            translationMatrix = Matrix4x4.Translate(this.gameObject.transform.position);

            translationMatrixStep = Matrix4x4.Inverse(translationMatrixPrev);
            translationMatrixStep = translationMatrix * translationMatrixStep;
            positionStep = Utils.GetPositionn(translationMatrixStep);

            rotationMatrixStep = Matrix4x4.Inverse(rotationMatrixPrev);
            rotationMatrixStep = rotationMatrix * rotationMatrixStep;
            rotationStep = Utils.GetRotationn(rotationMatrixStep);


            //Start to implement bimanual scale
            var thishand = this.gameObject.GetComponent<Hand>();
            

            translationMatrixPrev = translationMatrix;
            rotationMatrixPrev = rotationMatrix;
        }

        void DebugMatrix(Matrix4x4 mat)
        {
            Debug.Log(mat.m00 + " " + mat.m01 + " " + mat.m02 + " " + mat.m03 + "\n" +
            mat.m10 + " " + mat.m11 + " " + mat.m12 + " " + mat.m13 + "\n" +
            mat.m20 + " " + mat.m21 + " " + mat.m22 + " " + mat.m23 + "\n" +
            mat.m30 + " " + mat.m31 + " " + mat.m32 + " " + mat.m33 + "\n");
        }
    }
}