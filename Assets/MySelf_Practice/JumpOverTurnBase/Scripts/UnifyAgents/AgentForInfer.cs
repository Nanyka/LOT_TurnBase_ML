using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;

public class AgentForInfer : AgentManager
{
    [SerializeField] private SkillManager m_SkillManager;
    [SerializeField] private Material _factionMaterial;

    private Material _defaultMaterial;
    [SerializeField] private List<JumperForGame> m_JumperForGames;
    private List<JumperForGame> _dummyJumper;
    private int _skillCount;

    protected override void Start()
    {
        m_Environment.OnChangeFaction.AddListener(ToMyTurn);
        m_Environment.OnReset.AddListener(ResetAgents);
        m_Environment.OnPunishOppositeTeam.AddListener(GetPunish);
        m_Environment.OnOneTeamWin.AddListener(FinishRound);

        _defaultMaterial = m_JumpOverControllers[0].GetDefaultMaterial();

        InitiateJumperList();
        MultiJumperKickOff();
    }

    private void InitiateJumperList()
    {
        m_JumperForGames = new List<JumperForGame>(m_JumpOverControllers.Count);
        _dummyJumper = new List<JumperForGame>(m_JumpOverControllers.Count);

        foreach (var jumperController in m_JumpOverControllers)
            m_JumperForGames.Add((JumperForGame) jumperController);

        InsertTempIndex();
    }

    private void InsertTempIndex()
    {
        for (int i = 0; i < m_JumperForGames.Count; i++)
        {
            var jumper = m_JumperForGames[i];
            jumper.InferMoving.AgentIndex = i;
        }
    }

    protected override void ResetAgents()
    {
        foreach (var agent in m_JumperForGames)
            agent.ResetAgent();
    }

    #region MY TURN

    // Change unit colour from environmentManager when changing faction

    protected override void ToMyTurn()
    {
        if (m_Environment.GetCurrFaction() != m_Faction)
            return;

        // reset all agent's moving state
        foreach (var jumperController in m_JumperForGames)
            jumperController.ResetMoveState(_factionMaterial);

        KickOffUnitActions(); // kick off the first round of inferring action
    }

    public override void KickOffUnitActions()
    {
        // Just select jumpers who still not move this turn
        _dummyJumper.Clear();
        foreach (var jumperForGame in m_JumperForGames)
            if (jumperForGame.CheckUsedThisTurn() == false)
                _dummyJumper.Add(jumperForGame);

        if (_dummyJumper.Count > 0)
        {
            _skillCount = 0;
            m_SkillManager.ResetSkillCache();
            StartInferAgentsAction(_dummyJumper);
        }
        else
            EndTurn();
    }

    private void StartInferAgentsAction(IEnumerable<JumperForGame> jumperForGames)
    {
        if (_skillCount < m_SkillManager.GetSkillAmount() && m_SkillManager.GetSkillByIndex(_skillCount) != null)
        {
            var forGames = jumperForGames as JumperForGame[] ?? jumperForGames.ToArray();
            // reset counter before collect action
            _responseCounter = 0;
            // Ask for action based on skill count if still not over all skill
            foreach (var jumper in forGames)
            {
                // Infer action & add to jumper cache as currentAction when other idle
                jumper.SetBrain(m_SkillManager.GetSkillByIndex(_skillCount).GetModel());
                jumper.AskForAction();
            }
        }
        else
        {
            m_SkillManager.ActionBrainstorming();
        }
    }

    public override void CollectUnitResponse()
    {
        _responseCounter++;

        // if all agent response, add action to cache at skillManager
        if (_responseCounter != _dummyJumper.Count) return;

        foreach (var jumperForGame in _dummyJumper)
            m_SkillManager.AddActionToCache(jumperForGame.InferMoving);

        // Move to next skill
        _skillCount++;
        StartInferAgentsAction(_dummyJumper);
    }

    #endregion

    #region END TURN

    protected override void EndTurn()
    {
        // Attack nearby enemy
        foreach (var agent in m_JumperForGames)
        {
            if (agent.GetJumpStep() == 0)
                continue;

            agent.Attack();
        }

        foreach (var jumper in m_JumperForGames)
            jumper.SetMaterial(_defaultMaterial);

        // call for the end-turn event
        StartCoroutine(WaitForChangeFaction());
    }

    private IEnumerator WaitForChangeFaction()
    {
        yield return new WaitForSeconds(1f);
        m_Environment.ChangeFaction();
        m_Environment.OnChangeFaction.Invoke();
    }
    
    protected override void FinishRound(int faction)
    {
        Debug.Log("NPC end game");
    }

    #endregion

    #region GET & SET
    // Remove dieJumper from environment
    public override void RemoveAgent(SingleJumperController jumper)
    {
        m_Environment.RemoveObject(jumper.gameObject, m_Faction);
        m_JumperForGames.Remove(jumper as JumperForGame);
        InsertTempIndex();
    }

    public List<JumperForGame> GetJumpers()
    {
        return m_JumperForGames;
    }

    #endregion
}