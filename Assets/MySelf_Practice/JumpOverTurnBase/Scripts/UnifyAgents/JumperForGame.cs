using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents.Policies;
using UnityEngine;

public class JumperForGame : SingleJumperController, IGetUnitInfo
{
    public BehaviorParameters m_BehaviorParameters;
    public DummyAction InferMoving;

    [SerializeField] private UnitEntity _unitEntity;

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
        
        if (CheckInBoundary(newPos))
        {
            if (CheckAvailableMove(newPos))
            {
                InferMoving.TargetPos = jumpCount == 0 ? newPos : curPos;
                InferMoving.JumpCount = jumpCount;
                return;
            }
        }
        else
        {
            InferMoving.TargetPos = curPos;
            InferMoving.JumpCount = jumpCount;
            return;
        }

        if (CheckAvailableMove(newPos + DirectionToVector(direction)))
        {
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

        StartCoroutine(MoveOverTime(selectedAction.TargetPos));
    }
    
    private IEnumerator MoveOverTime(Vector3 targetPos)
    {
        while (transform.position != targetPos)
        {
            _mTransform.position = Vector3.MoveTowards(transform.position, targetPos, 10f * Time.deltaTime);
            yield return null;
        }

        // Ask for the next inference
        m_AgentManager.KickOffUnitActions();
    }

    #endregion

    #region GET

    public (string name, int health, int damage, int power) GetUnitInfo()
    {
        return (name ,3, 1, InferMoving.JumpCount);
    }

    public int GetAttackDamage()
    {
        return _unitEntity.GetAttackDamage();
    }

    #endregion
}