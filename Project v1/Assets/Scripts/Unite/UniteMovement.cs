using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Unite))]
public class UniteMovement : MonoBehaviour
{

    private Transform target;
    private int wavePointIndex = 0;
 

    private Unite unit;


    void OnEnable()
    {
        unit = GetComponent<Unite>();

        target = Waypoints.points[0];

        wavePointIndex = 0;

    }

void Update()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * unit.movementSpeed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 1f)
        {
            GetNextWaypoint();
        }
    }


    void GetNextWaypoint()
    {
        if (wavePointIndex >= Waypoints.points.Length - 1)
        {
            EndPath();
            return;
        }

        wavePointIndex++;
        target = Waypoints.points[wavePointIndex];
    }


    void EndPath()
    {
        PlayerStats.Instance.DecreaseLife();

        unit.Remove();
    }
}
