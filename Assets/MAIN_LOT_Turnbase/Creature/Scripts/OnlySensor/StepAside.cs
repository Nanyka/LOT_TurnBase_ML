using UnityEngine;

namespace JumpeeIsland
{
    public class StepAside : ISensorExecute
    {
        public int DecideDirection(CreatureData m_CreatureData, Transform m_Transform, EnvironmentManager _envManager,
            CreatureEntity m_Entity, SkillComp skillComp)
        {
            int movingIndex = 0;
            int detectedDir = 0;
            var envData = SavingSystemManager.Instance.GetEnvironmentData();
            var movementInspector = _envManager.GetMovementInspector();

            for (int i = 1; i < 5; i++)
            {
                var detectPos = movementInspector.DirectionTo(i) + m_Transform.position;
                if (_envManager.FreeToMove(detectPos) == false)
                {
                    detectPos += movementInspector.DirectionTo(i);
                    if (envData.CheckEnemy(detectPos, m_CreatureData.FactionType) == false)
                    {
                        detectedDir = i;
                        break;
                    }
                }
            }

            if (detectedDir > 0)
            {
                for (int j = 1; j < 5; j++)
                {
                    var detectPos = movementInspector.DirectionTo(j) + m_Transform.position;
                    if (_envManager.FreeToMove(detectPos))
                    {
                        movingIndex = j;
                        break;
                    }
                }
            }

            return movingIndex;
        }
    }
}