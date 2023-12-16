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

        private void Start()
        {
            m_AttackExecutor = m_Character.GetComponent<IAttackExecutor>();
            _checkDistance = m_GAgent.GetComponent<ISensor>().DetectRange() + 2f;
        }

        public override bool PrePerform()
        {
            if (m_GAgent.Inventory.IsEmpty())
                return false;

            var target = m_GAgent.Inventory.items[0];

            if (target.TryGetComponent(out ICheckableObject checkableObject))
            {
                var distanceToTarget = Vector3.Distance(checkableObject.GetPosition(), transform.position);

                if (distanceToTarget < _checkDistance)
                {
                    var position = target.transform.position;
                    m_Character.transform.LookAt(new Vector3(position.x, m_Character.transform.position.y, position.z));
                    m_AttackExecutor.ExecuteHitEffect(position); // Set target

                    if (checkableObject.IsCheckable() == false)
                        m_GAgent.Inventory.ClearInventory();

                    Duration = 1f;
                }
                else
                    Duration = 0f;
            }

            return true;
        }

        public override bool PostPerform()
        {
            // m_GAgent.Inventory.ClearInventory();
            return true;
        }
    }
}