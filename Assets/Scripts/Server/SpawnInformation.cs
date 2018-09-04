﻿using System.Collections;
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

    float baseAngle = 30.0f;

    private void Awake()
    {
        table = GameObject.Find("TablePosition");
        if (table == null) return;

        CreatePositions();
        CreateGhostPos();
        CreateGhostRot();
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
        rot.Add(Quaternion.AngleAxis(baseAngle*3, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))));
        rot.Add(Quaternion.AngleAxis(baseAngle*4, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))));
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

    void CreateGhostPos()
    {
        ghostPos.Clear();
        var centerPos = new Vector3(table.transform.position.x, 1.0f, table.transform.position.z);
        for (int i = 0; i < 7; i++) // tree training + 4 first trials
            ghostPos.Add(new Vector3(table.transform.position.x, 1.0f, table.transform.position.z));
        for (int i = 0; i < 4; i++)
        {

            Vector3 newPos = -pos[i];
            //newPos = new Vector3(newPos.x * (-1), newPos.y, centerPos.y * (-1));

            ghostPos.Add(newPos);
        }             
    }

    void CreateGhostRot()
    {
        int j = 1;
        for (int i = 0; i < 11; i++)
        {
            ghostRot.Add(Quaternion.AngleAxis(baseAngle * (j), new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))));
            if (j % 4 == 0) j = 1;
            else j++;
        }
    }

}