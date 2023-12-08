using System;
using System.Collections.Generic;
using GOAP;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class EnemyAttack : GAction
    {
        [SerializeField] private CharacterEntity m_Character;
        [SerializeField] private float _checkDistance = 1f;
        [SerializeField] private bool _isModifyBeliefs;

        public override bool PrePerform()
        {
            if (m_GAgent.Inventory.IsEmpty())
                return true;

            var target = m_GAgent.Inventory.items[0];

            if (target.TryGetComponent(out ICheckableObject checkableObject))
            {
                var distanceToTarget = Vector3.Distance(checkableObject.GetPosition(), transform.position);

                if (distanceToTarget < _checkDistance)
                {
                    // checkableObject.ReduceCheckableAmount(1);
                    var position = target.transform.position;
                    m_Character.transform.LookAt(new Vector3(position.x, m_Character.transform.position.y, position.z));
                    m_Character.StartAttack();
                    
                    if (checkableObject.IsCheckable() == false)
                        m_GAgent.Inventory.ClearInventory();

                    if (_isModifyBeliefs)
                    {
                        m_GAgent.Beliefs.RemoveState("Empty");
                        m_GAgent.Beliefs.ModifyState("targetAvailable", 1);
                    }
                    
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
            return true;
        }

        // public void ExecuteAttack(GameObject target)
        // {
        //     // if (target.TryGetComponent(out ICheckableObject checkableObject))
        //     //     checkableObject.ReduceCheckableAmount(1);
        // }
    }
}