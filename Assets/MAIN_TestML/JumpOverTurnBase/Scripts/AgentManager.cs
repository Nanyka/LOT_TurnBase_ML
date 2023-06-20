using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [Header("General control part")] [SerializeField]
    protected EnvironmentController m_Environment;

    [SerializeField] protected int m_Faction;
    [SerializeField] protected bool _isResetInstance;
    [SerializeField] protected List<SingleJumperController> m_JumpOverControllers;

    [Header("Attack part")] [SerializeField]
    protected UnitSkill m_UnitSkill;

    [Header("Reward part")] [SerializeField]
    protected float _unitReward;

    [SerializeField] private float _punishAmount;
    [SerializeField] protected float _movementCost = 0.01f;
    [SerializeField] protected float _visualGroupReward;

    private List<(int unitIndex, int prefer)> _movingOrder = new();
    protected SimpleMultiAgentGroup m_AgentGroup;
    protected int _responseCounter;
    private bool _isMoved;

    protected virtual void Start()
    {
        m_Environment.OnChangeFaction.AddListener(ToMyTurn);
        m_Environment.OnReset.AddListener(ResetAgents);
        m_Environment.OnPunishOppositeTeam.AddListener(GetPunish);
        m_Environment.OnOneTeamWin.AddListener(FinishRound);

        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var singleJumperController in m_JumpOverControllers)
        {
            m_AgentGroup.RegisterAgent(singleJumperController.GetAgent());
        }

        MultiJumperKickOff();
    }

    protected virtual void ResetAgents()
    {
        foreach (var agent in m_JumpOverControllers)
            agent.ResetAgent();

        m_AgentGroup.GroupEpisodeInterrupted();
        _visualGroupReward = 0f;
    }

    // Get punish whenever an agent jump over enemies
    protected void GetPunish(int faction)
    {
        if (faction == m_Faction)
            return;

        m_AgentGroup.AddGroupReward(-1f * _punishAmount);
        _visualGroupReward += -1f * _punishAmount;
    }

    // KICK-OFF this MLAgents environment
    protected void MultiJumperKickOff()
    {
        if (m_Faction == 0)
            m_Environment.KickOffEnvironment();
    }

    #region Ask for decision from agents

    protected virtual void ToMyTurn()
    {
        if (m_Environment.GetCurrFaction() != m_Faction)
            return;
        
        // reset all agent's moving state
        foreach (var jumperController in m_JumpOverControllers)
            jumperController.ResetMoveState();

        // reset counter before an iteration
        _responseCounter = 0;

        KickOffUnitActions(); // kick off unit action recursion
    }

    public virtual void KickOffUnitActions()
    {
        m_JumpOverControllers[_responseCounter].AskForAction();

        // Movement cost an amount of point
        m_AgentGroup.AddGroupReward(-1f * _movementCost);
        _visualGroupReward += -1f * _movementCost;
    }

    public virtual void CollectUnitResponse()
    {
        _responseCounter++;

        if (_responseCounter < m_JumpOverControllers.Count)
            KickOffUnitActions();
        else
            EndTurn();
    }

    #endregion

    protected virtual void EndTurn()
    {
        // Attack nearby enemy
        int successAttacks = 0;
        foreach (var agent in m_JumpOverControllers)
        {
            if (agent.GetJumpStep() == 0)
                continue;

            var attackPoints = m_UnitSkill.AttackPoints(agent.GetPosition(), agent.GetDirection(), agent.GetJumpStep());
            if (attackPoints == null)
                continue;

            foreach (var attackPoint in attackPoints)
            {
                // Debug.Log($"Attack at {attackPoint}");
                if (m_Environment.CheckEnemy(attackPoint, m_Faction))
                    successAttacks++;
            }

            if (successAttacks > 0)
            {
                agent.ChangeColor(successAttacks);
                m_AgentGroup.AddGroupReward(_unitReward * successAttacks *
                                            m_UnitSkill.GetSkillMagnitude(agent.GetJumpStep()));
                _visualGroupReward += _unitReward * successAttacks;

                m_Environment.OnPunishOppositeTeam.Invoke(GetFaction()); // punish the opposite team
                // Debug.Log($"Group reward {_visualGroupReward}");
                break;
            }
        }
        
        // call for the end-turn event
        m_Environment.ChangeFaction(_isResetInstance && successAttacks>0);
        m_Environment.OnChangeFaction.Invoke();
    }

    #region GET & SET

    public int GetFaction()
    {
        return m_Faction;
    }

    public EnvironmentController GetEnvironment()
    {
        return m_Environment;
    }

    protected virtual void FinishRound(int faction)
    {
        foreach (var agent in m_JumpOverControllers)
            agent.ResetAgent();

        m_AgentGroup.EndGroupEpisode();
    }

    #endregion

    #region OLD VERSION (DEDICATED)

    #region Supervisor agent

    // Use managerAgent to select agent in list and selected agent do its action
    public void SelectAgent(int agentIndex)
    {
        var checkReward = GetTotalReward();
        // m_Agent.AddReward(_idlePunish);
        m_JumpOverControllers[agentIndex].MoveDirection();
        checkReward = GetTotalReward() - checkReward;
        // m_Agent.AddReward(checkReward);
        // TODO: Use JumpOverController and this agent instead of two kind of agent;

        EndTurn();
    }

    #endregion

    #region GET & SET

    private float GetTotalReward()
    {
        return m_JumpOverControllers.Sum(x => x.GetAgent().GetCumulativeReward());
    }

    public virtual void RemoveAgent(SingleJumperController jumper)
    {
        m_JumpOverControllers.Remove(jumper);
    }

    #endregion

    #endregion
}