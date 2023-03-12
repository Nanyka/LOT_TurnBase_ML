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
    [SerializeField] private float _punishAmount;
    [SerializeField] private float _idlePunish;

    private int _responseCounter;
    private int _steps;
    private bool _isMoved;

    private void Start()
    {
        _environmentController.OnChangeFaction.AddListener(ToMyTurn);
        _environmentController.OnReset.AddListener(ResetAgents);
        _environmentController.OnPunishOppositeTeam.AddListener(GetPunish);

        MultiJumperKickOff();
    }

    private void ResetAgents()
    {
        foreach (var agent in m_JumpOverControllers)
        {
            agent.ResetAgent();
        }
    }

    // Get punish whenever an agent jump over enemies
    private void GetPunish(int faction, int overEnemy)
    {
        if (faction == m_Faction)
            return;

        foreach (var agent in m_JumpOverControllers)
        {
            agent.Punish(_punishAmount * overEnemy);
        }
    }

    // KICK-OFF function of this MLAgents environment
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
        // _isMoved = false;

        // if the selected agent do not choose idle --> action & break
        foreach (var agent in m_JumpOverControllers)
        {
            // if (agent.GetCurrentAction() == 0)
            //     agent.Punish(_idlePunish);
            // if (agent.GetCurrentAction() != 0 && _isMoved == false)
            // {
            //     _isMoved = true;
            //     agent.MoveDirection();
            // }
            
            // v3.5: Allow all agent act
            agent.Punish(_idlePunish);
            if (agent.GetCurrentAction() != 0 )
                agent.MoveDirection();
        }
        
        EndTurn();
    }

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

    public int GetFaction()
    {
        return m_Faction;
    }

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
}