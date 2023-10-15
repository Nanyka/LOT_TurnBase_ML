using UnityEngine;

namespace JumpeeIsland
{
    public class GoAwaySensor : ISensorExecute
    {
        private int _checkingRadius;

        public GoAwaySensor(int range)
        {
            _checkingRadius = range;
        }
        
        public int DecideDirection(CreatureData mCreatureData, Transform mTransform, EnvironmentManager envManager,
            CreatureEntity mEntity, SkillComp skillComp)
        {
            Vector3 _enemyPos = Vector3.negativeInfinity;
            
            // get enemy list
            var enemies = SavingSystemManager.Instance.GetEnvironmentData().PlayerData;
            
            var checkingDistance = float.PositiveInfinity;

            // check enemy in radius range and get the enemy closet to this creature
            foreach (var enemy in enemies)
            {
                if (Mathf.Abs(enemy.Position.x - mCreatureData.Position.x) <= _checkingRadius &&
                    Mathf.Abs(enemy.Position.z - mCreatureData.Position.z) <= _checkingRadius)
                {
                    var currentDistance = Vector3.Distance(enemy.Position, mCreatureData.Position);
                    if (currentDistance < checkingDistance)
                    {
                        checkingDistance = currentDistance;
                        _enemyPos = enemy.Position;
                    }
                }
            }

            if (float.IsNegativeInfinity(_enemyPos.x))
                return 0;

                // check the adjacent directions and add state to belief if available
            var movingIndex = 0;
            checkingDistance = 0f;
            var checkingPos = mCreatureData.Position + Vector3.left;
            if (GameFlowManager.Instance.GetEnvManager().FreeToMove(checkingPos))
            {
                if (Vector3.Distance(checkingPos, _enemyPos) > checkingDistance)
                {
                    checkingDistance = Vector3.Distance(checkingPos, _enemyPos);
                    movingIndex = 1;
                }
            }
            
            checkingPos = mCreatureData.Position + Vector3.right;
            if (GameFlowManager.Instance.GetEnvManager().FreeToMove(checkingPos))
            {
                if (Vector3.Distance(checkingPos, _enemyPos) > checkingDistance)
                {
                    checkingDistance = Vector3.Distance(checkingPos, _enemyPos);
                    movingIndex = 2;
                }
            }
            
            checkingPos = mCreatureData.Position + Vector3.back;
            if (GameFlowManager.Instance.GetEnvManager().FreeToMove(checkingPos))
            {
                if (Vector3.Distance(checkingPos, _enemyPos) > checkingDistance)
                {
                    checkingDistance = Vector3.Distance(checkingPos, _enemyPos);
                    movingIndex = 3;
                }
            }
            
            checkingPos = mCreatureData.Position + Vector3.forward;
            if (GameFlowManager.Instance.GetEnvManager().FreeToMove(checkingPos))
            {
                if (Vector3.Distance(checkingPos, _enemyPos) > checkingDistance)
                {
                    movingIndex = 4;
                }
            }
            
            return movingIndex;
        }
    }
}