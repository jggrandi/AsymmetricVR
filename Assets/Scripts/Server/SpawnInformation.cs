using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInformation : MonoBehaviour
{

    public List<Vector3> pos = new List<Vector3>();
    public Vector3 ghostPos = new Vector3();
    public List<Quaternion> rot = new List<Quaternion>();
    public List<float> scale = new List<float>();

    public GameObject table;

    private void Awake()
    {
        table = GameObject.Find("Table");
        if (table == null) return;

        CreatePositions();
        CreateGhostPos();
        CreateRotations();
        CreateScales();
    }

    void CreatePositions()
    {
        var height = 1.0f;
        var xDist = 1.0f;
        var zDist = 0.5f;

        var centerPos = new Vector3(table.transform.position.x, height, table.transform.position.z);

        pos.Add(centerPos + new Vector3(xDist, 0f, -zDist));
        pos.Add(centerPos + new Vector3(xDist, 0f, zDist));
        pos.Add(centerPos + new Vector3(-xDist, 0f, zDist));
        pos.Add(centerPos + new Vector3(-xDist, 0f, -zDist));
    }

    void CreateRotations()
    {
        var baseAngle = 30;

        rot.Add(Quaternion.AngleAxis(baseAngle, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))));
        rot.Add(Quaternion.AngleAxis(baseAngle*2, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))));
        rot.Add(Quaternion.AngleAxis(baseAngle*3, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))));
        rot.Add(Quaternion.AngleAxis(baseAngle*4, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))));
    }

    void CreateScales() {
        var scaleNormal = 1f;
        var scale1 = .25f;
        var scale2 = .5f;

        scale.Add(scaleNormal + scale1);
        scale.Add(scaleNormal - scale1);
        scale.Add(scaleNormal + scale2);
        scale.Add(scaleNormal - scale2);
    }

    void CreateGhostPos()
    {
        ghostPos = new Vector3(table.transform.position.x, 1f, table.transform.position.z);
    }
}