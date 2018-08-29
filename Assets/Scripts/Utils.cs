using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public static class Utils {
    public enum Transformations { None, Translation, Rotation, Scale, Lock };
    public enum PlayerType { VR, AR, None };


    //public static void FromMatrix4x4(this Transform transform, Matrix4x4 matrix)
    //{
    //	transform.localScale = matrix.GetScale();
    //	transform.rotation = matrix.GetRotation();
    //	transform.position = matrix.GetPosition();
    //}

    public static Quaternion GetRotationn(this Matrix4x4 matrix)
	{
		var qw = Mathf.Sqrt(1f + matrix.m00 + matrix.m11 + matrix.m22) / 2;
		var w = 4 * qw;
		var qx = (matrix.m21 - matrix.m12) / w;
		var qy = (matrix.m02 - matrix.m20) / w;
		var qz = (matrix.m10 - matrix.m01) / w;
		
		return new Quaternion(qx, qy, qz, qw);
	}


	public static System.Random random = new System.Random();
	public static float rand(){
		return (float)(random.NextDouble () * 2.0f - 1.0f);
	}

	public static Vector3 RandomUnitVector(){
		return new Vector3 (rand(), rand(),rand()).normalized;

	}

    public static float ToutchSensibility = 1.0f;

    public static void UpdateTouchSensibilty() {

        /*
         * MotoX2 420
         * S6 577
         * Nexeus5 445
         * iPad 350 ~
         * 
         * */
        ToutchSensibility = 350/Screen.dpi;
        if (ToutchSensibility < 1) {
            ToutchSensibility *= ToutchSensibility;
        }
    }

    public static Vector3 GetPositionn(this Matrix4x4 matrix)
	{
		var x = matrix.m03;
		var y = matrix.m13;
		var z = matrix.m23;
		
		return new Vector3(x, y, z);
	}
	
	public static Vector3 GetScalee(this Matrix4x4 m)
	{
		var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
		var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
		var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
		
		return new Vector3(x, y, z);
	}
	
	public static float[] ConvertToFloat(Matrix4x4 m)
	{
		float[] v = {
			m.GetColumn(0).x, m.GetColumn(0).y, m.GetColumn(0).z, m.GetColumn(0).w,
			m.GetColumn(1).x, m.GetColumn(1).y, m.GetColumn(1).z, m.GetColumn(1).w,
			m.GetColumn(2).x, m.GetColumn(2).y, m.GetColumn(2).z, m.GetColumn(2).w,
			m.GetColumn(3).x, m.GetColumn(3).y, m.GetColumn(3).z, m.GetColumn(3).w
		};
		return v;
	}

	public static Matrix4x4 ConvertToMatrix(float[] f)
	{
		Matrix4x4 m = new Matrix4x4();

		m.SetColumn (0, new Vector4 (f [0], f [1], f [2], f [3]));
		m.SetColumn (1, new Vector4 (f [4], f [5], f [6], f [7]));
		m.SetColumn (2, new Vector4 (f [8], f [9], f [10], f [11]));
		m.SetColumn (3, new Vector4 (f [12], f [13], f [14], f [15]));

		return m;
	}

    public static Vector3 PowVec3(Vector3 v, float p) {
        float length = v.magnitude;
        length = Mathf.Pow(length, p);
        return v.normalized * length;
    }

    //public static int GetIndex(GameObject g) {
    //    return g.GetComponent<ObjectGroupId>().index;
    //}

    public static float distMatrices(Matrix4x4 a, Matrix4x4 b)
	{
		float r = 0;
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				float v = a [i, j] - b [i, j];
				r += v * v;
			}
		}
		return (float)Math.Sqrt (r);
	}



    public static String ConvertToString(float[] m)
    {
        return "*0*" + m[0] + "," + m[4] + "," + m[8] + "," + m[12] + "\n" +
                "*1*" + m[1] + "," + m[5] + "," + m[9] + "," + m[13] + "\n" +
                "*2*" + m[2] + "," + m[6] + "," + m[10] + "," + m[14] + "\n" +
                "*3*" + m[3] + "," + m[7] + "," + m[11] + "," + m[15] + "\n";

    }

    public static String matrixString(Matrix4x4 matrix)
    {

        float[] m = ConvertToFloat(matrix);
        return "*0*" + m[0] + "," + m[4] + "," + m[8] + "," + m[12] + "\n" +
                "*1*" + m[1] + "," + m[5] + "," + m[9] + "," + m[13] + "\n" +
                "*2*" + m[2] + "," + m[6] + "," + m[10] + "," + m[14] + "\n" +
                "*3*" + m[3] + "," + m[7] + "," + m[11] + "," + m[15] + "\n";

    }
	/*public static float[] ConvertToFloat(float[,] m, int i, int size=16){
		float [] r = new float[size];
		for (int j=0; j<size; j++)
			r [j] = m [i,j];
		return r;
	}*/

    public static float[] ConvertToFloat(byte[] array, int offSet, int size)
    {

        float[] floats = new float[size / 4];

        for (int i = 0; i < size / 4; i++)
            floats[i] = BitConverter.ToSingle(array, i * 4 + offSet);

        return floats;
    }

    public static Matrix4x4 ConvertToMatrix(byte[] array, int offSet)
    {
        return ConvertToMatrix(ConvertToFloat(array, offSet, 64));
    }


    public static bool isNaN(Quaternion q)
    {
        return (
			float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w) ||
			float.IsInfinity(q.x) || float.IsInfinity(q.y) || float.IsInfinity(q.z) || float.IsInfinity(q.w)

		);
    }

    public static Quaternion NormalizeQuaternion(Quaternion q)
    {
        float sum = 0;
        for (int i = 0; i < 4; ++i)
            sum += q[i] * q[i];
        float magnitudeInverse = 1 / Mathf.Sqrt(sum);
        for (int i = 0; i < 4; ++i)
            q[i] *= magnitudeInverse;
        return q;
    }


    public static Color32 HexColor(int HexVal, float alpha)
    {
        byte B = (byte)((HexVal >> 24) & 0xFF);
        byte G = (byte)((HexVal >> 16) & 0xFF);
        byte R = (byte)((HexVal >> 8) & 0xFF);
        byte A = (byte)((HexVal) & 0xFF);
        return new Color32(R, G, B, (byte)(int)(A * alpha));
    }

    /*public static Matrix4x4 matrix(Quaternion q)
    {

        Matrix4x4 a = new Matrix4x4();
        a.SetRow(0, new Vector4(q.w, q.z, -q.y, q.x));
        a.SetRow(1, new Vector4(-q.z, q.w, q.x, q.y));
        a.SetRow(2, new Vector4(q.y, -q.x, q.w, q.z));
        a.SetRow(3, new Vector4(-q.x, -q.y, -q.z, q.w));

        Matrix4x4 b = new Matrix4x4();
        b.SetRow(0, new Vector4(q.w, q.z, -q.y, -q.x));
        b.SetRow(1, new Vector4(-q.z, q.w, q.x, -q.y));
        b.SetRow(2, new Vector4(q.y, -q.x, q.w, -q.z));
        b.SetRow(3, new Vector4(q.x, q.y, q.z, q.w));

        return b * a;
    }*/


