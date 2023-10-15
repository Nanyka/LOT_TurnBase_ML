using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class DefenseSensor : ISensorExecute
    {
        public int DecideDirection(CreatureData m_CreatureData, Transform m_Transform, EnvironmentManager _envManager,
            CreatureEntity m_Entity, SkillComp skillComp)
        {
            // Get enemy list
            // Get each enemy potential attack points
            // Check if any attack hit me
            // Check actions to avoid

            List<Vector3> enemyHits = new();
            var movingIndex = 0;
            var enemies = SavingSystemManager.Instance.GetEnvironmentData().PlayerData;
            foreach (var enemy in enemies)
            {
                for (int i = 1; i < 5; i++)
                {
                    var movement = _envManager.GetMovementInspector()
                        .MovingPath(enemy.Position, i, 0, 0);
                    if (movement.jumpCount > 0)
                    {
                        var enemyInventory = SavingSystemManager.Instance.GetInventoryItemByName(enemy.EntityName);
                        var enemySkill = (Skill_SO)AddressableManager.Instance.GetAddressableSO(
                            enemyInventory.skillsAddress[movement.jumpCount - 1]);

                        var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(i), enemySkill);
                        if (attackPoints == null)
                            continue;

                        foreach (var attackPoint in attackPoints)
                        {
                            if (Vector3.Distance(attackPoint, m_Transform.position) < 0.1f)
                            {
                                enemyHits = (List<Vector3>)attackPoints;
                                goto LoopEnd;
                            }
                        }
                    }
                }
            }

            LoopEnd:
            {
                if (enemyHits.Count > 0)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        var lurePos = m_Transform.position + JIGeneralUtils.DirectionTo(i);
                        if (enemyHits.Contains(lurePos) == false && _envManager.FreeToMove(lurePos))
                        {
                            movingIndex = i;
                            break;
                        }
                    }
                }
            }

            return movingIndex;
        }
        
        private IEnumerable<Vector3> AttackPoints(Vector3 targetPos, Vector3 direction, Skill_SO skill)
        {
            return skill.CalculateSkillRange(targetPos, direction);
        }
    }
}