using System;
using System.Collections.Generic;
using GOAP;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AttackCheckableObj : GAction
    {
        [SerializeField] private CharacterEntity m_Character;
        [SerializeField] private float _checkDistance = 1f;

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
                    m_Character.StartAttack();
                    
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