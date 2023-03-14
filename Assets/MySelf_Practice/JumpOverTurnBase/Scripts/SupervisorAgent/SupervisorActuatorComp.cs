using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class SupervisorActuatorComp : ActuatorComponent
{
    [SerializeField] private Supervisor _controller;
    private ActionSpec _actionSpec = ActionSpec.MakeDiscrete(5);

    public override IActuator[] CreateActuators()
    {
        return new IActuator[] {new SupervisorActuator(_controller)};
    }

    public override ActionSpec ActionSpec
    {
        get { return _actionSpec; }
    }
}

public class SupervisorActuator : IActuator
{
    private Supervisor _controller;
    ActionSpec m_ActionSpec;

    public SupervisorActuator(Supervisor controller)
    {
        _controller = controller;
        m_ActionSpec = ActionSpec.MakeDiscrete(3,5);
    }

    public ActionSpec ActionSpec
    {
        get { return m_ActionSpec; }
    }

    /// <inheritdoc/>
    public String Name
    {
        get { return "Supervisor"; }
    }

    public void ResetData()
    {
    }

    public void OnActionReceived(ActionBuffers actionBuffers)
    {
        var movement = actionBuffers.DiscreteActions[1];
        var direction = 0;

        switch (movement)
        {
            case 0:
                direction = 0;
                break;
            case 1:
                direction = 1;
                break;
            case 2:
                direction = 2;
                break;
            case 3:
                direction = 3;
                break;
            case 4:
                direction = 4;
                break;
        }

        _controller.MoveUnit(actionBuffers.DiscreteActions[0],direction);
    }

    public void Heuristic(in ActionBuffers actionBuffersOut)
    {
        var directionX = Input.GetAxis("Horizontal");
        var directionZ = Input.GetAxis("Vertical");
        var discreteActions = actionBuffersOut.DiscreteActions;
        discreteActions[0] = 0;

        if (Mathf.Approximately(directionX, 0.0f) && Mathf.Approximately(directionZ, 0.0f))
        {
            discreteActions[1] = 0;
        }
        else if (Mathf.Approximately(directionZ, 0.0f))
        {
            var signX = Mathf.Sign(directionX);
            discreteActions[1] = signX < 0 ? 1 : 2;
        }
        else if (Mathf.Approximately(directionX, 0.0f))
        {
            var signZ = Mathf.Sign(directionZ);
            discreteActions[1] = signZ < 0 ? 3 : 4;
        }
    }

    public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
    }
}

