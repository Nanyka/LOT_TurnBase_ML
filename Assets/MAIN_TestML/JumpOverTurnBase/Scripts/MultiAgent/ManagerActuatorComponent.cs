using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class ManagerActuatorComponent : ActuatorComponent
{
    [SerializeField] private AgentManager _controller;
    private ActionSpec _actionSpec = ActionSpec.MakeDiscrete(3);

    public override IActuator[] CreateActuators()
    {
        return new IActuator[] {new ManagerActuator(_controller)};
    }

    public override ActionSpec ActionSpec
    {
        get { return _actionSpec; }
    }
}

public class ManagerActuator : IActuator
{
    private AgentManager _controller;
    ActionSpec m_ActionSpec;

    public ManagerActuator(AgentManager controller)
    {
        _controller = controller;
        m_ActionSpec = ActionSpec.MakeDiscrete(3);
    }

    public ActionSpec ActionSpec
    {
        get { return m_ActionSpec; }
    }

    /// <inheritdoc/>
    public String Name
    {
        get { return "JumpOverManager"; }
    }

    public void ResetData()
    {
    }

    public void OnActionReceived(ActionBuffers actionBuffers)
    {
        var selection = actionBuffers.DiscreteActions[0];
        var agentIndex = 0;

        switch (selection)
        {
            case 0:
                agentIndex = 0;
                break;
            case 1:
                agentIndex = 1;
                break;
            case 2:
                agentIndex = 2;
                break;
        }

        _controller.SelectAgent(agentIndex);
    }

    public void Heuristic(in ActionBuffers actionBuffersOut)
    {
        var pressA = Input.GetKeyDown(KeyCode.A);
        var pressS = Input.GetKeyDown(KeyCode.S);
        var discreteActions = actionBuffersOut.DiscreteActions;
        
        if (pressA)
        {
            discreteActions[0] = 0;
        }
        else if (pressS)
        {
            discreteActions[0] = 1;
        }
        else
        {
            discreteActions[0] = 2;
        }
    }

    public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
    }
}
