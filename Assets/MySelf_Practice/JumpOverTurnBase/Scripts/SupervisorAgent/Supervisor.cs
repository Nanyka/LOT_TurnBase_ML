using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;

public class Supervisor : MonoBehaviour
{
    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private int m_Faction;
    [SerializeField] private List<NPCUnit> _npvUnits;
    [SerializeField] private float _standardReward = 1f;
    [SerializeField] private float _standarPunishment = -0.1f;
    [SerializeField] private float _idlePunish = -1f;
    [SerializeField] private float _showCurrentReward;

    private Agent _mAgent;
    
    public void Awake()
    {
        // Since this example does not inherit from the Agent class, explicit registration
        // of the RpcCommunicator is required. The RPCCommunicator should only be compiled
        // for Standalone platforms (i.e. Windows, Linux, or Mac)
#if UNITY_EDITOR || UNITY_STANDALONE
        if (!CommunicatorFactory.CommunicatorRegistered)
        {
            CommunicatorFactory.Register<ICommunicator>(RpcCommunicator.Create);
        }
#endif
    }

    private void Start()
    {
        _mAgent = GetComponent<Agent>();

        _environmentController.OnChangeFaction.AddListener(ToMyTurn);
        _environmentController.OnReset.AddListener(ResetAgents);
        _environmentController.OnPunishOppositeTeam.AddListener(GetPunish);

        MultiJumperKickOff();
    }

    #region Ask for decision from agents

    private void ToMyTurn()
    {
        if (_environmentController.GetCurrFaction() != m_Faction)
            return;
        
        if (Academy.Instance.IsCommunicatorOn)
            _mAgent?.RequestDecision();
        else
            StartCoroutine(WaitToRequestDecision());
    }

    private IEnumerator WaitToRequestDecision()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        _mAgent?.RequestDecision();
    }

    // Receive action and assign task to selected unit
    public void MoveUnit(int index, int direction)
    {
        _mAgent.AddReward(_standarPunishment);
        _npvUnits[index].MoveDirection(direction);
    }

    // get response from selected unit after it conducted action
    public void FinishAndResponse()
    {
        UpdateRewardUI();
        EndTurn();
    }

    #endregion

    private void ResetAgents()
    {
        // Reset this agent
        foreach (var unit in _npvUnits)
        {
            unit.ResetUnit();
        }

        _mAgent.EndEpisode();
    }

    // KICK-OFF this MLAgents environment
    private void MultiJumperKickOff()
    {
        if (m_Faction == 0)
            _environmentController.KickOffEnvironment();
    }

    private void EndTurn()
    {
        // call for the end-turn event
        _environmentController.ChangeFaction();
        _environmentController.OnChangeFaction.Invoke();
    }

    #region REWARDs

    public void AddReward()
    {
        _mAgent.AddReward(_standardReward);
    }

    public void AddReward(float reward)
    {
        _mAgent.AddReward(reward);
    }

    public void AddIdlePunishment()
    {
        _mAgent.AddReward(_idlePunish);
    }

    // Get punish whenever an agent jump over enemies
    private void GetPunish(int faction, int overEnemy)
    {
        if (faction == m_Faction)
            return;

        // Punish something when opposite faction get score
        // foreach (var agent in m_JumpOverControllers)
        // {
        //     agent.Punish(_punishAmount * overEnemy);
        // }
    }

    #endregion

    #region GET & SET

    public int GetFaction()
    {
        return m_Faction;
    }

    private void UpdateRewardUI()
    {
        _showCurrentReward = _mAgent.GetCumulativeReward();
    }

    #endregion
}