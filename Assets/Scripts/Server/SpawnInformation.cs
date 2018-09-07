using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnInformation : NetworkBehaviour
{

    public List<Vector3> pos = new List<Vector3>();
    public List<Quaternion> rot = new List<Quaternion>();
    public List<Vector3> ghostPos = new List<Vector3>();
    public List<Quaternion> ghostRot = new List<Quaternion>();
    public List<float> scale = new List<float>();

    public GameObject table;

    float baseAngle = 45.0f;

    private void Awake()
    {
        table = GameObject.Find("TablePosition");
        if (table == null) return;

        CreatePositions();
        CreateRotations();
        CreateScales();
    }

    void CreatePositions()
    {
        var height = 1.0f;
        var xDist = 0.6f;
        var zDist = 0.2f;

        var centerPos = new Vector3(table.transform.position.x, height, table.transform.position.z);

        pos.Add(centerPos + new Vector3(xDist, 0f, -zDist));
        pos.Add(centerPos + new Vector3(xDist, 0f, zDist));
        pos.Add(centerPos + new Vector3(-xDist, 0f, zDist));
        pos.Add(centerPos + new Vector3(-xDist, 0f, -zDist));
    }

    void CreateRotations()
    {
        rot.Add(Quaternion.AngleAxis(baseAngle, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))));
        rot.Add(Quaternion.AngleAxis(baseAngle*2, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))));
    }

    void CreateScales() {
        var scaleNormal = .0625f;
        var scale1 = .015f;
        var scale2 = .025f;

        scale.Add(scaleNormal + scale1);
        scale.Add(scaleNormal - scale1);
        scale.Add(scaleNormal + scale2);
        scale.Add(scaleNormal - scale2);
    }

}