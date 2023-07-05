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

    private bool _isDie;

    public override void OnEnable()
    {
        base.OnEnable();
        _unitEntity.OnUnitDie.AddListener(UnitDie);
    }

    #region INFER PHASE

    /// <summary>
    ///   <para>Send an action to agent, instead of infer from a brain, and ask for its reaction</para>
    /// </summary>
    public DummyAction RespondFromAction(int action)
    {
        InferMoving.Action = action;
        InferMoving.CurrentPos = _mTransform.position;
        GetPositionByDirection(InferMoving.Action);
        return InferMoving;
    }

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
        var movement = _environmentController.GetMovementCalculator()
            .MovingPath(_mTransform.position, direction, 0, 0);
        InferMoving.TargetPos = movement.returnPos;
        InferMoving.JumpCount = movement.jumpCount;

        if (InferMoving.TargetPos != _mTransform.position)
            InferMoving.Direction = InferMoving.TargetPos - _mTransform.position;
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
        InferMoving = selectedAction;
        
        // Change agent direction before the agent jump to the new position
        if (selectedAction.TargetPos != _mTransform.position)
            _rotatePart.forward = selectedAction.TargetPos - _mTransform.position;

        StartCoroutine(MoveOverTime());
    }
    
    private IEnumerator MoveOverTime()
    {
        while (transform.position != InferMoving.TargetPos)
        {
            _mTransform.position = Vector3.MoveTowards(transform.position, InferMoving.TargetPos, 10f * Time.deltaTime);
            yield return null;
        }

        // Ask for the next inference
        m_AgentManager.KickOffUnitActions();
    }

    public void Attack()
    {
        _unitEntity.Attack(this);
    }

    public void ShowAttackRange(IEnumerable<Vector3> attackRange)
    {
        _unitEntity.ShowAttackRange(attackRange);
    }

    #endregion

    #region GET

    public (string name, int health, int damage, int power) GetUnitInfo()
    {
        return (name ,_unitEntity.GetCurrentHealth(), _unitEntity.GetAttackDamage(), InferMoving.JumpCount);
    }

    public (Vector3 midPos, Vector3 direction, int jumpStep, int faction) GetCurrentState()
    {
        return (_mTransform.position, _rotatePart.forward, InferMoving.JumpCount, m_AgentManager.GetFaction());
    }

    public EnvironmentController GetEnvironment()
    {
        return m_AgentManager.GetEnvironment();
    }

    public UnitEntity GetEntity()
    {
        return _unitEntity;
    }

    public override void ResetAgent()
    {
        base.ResetAgent();
        _unitEntity.ResetEntity();
    }

    private void UnitDie()
    {
        m_AgentManager.RemoveAgent(this);
        Destroy(gameObject,1f);
    }

    #endregion
}