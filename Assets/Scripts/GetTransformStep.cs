using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTransformStep : MonoBehaviour {

    public Matrix4x4 rotationMatrix;
    public Matrix4x4 rotationMatrixPrev;
    public Matrix4x4 rotationMatrixStep;
    public Vector4 translation;
    public Matrix4x4 translationMatrix;
    public Matrix4x4 translationMatrixStep;
    public Matrix4x4 translationMatrixPrev;

    public Vector3 positionStep;
    public Quaternion rotationStep;

    //public GameObject objTransformed;
    //public GameObject cubao;
    // Use this for initialization
    void Start () {
        rotationMatrix = Matrix4x4.identity;
        rotationMatrixPrev = Matrix4x4.identity;
        rotationMatrixStep = Matrix4x4.identity;
        translationMatrix = Matrix4x4.identity;
        translation = new Vector4(0f, 0f, 0f, 1f);


        //cubao = GameObject.Find("CUBAO");
        //objTransformed = GameObject.Find("CubeAQUI");
    }
	
	// Update is called once per frame
	void Update () {

        rotationMatrix = Matrix4x4.Rotate( this.gameObject.transform.rotation);
        translationMatrix = Matrix4x4.Translate(this.gameObject.transform.position);

        translationMatrixStep = Matrix4x4.Inverse(translationMatrixPrev);
        translationMatrixStep = translationMatrix * translationMatrixStep;

        positionStep = Utils.GetPositionn(translationMatrixStep);

        //DebugMatrix(translationMatrixStep);
        rotationMatrixStep = Matrix4x4.Inverse(rotationMatrixPrev);
        rotationMatrixStep = rotationMatrix * rotationMatrixStep;

        rotationStep = Utils.GetRotationn(rotationMatrixStep);

        //cubao.transform.position += positionStep;
        //cubao.transform.rotation = rotationStep * cubao.transform.rotation;

        //objTransformed.transform.localPosition += Utils.GetPositionn(translationMatrixStep);
        //objTransformed.transform.Rotate(objTransformed.transform.localRotation.eulerAngles + Utils.GetRotationn(rotationMatrixStep).eulerAngles);


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
