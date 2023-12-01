using System;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectResource : GAction, ICharacterAttack
    {
        [SerializeField] private CharacterEntity m_Character;
        [SerializeField] private float _checkDistance = 1f;
        [SerializeField] private bool _isModifyBeliefs;

        private HarvestComp m_HarvestComp;

        private void Start()
        {
            m_HarvestComp = m_Character.GetComponent<HarvestComp>();
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
                    m_Character.StartAttack(this);
                    
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
            m_GAgent.Inventory.ClearInventory();

            if (_isModifyBeliefs)
            {
                m_GAgent.Beliefs.RemoveState("Empty");
                m_GAgent.Beliefs.ModifyState("targetAvailable", 1);
            }
            
            return true;
        }

        public void ExecuteAttack(GameObject target)
        {
            if (target.TryGetComponent(out ICheckableObject checkableObject))
            {
                checkableObject.ReduceCheckableAmount(1);
            }
        }
    }
}