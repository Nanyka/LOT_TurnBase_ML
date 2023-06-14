using System;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class Creature1Sensor : GOAPCreatureSensor
    {
        [SerializeField] private int _checkingRadius;

        private CreatureData m_CreatureData;
        private Vector3 _enemyPos;

        private void Start()
        {
            m_CreatureData = (CreatureData) GetComponent<GOAPCreatureBrain>().GetCreature().GetEntity().GetData();
        }

        public override void DetectEnvironment(WorldStates beliefs)
        {
            beliefs.ClearStates();

            DetectEnemy(beliefs);
        }

        private void DetectEnemy(WorldStates beliefs)
        {
            // get enemy list
            var enemies = SavingSystemManager.Instance.GetEnvironmentData().PlayerData;
            
            var checkingDistance = float.PositiveInfinity;

            // check enemy in radius range and get the enemy closet to this creature
            foreach (var enemy in enemies)
            {
                if (Mathf.Abs(enemy.Position.x - m_CreatureData.Position.x) <= _checkingRadius &&
                    Mathf.Abs(enemy.Position.z - m_CreatureData.Position.z) <= _checkingRadius)
                {
                    var currentDistance = Vector3.Distance(enemy.Position, m_CreatureData.Position);
                    if (currentDistance < checkingDistance)
                    {
                        checkingDistance = currentDistance;
                        _enemyPos = enemy.Position;
                    }
                }
            }
            
            // check the adjacent directions and add state to belief if available
            var movingIndex = 0;
            checkingDistance = 0f;
            var checkingPos = m_CreatureData.Position + Vector3.left;
            if (GameFlowManager.Instance.GetEnvManager().FreeToMove(checkingPos))
            {
                if (Vector3.Distance(checkingPos, _enemyPos) > checkingDistance)
                {
                    checkingDistance = Vector3.Distance(checkingPos, _enemyPos);
                    movingIndex = 1;
                }
            }
            
            checkingPos = m_CreatureData.Position + Vector3.right;
            if (GameFlowManager.Instance.GetEnvManager().FreeToMove(checkingPos))
            {
                if (Vector3.Distance(checkingPos, _enemyPos) > checkingDistance)
                {
                    checkingDistance = Vector3.Distance(checkingPos, _enemyPos);
                    movingIndex = 2;
                }
            }
            
            checkingPos = m_CreatureData.Position + Vector3.back;
            if (GameFlowManager.Instance.GetEnvManager().FreeToMove(checkingPos))
            {
                if (Vector3.Distance(checkingPos, _enemyPos) > checkingDistance)
                {
                    checkingDistance = Vector3.Distance(checkingPos, _enemyPos);
                    movingIndex = 3;
                }
            }
            
            checkingPos = m_CreatureData.Position + Vector3.forward;
            if (GameFlowManager.Instance.GetEnvManager().FreeToMove(checkingPos))
            {
                if (Vector3.Distance(checkingPos, _enemyPos) > checkingDistance)
                {
                    movingIndex = 4;
                }
            }
            
            // Select state and save it to belief
            switch (movingIndex)
            {
                case 1:
                    beliefs.ModifyState("GOLEFT",1);
                    break;
                case 2:
                    beliefs.ModifyState("GORIGHT",1);
                    break;
                case 3:
                    beliefs.ModifyState("GOBACKWARD",1);
                    break;
                case 4:
                    beliefs.ModifyState("GOFORWARD",1);
                    break;
            }
        }
    }
}