using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class JumpOverActuatorComponent : ActuatorComponent
{
    [SerializeField] private JumpOverController _controller;
    private ActionSpec _actionSpec = ActionSpec.MakeDiscrete(5);

    public override IActuator[] CreateActuators()
    {
        return new IActuator[] {new JumpOverActuator(_controller)};
    }

    public override ActionSpec ActionSpec
    {
        get { return _actionSpec; }
    }
}

public class JumpOverActuator : IActuator
{
    private JumpOverController _controller;
    ActionSpec m_ActionSpec;

    public JumpOverActuator(JumpOverController controller)
    {
        _controller = controller;
        m_ActionSpec = ActionSpec.MakeDiscrete(5);
    }

    public ActionSpec ActionSpec
    {
        get { return m_ActionSpec; }
    }

    /// <inheritdoc/>
    public String Name
    {
        get { return "JumpOver"; }
    }

    public void ResetData()
    {
    }

    public void OnActionReceived(ActionBuffers actionBuffers)
    {
        var movement = actionBuffers.DiscreteActions[0];
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

        _controller.MoveDirection(direction);
    }

    public void Heuristic(in ActionBuffers actionBuffersOut)
    {
        var directionX = Input.GetAxis("Horizontal");
        var directionZ = Input.GetAxis("Vertical");
        var discreteActions = actionBuffersOut.DiscreteActions;
        if (Mathf.Approximately(directionX, 0.0f) && Mathf.Approximately(directionZ, 0.0f))
        {
            discreteActions[0] = 0;
        }
        else if (Mathf.Approximately(directionZ, 0.0f))
        {
            var signX = Mathf.Sign(directionX);
            discreteActions[0] = signX < 0 ? 1 : 2;
        }
        else if (Mathf.Approximately(directionX, 0.0f))
        {
            var signZ = Mathf.Sign(directionZ);
            discreteActions[0] = signZ < 0 ? 3 : 4;
        }
    }

    public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
    }
}