using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class TestMatch3 : MonoBehaviour, IActuator
{
    public void OnActionReceived(ActionBuffers actionBuffers)
    {
        Debug.Log("In Action received");
    }

    public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        Debug.Log("In Action received");
    }

    public void Heuristic(in ActionBuffers actionBuffersOut)
    {
        Debug.Log("In Action received");
    }

    public void ResetData()
    {
        Debug.Log("In Action received");
    }

    public ActionSpec ActionSpec { get; }
    public string Name { get; }
}
