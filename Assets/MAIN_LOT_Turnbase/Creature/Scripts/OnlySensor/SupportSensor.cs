using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class SupportSensor : ISensorExecute
    {
        public int DecideDirection(CreatureData m_CreatureData, Transform m_Transform, EnvironmentManager _envManager,
            CreatureEntity m_Entity, SkillComp skillComp)
        {
            // Get cohorts
            // Check each direction
            // How many jump cohort can execute in each direction if it is supported
            // Check curHit of the max jumping path
            // If curHit higher than maxHit, record support position

            int movingIndex = 0;
            var cohorts = SavingSystemManager.Instance.GetEnvironmentData().EnemyData;
            Vector3 supportPos = Vector3.negativeInfinity;
            var maxHit = 0;

            foreach (var cohort in cohorts)
            {
                if (cohort == m_CreatureData)
                    continue;

                for (int i = 1; i < 5; i++)
                {
                    var jumpingPathScore = 0; // How the jumping path complete their shape
                    var checkFreePos = cohort.Position + JIGeneralUtils.DirectionTo(i);
                    if (_envManager.FreeToMove(checkFreePos) == false &&
                        _envManager.CheckOutOfBoundary(checkFreePos) == false)
                    {
                        jumpingPathScore += 1;
                    }

                    checkFreePos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 3;
                    if (_envManager.FreeToMove(checkFreePos) == false &&
                        _envManager.CheckOutOfBoundary(checkFreePos) == false)
                    {
                        jumpingPathScore += 3;
                    }

                    checkFreePos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 5;
                    if (_envManager.FreeToMove(checkFreePos) == false &&
                        _envManager.CheckOutOfBoundary(checkFreePos) == false)
                    {
                        jumpingPathScore += 5;
                    }

                    Vector3 tempPos = Vector3.negativeInfinity;
                    Vector3 checkPos = Vector3.negativeInfinity;
                    var jumpCount = 0;
                    switch (jumpingPathScore)
                    {
                        case 0:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i);
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 2;
                            jumpCount = 1;
                            break;
                        }
                        case 1:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 3;
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 4;
                            jumpCount = 2;
                            break;
                        }
                        case 3:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i);
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 4;
                            jumpCount = 2;
                            break;
                        }
                        case 4:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 5;
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 6;
                            jumpCount = 3;
                            break;
                        }
                        case 6:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 3;
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 6;
                            jumpCount = 3;
                            break;
                        }
                        case 8:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i);
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 6;
                            jumpCount = 3;
                            break;
                        }
                    }

                    if (_envManager.CheckOutOfBoundary(checkPos) == false)
                    {
                        var attackPoints = AttackPoints(checkPos, JIGeneralUtils.DirectionTo(i), jumpCount, skillComp);
                        var curHit = 0;
                        foreach (var attackPoint in attackPoints)
                            if (_envManager.CheckEnemy(attackPoint, m_Entity.GetFaction()))
                                curHit++;

                        if (curHit > maxHit)
                        {
                            maxHit = curHit;
                            supportPos = tempPos;
                        }
                    }
                }
            }

            if (supportPos.x.CompareTo(float.NegativeInfinity) == 1)
            {
                float minDistance = Mathf.Infinity;
                for (int i = 1; i < 5; i++)
                {
                    var curDistance = Vector3.Distance((m_Transform.position + JIGeneralUtils.DirectionTo(i)),
                        supportPos);
                    if (curDistance < minDistance && _envManager.FreeToMove(supportPos))
                    {
                        minDistance = curDistance;
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