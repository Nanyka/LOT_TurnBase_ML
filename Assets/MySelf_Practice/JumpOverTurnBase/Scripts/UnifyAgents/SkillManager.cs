using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("General control part")] [SerializeField]
    private EnvironmentController _environmentController;

    [SerializeField] private AgentManager _agentManager;

    [Header("Skill part")] [SerializeField]
    private UnitSkill m_UnitSkill;

    [SerializeField] private List<JumperForGame> _jumperControllers;

    private List<DummyAction> m_ActionCache = new();

    private void Start()
    {
        for (int i = 0; i < _jumperControllers.Count; i++)
        {
            var jumper = _jumperControllers[i];
            jumper.InferMoving.AgentIndex = i;
        }

        InferActionForAgents();
    }

    public void InferActionForAgents()
    {
        // Gather skills from all agent. Leave it there for later

        // Get one skill manager

        var skill = m_UnitSkill.GetSkills().ElementAt(0);
        if (skill == null)
            return;

        foreach (var jumperController in _jumperControllers)
        {
            jumperController.SetBrain(skill.GetModel());
        }

        foreach (var jumper in _jumperControllers)
        {
            // Infer action & add to jumper cache as currentAction when other idle
            jumper.AskForAction();

            // Calculate reward for each agent
            if (jumper.GetJumpStep() > 0)
            {
                var attackPoints = m_UnitSkill.AttackPoints(jumper.GetPosition(), jumper.GetDirection(),
                    jumper.GetJumpStep());
                if (attackPoints != null)
                {
                    jumper.InferMoving.Reward = 0;
                    foreach (var attackPoint in attackPoints)
                    {
                        // Debug.Log($"Attack at {attackPoint}");
                        if (_environmentController.CheckEnemy(attackPoint, _agentManager.GetFaction()))
                            jumper.InferMoving.Reward++;
                    }
                }
            }

            // Check if any tuple store the same action for this agent, if not, save a new tuple in cache
            var checkDuplicateTuple = false;
            foreach (var action in m_ActionCache.Where(action =>
                action.CheckTupleExist(jumper.InferMoving.AgentIndex, jumper.InferMoving.Action)))
            {
                checkDuplicateTuple = true;
                action.VoteAmount++;
            }

            if (checkDuplicateTuple == false)
                m_ActionCache.Add(jumper.InferMoving);
        }
        
        // m_ActionCache.Clear();
        // foreach (var skill in m_UnitSkill.GetSkills())
        // {
        //     if (skill == null)
        //         continue;
        //
        //     foreach (var jumperController in _jumperControllers)
        //     {
        //         jumperController.SetBrain(skill.GetModel());
        //     }
        //
        //     foreach (var jumper in _jumperControllers)
        //     {
        //         // Infer action & add to jumper cache as currentAction when other idle
        //         jumper.AskForAction();
        //
        //         // Calculate reward for each agent
        //         if (jumper.GetJumpStep() > 0)
        //         {
        //             var attackPoints = m_UnitSkill.AttackPoints(jumper.GetPosition(), jumper.GetDirection(),
        //                 jumper.GetJumpStep());
        //             if (attackPoints != null)
        //             {
        //                 jumper.InferMoving.Reward = 0;
        //                 foreach (var attackPoint in attackPoints)
        //                 {
        //                     // Debug.Log($"Attack at {attackPoint}");
        //                     if (_environmentController.CheckEnemy(attackPoint, _agentManager.GetFaction()))
        //                         jumper.InferMoving.Reward++;
        //                 }
        //             }
        //         }
        //
        //         // Check if any tuple store the same action for this agent, if not, save a new tuple in cache
        //         var checkDuplicateTuple = false;
        //         foreach (var action in m_ActionCache.Where(action =>
        //             action.CheckTupleExist(jumper.InferMoving.AgentIndex, jumper.InferMoving.Action)))
        //         {
        //             checkDuplicateTuple = true;
        //             action.VoteAmount++;
        //         }
        //
        //         if (checkDuplicateTuple == false)
        //             m_ActionCache.Add(jumper.InferMoving);
        //     }
        // }

        // Sort by reward
        var orderedAction = m_ActionCache.OrderBy(x => x.Reward);

        foreach (var action in orderedAction)
        {
            Debug.Log(
                $"Agent {action.AgentIndex} action as {action.Direction} with direction {action.Direction} have reward {action.Reward}");
        }

        // Get top tuple

        // Decide action for selected agent and remove tuple of relevant agents from list
    }

    private IEnumerator WaitForModel()
    {
        yield return new WaitForSeconds(2f);
        
    }
}