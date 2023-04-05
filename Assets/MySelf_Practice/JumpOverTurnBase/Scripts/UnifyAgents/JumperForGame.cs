using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents.Policies;
using UnityEngine;

public class JumperForGame : SingleJumperController
{
    public BehaviorParameters m_BehaviorParameters;
    public DummyAction InferMoving;

    public override void AskForAction()
    {
        m_Agent?.RequestDecision();
    }

    public override void ResponseAction(int direction)
    {
        _currentDirection = direction;
        GetPositionByDirection(_currentDirection);
    }

    private void GetPositionByDirection(int direction)
    {
        var curPos = _mMoving.targetPos;
        var newPos = curPos + DirectionToVector(direction);
        MovingPath(curPos, newPos, direction, 0);

        if (InferMoving.TargetPos != _mTransform.position)
            InferMoving.Direction = InferMoving.TargetPos - _mTransform.position;
    }

    private void MovingPath(Vector3 curPos, Vector3 newPos, int direction, int jumpCount)
    {
        if (CheckAvailableMove(newPos))
        {
            InferMoving.TargetPos = jumpCount == 0 ? newPos : curPos;
            InferMoving.JumpCount = jumpCount;
            return;
        }

        if (CheckAvailableMove(newPos + DirectionToVector(direction)))
        {
            if (_environmentController.CheckObjectInTeam(newPos, m_AgentManager.GetFaction()))
            {
                InferMoving.JumpOverAt[jumpCount] = newPos;
                jumpCount++;
            }
            else
                jumpCount++;

            curPos = newPos + DirectionToVector(direction);
            newPos = curPos + DirectionToVector(direction);

            MovingPath(curPos, newPos, direction, jumpCount);
        }

        InferMoving.TargetPos = curPos;
        InferMoving.JumpCount = jumpCount;
    }

    public override int GetJumpStep()
    {
        return InferMoving.JumpCount;
    }

    public override Vector3 GetDirection()
    {
        return InferMoving.Direction;
    }

    public void SetBrain(NNModel brain)
    {
        m_BehaviorParameters.Model = brain;
    }
}