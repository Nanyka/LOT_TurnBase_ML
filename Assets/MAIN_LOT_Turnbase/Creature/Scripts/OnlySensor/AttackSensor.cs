using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AttackSensor: ISensorExecute
    {
        public int DecideDirection(CreatureData m_CreatureData, Transform m_Transform, EnvironmentManager _envManager, CreatureEntity m_Entity, SkillComp skillComp)
        {
            int movingIndex = 0;
            var maxHit = 0;
            for (int i = 1; i < 5; i++)
            {
                var movement = _envManager.GetMovementInspector().MovingPath(m_Transform.position, i, 0, 0);
                int dummyJump = movement.jumpCount + m_Entity.GetJumpBoost();
                
                if (dummyJump > 0)
                {
                    var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(i), dummyJump, skillComp);
                    if (attackPoints == null)
                        continue;

                    int curHit = 0;
                    foreach (var attackPoint in attackPoints)
                        if (_envManager.CheckEnemy(attackPoint, m_Entity.GetFaction()))
                            curHit++;

                    if (curHit > maxHit)
                    {
                        maxHit = curHit;
                        movingIndex = i;
                    }
                }
            }
            
            return movingIndex;
        }
        
        private IEnumerable<Vector3> AttackPoints(Vector3 targetPos, Vector3 direction, int jumpStep, SkillComp m_Skills)
        {
            if (m_Skills.GetSkillByIndex(jumpStep - 1) == null)
                return null;

            jumpStep = jumpStep < m_Skills.GetSkillAmount() ? jumpStep : m_Skills.GetSkillAmount();
            return m_Skills.GetSkillByIndex(jumpStep - 1).CalculateSkillRange(targetPos, direction);
        }
    }
}