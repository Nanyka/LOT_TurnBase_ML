using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class NPCActionInferer : MonoBehaviour
    {
        public List<DummyAction> m_ActionCache = new();

        private NpcFactionController _npcsController;
        [SerializeField] private List<Skill_SO> m_SkillSOs = new();

        public void Init()
        {
            _npcsController = GetComponent<NpcFactionController>();
            // GatherSkillFromJumpers();
        }

        public void GatherSkillFromJumpers()
        {
            foreach (var enemy in _npcsController.GetEnemies())
            {
                foreach (var skill in enemy.GetAttackRelated().GetSkills())
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
            // dummyAction.VoteAmount = 0;
            // dummyAction.Reward = 0;

            // Check if any tuple store the same action for this agent, if not, save a new tuple in cache
            var checkDuplicateTuple = false;
            foreach (var action in m_ActionCache)
            {
                if (action.CheckTupleExist(dummyAction.AgentIndex, dummyAction.Action))
                {
                    checkDuplicateTuple = true;
                    action.VoteAmount += dummyAction.VoteAmount;
                    break;
                }
            }

            if (checkDuplicateTuple == false)
            {
                // dummyAction.VoteAmount++;
                m_ActionCache.Add(dummyAction);
            }
        }

        public void ActionBrainstorming()
        {
            //--> START brainstorming from here

            // Calculate reward for each action
            // foreach (var action in m_ActionCache)
            // {
            //     // Debug.Log($"Agent {action.AgentIndex} action as {action.Action} with direction {action.Direction} have reward {action.Reward} and {action.VoteAmount} votes");
            //
            //     // Calculate reward for each agent
            //     if (action.JumpCount > 0)
            //     {
            //         var attackPoints = AttackPoints(action.TargetPos, action.Direction, action.JumpCount);
            //         // var attackRange = attackPoints as Vector3[] ?? attackPoints.ToArray();
            //         // _agentManager.GetJumpers()[action.AgentIndex].ShowAttackRange(attackRange); // --> TESTING
            //
            //         if (attackPoints == null)
            //             continue;
            //
            //         foreach (var attackPoint in attackPoints)
            //             if (_npcsController.GetEnvironment().CheckEnemy(attackPoint, _npcsController.GetFaction()))
            //                 action.Reward++;
            //     }
            // }

            // Sort by reward
            // var orderedAction = m_ActionCache.OrderByDescending(x => x.Reward).ElementAt(0);
            // if (orderedAction.Reward == 0)
            //     orderedAction = m_ActionCache.OrderByDescending(x => x.Action > 0).ThenByDescending(x => x.VoteAmount).ElementAt(0);

            // Sort by reward and by voting after
            var orderedAction = m_ActionCache.OrderByDescending(x => x.Action > 0).ThenByDescending(x => x.VoteAmount).ElementAt(0);
            
            // Get top tuple
            _npcsController.GetEnemies()[orderedAction.AgentIndex].ConductSelectedAction(orderedAction);

            // StartCoroutine(WaitBeforeAction(orderedAction)); // --TESTING--
        }

        // just for testing purpose
        private IEnumerator WaitBeforeAction(DummyAction action)
        {
            yield return new WaitUntil(() => Input.anyKey);
            Debug.Log("Wait for testing");
            _npcsController.GetEnemies()[action.AgentIndex].ConductSelectedAction(action);
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
}