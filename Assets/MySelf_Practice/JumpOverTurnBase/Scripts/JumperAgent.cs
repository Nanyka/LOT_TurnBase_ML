using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Sensors.Reflection;
using UnityEngine;

public class JumperAgent : Agent
{
    private float _fromTop;
    private float _fromBottom;
    private float _fromLeft;
    private float _fromRight;

    public void UpdatePosition(Vector3 currentPos, float maxCol, float maxRow)
    {
        _fromTop = maxCol - currentPos.z;
        _fromBottom = -1f * maxCol + currentPos.z;
        _fromRight = maxRow - currentPos.x;
        _fromLeft = -1f * maxRow + currentPos.x;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_fromTop);
        sensor.AddObservation(_fromBottom);
        sensor.AddObservation(_fromLeft);
        sensor.AddObservation(_fromRight);
    }
}
