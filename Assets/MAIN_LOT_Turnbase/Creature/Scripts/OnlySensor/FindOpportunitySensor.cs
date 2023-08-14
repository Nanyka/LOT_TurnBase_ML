using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class FindOpportunitySensor : ISensorExecute
    {
        private int _detectRange;

        public FindOpportunitySensor(int detectRange)
        {
            _detectRange = detectRange;
        }
        
        public int DecideDirection(CreatureData m_CreatureData, Transform m_Transform, EnvironmentManager _envManager,
            CreatureEntity m_Entity, SkillComp skillComp)
        {
            // Get object in range
            // Check any available position to execute successful attacks from the objects
            // Check jumping point for that attack is available or not
            // Check the movement that bring character as close to the intent position as possible
            int movingIndex = 0;
            Vector3 potentialPos = Vector3.negativeInfinity;

            for (int i = -_detectRange; i <= _detectRange; i++)
            {
                for (int j = -_detectRange; j <= _detectRange; j++)
                {
                    var detectPos = new Vector3(i, 0f, j);
                    if (detectPos == Vector3.zero)
                        continue;

                    detectPos += m_Transform.position;

                    if (_envManager.FreeToMove(detectPos) || _envManager.CheckOutOfBoundary(detectPos))
                        continue;

                    for (int k = 1; k < 5; k++)
                    {
                        var jumpPos = detectPos + JIGeneralUtils.AdverseDirectionTo(k);
                        if (_envManager.FreeToMove(jumpPos))
                        {
                            var movement = _envManager.GetMovementInspector()
                                .MovingPath(jumpPos, k, 0, 0);
                            if (movement.jumpCount > 0)
                            {
                                movement.jumpCount += m_Entity.GetJumpBoost();
                                var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(k),
                                    movement.jumpCount, skillComp);
                                if (attackPoints == null)
                                    continue;
                            
                                foreach (var attackPoint in attackPoints)
                                {
                                    if (_envManager.CheckEnemy(attackPoint, m_Entity.GetFaction()))
                                    {
                                        potentialPos = jumpPos;
                                        goto LoopEnd;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            LoopEnd:
            {
                if (potentialPos.x.CompareTo(float.NegativeInfinity) == 1)
                {
                    float minDistance = Mathf.Infinity;
                    for (int i = 1; i < 5; i++)
                    {
                        var curDistance = Vector3.Distance((m_Transform.position + JIGeneralUtils.DirectionTo(i)),
                            potentialPos);
                        if (curDistance < minDistance && _envManager.FreeToMove(potentialPos))
                        {
                            minDistance = curDistance;
                            movingIndex = i;
                        }
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