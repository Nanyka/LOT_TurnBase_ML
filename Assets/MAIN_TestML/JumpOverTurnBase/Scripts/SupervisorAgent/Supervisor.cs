using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Supervisor : MonoBehaviour
{
    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private int m_Faction;
    [SerializeField] private List<NPCUnit> _npcUnits;
    [SerializeField] private float _standardReward = 1f;
    [SerializeField] private float _standarPunishment = -0.1f;
    [SerializeField] private float _idlePunish = -1f;
    [SerializeField] private float _showCurrentReward;
    
    [Header("Attack part")] [SerializeField]
    private UnitSkill m_UnitSkill;

    private Agent _mAgent;
    private List<(int unitIndex, int order, int action)> _movingOrder = new (3);
    
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
        
        // Add 3 item corresponding to 3 unit into list tuple
        _movingOrder.Add(new (0,0,0));
        _movingOrder.Add(new (0,0,0));
        _movingOrder.Add(new (0,0,0));

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

    // Receive action and assign task to selected unit.
    public void MoveUnit(ActionSegment<int> returnAction)
    {
        _mAgent.AddReward(_standarPunishment); // ADD a small PUNISH to rush agent learn faster

        // Insert result into tuple array
        _movingOrder[0] = new(0, returnAction[0], returnAction[3]);
        _movingOrder[1] = new(1, returnAction[1], returnAction[4]);
        _movingOrder[2] = new(2, returnAction[2], returnAction[5]);
        
        // sort actions and have unit move as an order
        _movingOrder.Sort((x,y) => x.order.CompareTo(y.order));
        
        foreach (var moving in _movingOrder)
            _npcUnits[moving.unitIndex].MoveDirection(moving.action);
        
        FinishAndResponse();
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
        foreach (var unit in _npcUnits)
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
        // Attack nearby enemy and ADD REWARD based on number of successful attacks
        foreach (var unit in _npcUnits)
        {
            if (unit.GetJumpStep() == 0)
                continue;

            var attackPoints = m_UnitSkill.AttackPoints(unit.GetPosition(), unit.GetDirection(), unit.GetJumpStep());
            int successAttacks = 0;
            foreach (var attackPoint in attackPoints)
            {
                if (_environmentController.CheckEnemy(attackPoint, m_Faction))
                {
                    // Debug.Log($"Agent at {agent.GetPosition()} conduct a successful attack at {attackPoint}");
                    successAttacks++;
                }
            }

            if (successAttacks > 0)
            {
                unit.ChangeColor(successAttacks);
                _mAgent.AddReward(_standardReward * successAttacks);
                
                _environmentController.OnPunishOppositeTeam.Invoke(GetFaction()); // punish the opposite team
            }
        }
        
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
    private void GetPunish(int faction)
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