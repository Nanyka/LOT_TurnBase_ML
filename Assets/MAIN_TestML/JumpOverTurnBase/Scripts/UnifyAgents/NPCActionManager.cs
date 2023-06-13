using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCActionManager : MonoBehaviour
{
    [Header("General control part")] 
    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private AgentForInfer _agentManager;

    [Header("Skill part")] 
    [SerializeField] private List<Skill_SO> m_SkillSOs;
    // [SerializeField] private List<JumperForGame> _jumperControllers;

    public List<DummyAction> m_ActionCache = new();

    public void Init()
    {
        GatherSkillFromJumpers();
    }

    private void GatherSkillFromJumpers()
    {
        foreach (var jumper in _agentManager.GetJumpers())
        {
            foreach (var skill in jumper.GetEntity().GetSkills())
            {
                if (m_SkillSOs.Contains(skill))
                    continue;
                m_SkillSOs.Add(skill);
            }
        }
    }

    public void AddActionToCache(DummyAction inputAction)
    {
        DummyAction dummyAction = new DummyAction(inputAction);
        dummyAction.VoteAmount = 0;
        dummyAction.Reward = 0;
        
        // Check if any tuple store the same action for this agent, if not, save a new tuple in cache
        var checkDuplicateTuple = false;
        foreach (var action in m_ActionCache)
        {
            if (action.CheckTupleExist(dummyAction.AgentIndex, dummyAction.Action))
            {
                checkDuplicateTuple = true;
                action.VoteAmount++;
                break;
            }
        }

        if (checkDuplicateTuple == false)
        {
            dummyAction.VoteAmount++;
            m_ActionCache.Add(dummyAction);
        }
    }

    public void ActionBrainstorming()
    {
        //--> START brainstorming from here
        
        // Sort by reward
        foreach (var action in m_ActionCache)
        {
            // Debug.Log($"Agent {action.AgentIndex} action as {action.Action} with direction {action.Direction} have reward {action.Reward} and {action.VoteAmount} votes");
            
            // Calculate reward for each agent
            if (action.JumpCount > 0)
            {
                var attackPoints = AttackPoints(action.TargetPos, action.Direction, action.JumpCount);
                // var attackRange = attackPoints as Vector3[] ?? attackPoints.ToArray();
                // _agentManager.GetJumpers()[action.AgentIndex].ShowAttackRange(attackRange); // --> TESTING
                foreach (var attackPoint in attackPoints)
                    if (_environmentController.CheckEnemy(attackPoint, _agentManager.GetFaction()))
                        action.Reward++;
            }
        }
        
        // Sort by reward
        var orderedAction = m_ActionCache.OrderByDescending(x => x.Reward).ElementAt(0);
        if (orderedAction.Reward == 0)
            orderedAction = m_ActionCache.OrderByDescending(x => x.VoteAmount).ElementAt(0);
        
        // Get top tuple
        _agentManager.GetJumpers()[orderedAction.AgentIndex].ConductSelectedAction(orderedAction);
        // StartCoroutine(WaitBeforeAction(orderedAction)); // --TESTING--
    }

    // just for TESTING purpose
    private IEnumerator WaitBeforeAction(DummyAction action)
    {
        yield return new WaitUntil(() => Input.anyKey);
        _agentManager.GetJumpers()[action.AgentIndex].ConductSelectedAction(action);
    }

    #region GET

    public void ResetSkillCache()
    {
        m_ActionCache.Clear();    
    }

    private IEnumerable<Vector3> AttackPoints(Vector3 targetPos, Vector3 direction, int jumpStep)
    {
        if (m_SkillSOs.Count < jumpStep || m_SkillSOs[jumpStep - 1] == null)
            return null;
        return m_SkillSOs[jumpStep - 1].CalculateSkillRange(targetPos, direction);
    }

    public int GetSkillAmount()
    {
        return m_SkillSOs.Count;
    }

    public Skill_SO GetSkillByIndex(int skillCount)
    {
        return m_SkillSOs[skillCount];
    }

    #endregion
}