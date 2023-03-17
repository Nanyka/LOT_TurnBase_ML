using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private int m_Faction;
    [SerializeField] private List<SingleJumperController> m_JumpOverControllers;
    [SerializeField] private float _unitReward;
    [SerializeField] private float _punishAmount;
    [SerializeField] private float _idlePunish = 0.01f;

    private SimpleMultiAgentGroup m_AgentGroup;
    private int _responseCounter;
    private int _currentJump;
    private bool _isMoved;

    private void Start()
    {
        _environmentController.OnChangeFaction.AddListener(ToMyTurn);
        _environmentController.OnReset.AddListener(ResetAgents);
        _environmentController.OnPunishOppositeTeam.AddListener(GetPunish);
        _environmentController.OnOneTeamWin.AddListener(FinishRound);

        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var singleJumperController in m_JumpOverControllers)
        {
            m_AgentGroup.RegisterAgent(singleJumperController.GetAgent());
        }

        MultiJumperKickOff();
    }

    private void ResetAgents()
    {
        _currentJump = 0;
        foreach (var agent in m_JumpOverControllers)
            agent.ResetAgent();

        m_AgentGroup.GroupEpisodeInterrupted();
    }

    // Get punish whenever an agent jump over enemies
    private void GetPunish(int faction, int overEnemy)
    {
        if (faction == m_Faction)
            return;
        
        m_AgentGroup.AddGroupReward(-1f*_punishAmount);
    }

    // KICK-OFF this MLAgents environment
    private void MultiJumperKickOff()
    {
        if (m_Faction == 0)
            _environmentController.KickOffEnvironment();
    }

    #region Ask for decision from agents

    private void ToMyTurn()
    {
        if (_environmentController.GetCurrFaction() != m_Faction)
            return;

        // Ask each agent
        foreach (var agent in m_JumpOverControllers)
        {
            agent.AskForAction(); // Assign selected agent to make decision
        }

        // collect actions from agents
        _responseCounter = 0;
        StartCoroutine(WaitForAgents());
    }

    private IEnumerator WaitForAgents()
    {
        yield return new WaitUntil(() => _responseCounter == m_JumpOverControllers.Count);
        _isMoved = false;

        // if the selected agent do not choose idle --> action & break
        foreach (var agent in m_JumpOverControllers)
        {
            // v10: go one agent in one turn and suffer an existential penalty
            m_AgentGroup.AddGroupReward(-1f*_idlePunish);
            agent.MoveDirection();
        }

        EndTurn();
    }

    public void ResponseBack()
    {
        _responseCounter++;
    }

    #endregion

    private void EndTurn()
    {
        // call for the end-turn event
        _environmentController.ChangeFaction();
        _environmentController.OnChangeFaction.Invoke();
    }

    #region GET & SET

    public int GetFaction()
    {
        return m_Faction;
    }

    public void ContributeGroupReward((Vector3 targetPos,int jumpStep,int overEnemy) unitAction)
    {
        var rewardMultiplier = unitAction.jumpStep;
        m_AgentGroup.AddGroupReward(_unitReward * (rewardMultiplier + Mathf.Pow(1 + 1f, rewardMultiplier)));
        
        _currentJump += rewardMultiplier;

        if (_environmentController.CheckWinCondition(_currentJump))
            _environmentController.OnOneTeamWin.Invoke(m_Faction);
    }

    private void FinishRound(int faction)
    {
        if (faction == m_Faction)
            m_AgentGroup.AddGroupReward(_currentJump*_unitReward);
        else
            m_AgentGroup.AddGroupReward(-1f*_currentJump*_unitReward);

        _currentJump = 0;
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

    public float GetMaxReward()
    {
        return m_JumpOverControllers.Max(x => x.GetAgent().GetCumulativeReward());
    }

    public float GetMinReward()
    {
        return m_JumpOverControllers.Min(x => x.GetAgent().GetCumulativeReward());
    }

    private float GetTotalReward()
    {
        return m_JumpOverControllers.Sum(x => x.GetAgent().GetCumulativeReward());
    }

    public float GetIdlePunish()
    {
        return _idlePunish;
    }

    #endregion
    

    #endregion
}