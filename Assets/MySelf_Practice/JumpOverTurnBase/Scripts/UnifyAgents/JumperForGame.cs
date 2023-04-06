using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents.Policies;
using UnityEngine;

public class JumperForGame : SingleJumperController
{
    public BehaviorParameters m_BehaviorParameters;
    public DummyAction InferMoving;

    #region INFER PHASE

    public override void AskForAction()
    {
        m_Agent?.RequestDecision();
    }

    public override void ResponseAction(int direction)
    {
        InferMoving.Action = direction;
        InferMoving.CurrentPos = _mTransform.position;
        GetPositionByDirection(InferMoving.Action);
        m_AgentManager.CollectUnitResponse();
    }

    private void GetPositionByDirection(int direction)
    {
        var curPos = InferMoving.CurrentPos;
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
            // if (_environmentController.CheckObjectInTeam(newPos, m_AgentManager.GetFaction()))
            // {
            //     InferMoving.JumpOverAt[jumpCount] = newPos;
            //     jumpCount++;
            // }
            // else
            //     jumpCount++;

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

    #endregion

    #region ACTION PHASE

    public void ConductSelectedAction(DummyAction selectedAction)
    {
        MarkAsUsedThisTurn();
        
        // Change agent direction before the agent jump to the new position
        if (selectedAction.TargetPos != _mTransform.position)
            _rotatePart.forward = selectedAction.TargetPos - _mTransform.position;

        _mTransform.position = selectedAction.TargetPos;
        
        // Ask for the next inference
        m_AgentManager.KickOffUnitActions();
    }

    #endregion
}