/*	public static Interpolate(this Vector3 pR, this Quaternion rR, Vector3 pA, Quaternion rA, Vector3 pB, Quaternion rB, float f){
		rR = Quaternion.Slerp(rA, rB, f);
		pR = Vector3.Lerp(pA, pB, f); 
	}
*/

    static public Texture2D MakeTexture(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    static public void setTimeout(Action TheAction, int Timeout) {
        Debug.Log(Timeout);
        Thread t = new Thread(
            () => {
                Debug.Log("A");
                Thread.Sleep(Timeout);
                Debug.Log("B");
                TheAction.Invoke();
            }
        );
        t.Start();
    }


    static public List<int> randomizeVector(int size)
    {

        var randomNumbers = new List<int>();
        var numbers = new List<int>();


        for (int i = 0; i < size; i++) // numbers  to be randomized
            numbers.Add(i);

        for (int i = 0; i < size; i++)
        {
            var thisNumber = UnityEngine.Random.Range(0, numbers.Count);
            randomNumbers.Add(numbers[thisNumber]);
            numbers.RemoveAt(thisNumber);
        }
        return randomNumbers;
    }

    static public List<int> randomizeVector(int begin, int end)
    {

        var randomNumbers = new List<int>();
        var numbers = new List<int>();


        for (int i = begin; i < end; i++) // numbers  to be randomized
            numbers.Add(i);

        for (int i = begin; i < end; i++)
        {
            var thisNumber = UnityEngine.Random.Range(0, numbers.Count);
            randomNumbers.Add(numbers[thisNumber]);
            numbers.RemoveAt(thisNumber);
        }
        return randomNumbers;
    }


    static public List<int> randomizeVector(int[] vec)
    {

        var randomNumbers = new List<int>();
        var numbers = new List<int>();


        for (int i = 0; i < vec.Length; i++) // numbers  to be randomized
            numbers.Add(vec[i]);

        for (int i = 0; i < vec.Length; i++)
        {
            var thisNumber = UnityEngine.Random.Range(0, numbers.Count);
            randomNumbers.Add(numbers[thisNumber]);
            numbers.RemoveAt(thisNumber);
        }
        return randomNumbers;
    }

    static public List<int> RandomizeList(List<int> _list)
    {

        var randomNumbers = new List<int>();
        var numbers = new List<int>();


        for (int i = 0; i < _list.Count; i++) // numbers  to be randomized
            numbers.Add(_list[i]);

        for (int i = 0; i < _list.Count; i++)
        {
            var thisNumber = UnityEngine.Random.Range(0, numbers.Count);
            randomNumbers.Add(numbers[thisNumber]);
            numbers.RemoveAt(thisNumber);
        }
        return randomNumbers;
    }



    public static List<int> allPermutations = new List<int>();

    public static int[] selectTaskSequence(int uid, int tasks)
    { //Select an permutation based on the ID given
        allPermutations.Clear();
        int[] elements = new int[tasks];
        for (int i = 0; i < tasks; i++)
            elements[i] = i;
        permute(elements, 0, tasks); // Generate all permutations

        int permutationIndex = uid % (allPermutations.Count / tasks); // the beginning of the sequence to be taken from the list of all permutations

        int[] thePermutation = new int[tasks];
        for (int i = 0; i < tasks; i++)
            thePermutation[i] = allPermutations[permutationIndex * tasks + i]; //store the permutation selected

        return thePermutation;
    }

    public static void permute(int[] arry, int i, int n)
    {
        int j;
        if (i == n)
            for (int k = 0; k < arry.Length; k++)
                allPermutations.Add(arry[k]);  // Acumulate all permutations in an List
        else
        {
            for (j = i; j < n; j++)
            {
                swap(ref arry[i], ref arry[j]);
                permute(arry, i + 1, n);
                swap(ref arry[i], ref arry[j]); //backtrack
            }
        }
    }

    public static void swap(ref int a, ref int b)
    {
        int tmp;
        tmp = a;
        a = b;
        b = tmp;
    }

    public static List<int> FitVectorIntoAnother(int size1, int size2)
    {
        int count = 0;
        int i = 0;
        List<int> fittedList = new List<int>();

        var rest = size1 % size2;
        var times = (int)(size1 / size2);

        while (count < times)
        {
            if (i < size2)
            {
                fittedList.Add(i);
                i++;
            }
            else
            {
                count++;
                i = 0;
            }
        }

        for (int ii = 0; ii < rest; ii++)
            fittedList.Add(ii);

        return fittedList;
    }


    public static float AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis)
    {
        Vector3 right = Vector3.Cross(axis, forward);
        forward = Vector3.Cross(right, axis);
        return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward));
    }

    // The angle between dirA and dirB around axis
    public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        // Project A and B onto the plane orthogonal target axis
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);

        // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);

        // Return angle multiplied with 1 or -1
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }


}
