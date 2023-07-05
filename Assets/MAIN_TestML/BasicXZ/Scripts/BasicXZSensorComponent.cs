using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using Unity.MLAgentsExamples;
using UnityEngine;

public class BasicXZSensorComponent : SensorComponent
{
    public BasicXZController _controller;
    
    public override ISensor[] CreateSensors()
    {
        return new ISensor[] { new BasicXZSensor(_controller) };
    }
}

public class BasicXZSensor : SensorBase
{
    public BasicXZController _controller;

    public BasicXZSensor(BasicXZController controller)
    {
        _controller = controller;
    }
    
    public override void WriteObservation(float[] output)
    {
        // One-hot encoding of the position
        Array.Clear(output, 0, output.Length);
        output[_controller._currIndex] = 1;
        output[_controller._smallIndex + _controller.GetLengthOfList()] = 1;
        output[_controller._largeIndex + (_controller.GetLengthOfList() * 2)] = 1;
        // Debug.Log($"Position of index {_controller._currIndex} is {output[_controller._currIndex]}");
    }

    /// <inheritdoc/>
    public override ObservationSpec GetObservationSpec()
    {
        return ObservationSpec.Vector(_controller.GetLengthOfList()*3);
    }

    /// <inheritdoc/>
    public override string GetName()
    {
        return "BasicXZ";
    }

}
