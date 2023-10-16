using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class FindOpportunitySensor : ISensorExecute
    {
        // Get object in range
        // Check any available position to execute successful attacks from the objects
        // Check jumping point for that attack is available or not
        // Check the movement that bring character as close to the intent position as possible

        private int _detectRange;
        private int _maxHits;
        private List<Vector3> potentialPos = new();

        public FindOpportunitySensor(int detectRange)
        {
            _detectRange = detectRange;
        }
        
        public (int,int) DecideDirection(CreatureData mCreatureData, Transform mTransform, EnvironmentManager envManager,
            CreatureEntity mEntity, SkillComp skillComp)
        {
            int movingIndex = 0;
            potentialPos.Clear();

            for (int i = -_detectRange; i <= _detectRange; i++)
            {
                for (int j = -_detectRange; j <= _detectRange; j++)
                {
                    var detectPos = new Vector3(i, 0f, j);
                    if (detectPos == Vector3.zero)
                        continue;

                    detectPos += mTransform.position;

                    if (envManager.FreeToMove(detectPos) || envManager.CheckOutOfBoundary(detectPos))
                        continue;

                    for (int k = 1; k < 5; k++)
                    {
                        var jumpPos = detectPos + JIGeneralUtils.AdverseDirectionTo(k);
                        if (envManager.FreeToMove(jumpPos))
                        {
                            var movement = envManager.GetMovementInspector()
                                .MovingPath(jumpPos, k, 0, 0);
                            if (movement.jumpCount > 0)
                            {
                                movement.jumpCount += mEntity.GetJumpBoost();

                                if (skillComp.GetSkillByIndex(movement.jumpCount - 1).CheckGlobalTarget())
                                {
                                    potentialPos.Add(jumpPos);
                                    continue;
                                }
                                
                                var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(k),
                                    movement.jumpCount, skillComp);
                                if (attackPoints == null)
                                    continue;

                                var hitAmount = 0;
                                foreach (var attackPoint in attackPoints)
                                    if (envManager.CheckEnemy(attackPoint, mEntity.GetFaction()))
                                        hitAmount++;

                                if (hitAmount > _maxHits)
                                {
                                    _maxHits = hitAmount;
                                    potentialPos.Add(jumpPos);
                                }
                            }
                        }
                    }
                }
            }

            float minDistance = Mathf.Infinity;
            Vector3 target = Vector3.negativeInfinity;
            foreach (var pos in potentialPos)
            {
                var curDistance = Vector3.Distance(pos, mTransform.position);
                if (curDistance < minDistance)
                {
                    minDistance = curDistance;
                    target = pos;
                }
            }
                
            if (target.x.CompareTo(float.NegativeInfinity) < 0.1f)
            {
                return (movingIndex,_maxHits);
            }
                
            var movingPath = GameFlowManager.Instance.GetEnvManager()
                .GetAStarPath(mTransform.position, target);

            return (movingPath == null? 0 : envManager.GetActionByDirection(movingPath[0].position - mTransform.position),_maxHits);
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