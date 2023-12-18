using System;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class FireCheckableObj : GAction
    {
        [SerializeField] private GameObject m_Character;
        
        private float _checkDistance;
        private IAttackExecutor m_AttackExecutor;
        private ICheckableObject _currentPoint;

        private void Start()
        {
            m_AttackExecutor = m_Character.GetComponent<IAttackExecutor>();
            _checkDistance = m_GAgent.GetComponent<ISensor>().DetectRange() + 2f;
        }

        public override bool PrePerform()
        {
            _currentPoint = null;
            var distanceToTarget = float.PositiveInfinity;
            var buildings = SavingSystemManager.Instance.GetEnvLoader().GetBuildings(FactionType.Enemy);
                
            foreach (var building in buildings)
            {
                if (building.TryGetComponent(out ICheckableObject checkableObject))
                {
                    if (checkableObject.IsCheckable() == false)
                        continue;

                    var curDis = Vector3.Distance(transform.position, checkableObject.GetPosition());
                    if (curDis < distanceToTarget)
                    {
                        distanceToTarget = curDis;
                        _currentPoint = checkableObject;
                    }
                }
            }

            if (_currentPoint == null)
            {
                return false;
            }

            var position = _currentPoint.GetPosition();
            m_Character.transform.LookAt(new Vector3(position.x, m_Character.transform.position.y, position.z));
            m_AttackExecutor.ExecuteHitEffect(position); // Set target
            return true;
        }

        public override bool PostPerform()
        {
            // m_GAgent.Inventory.ClearInventory();
            return true;
        }
    }
}