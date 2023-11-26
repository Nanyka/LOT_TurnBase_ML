using System.Collections.Generic;
using GOAP;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CheckInProcessArea : GAction
    {
        [SerializeField] private float _checkDistance = 1f;

        public override bool PrePerform()
        {
            if (m_GAgent.Inventory.IsEmpty())
                return true;

            if (m_GAgent.Inventory.items[0].TryGetComponent(out ICheckableObject checkableObject))
            {
                var distanceToTarget = Vector3.Distance(checkableObject.GetPosition(), transform.position);

                if (distanceToTarget < _checkDistance)
                {
                    checkableObject.ReduceCheckableAmount(1);
                    if (checkableObject.IsCheckable() == false)
                        m_GAgent.Inventory.ClearInventory();

                    m_GAgent.Beliefs.RemoveState("Empty");
                    m_GAgent.Beliefs.ModifyState("targetAvailable", 1);
                    Duration = 1f;
                }
                else
                    Duration = 0f;
            }

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}