using UnityEngine;

namespace JumpeeIsland
{
    public class PikupSensor : ISensorExecute
    {
        public (int,int) DecideDirection(CreatureData m_CreatureData, Transform m_Transform, EnvironmentManager _envManager,
            CreatureEntity m_Entity, SkillComp skillComp)
        {
            var pickUpList = SavingSystemManager.Instance.GetEnvironmentData().CollectableData;
            var pickUpPos = Vector3.negativeInfinity;
            int movingIndex = 0;

            foreach (var pickUp in pickUpList)
            {
                // Test the first item
                pickUpPos = pickUp.Position;
                goto LoopEnd;
            }

            LoopEnd:
            {
                if (pickUpPos.x.CompareTo(float.NegativeInfinity) == 1)
                {
                    float minDistance = Mathf.Infinity;
                    for (int i = 1; i < 5; i++)
                    {
                        var movePos = m_Transform.position + JIGeneralUtils.DirectionTo(i);
                        if (_envManager.FreeToMove(movePos) == false)
                            continue;

                        var curDistance = Vector3.Distance((movePos),
                            pickUpPos);
                        if (curDistance < minDistance && _envManager.FreeToMove(pickUpPos))
                        {
                            minDistance = curDistance;
                            movingIndex = i;
                        }
                    }
                }
            }
            return (movingIndex,0);
        }
    }
}