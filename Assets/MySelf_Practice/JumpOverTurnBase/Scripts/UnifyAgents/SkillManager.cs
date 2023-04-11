using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("General control part")] 
    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private AgentForInfer _agentManager;

    [Header("Skill part")] 
    [SerializeField] private List<Skill_SO> m_SkillSOs;
    // [SerializeField] private List<JumperForGame> _jumperControllers;

    public List<DummyAction> m_ActionCache = new();

    private void Start()
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
        //Start brainstorming from this place
        
        // Sort by reward
        foreach (var action in m_ActionCache)
        {
            // Debug.Log($"Agent {action.AgentIndex} action as {action.Action} with direction {action.Direction} have reward {action.Reward} and {action.VoteAmount} votes");
            
            // Calculate reward for each agent
            if (action.JumpCount > 0)
            {
                var attackPoints = AttackPoints(action.CurrentPos, action.Direction, action.JumpCount);
                if (attackPoints != null)
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

        // Decide action for selected agent and reset current inference
    }

    public void ResetSkillCache()
    {
        m_ActionCache.Clear();    
    }
    
    public IEnumerable<Vector3> AttackPoints(Vector3 targetPos, Vector3 direction, int jumpStep)
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
}