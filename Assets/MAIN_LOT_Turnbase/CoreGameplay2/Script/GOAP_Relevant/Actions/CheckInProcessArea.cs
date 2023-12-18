using System.Collections.Generic;
using GOAP;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CheckInProcessArea : GAction
    {
        [SerializeField] private float _checkDistance = 1f;
        
        private ICheckableObject _currentPoint;

        public override bool PrePerform()
        {
            // if (m_GAgent.Inventory.IsEmpty())
            //     return true;
            //
            // if (m_GAgent.Inventory.items[0].TryGetComponent(out ICheckableObject checkableObject))
            // {
            //     var distanceToTarget = Vector3.Distance(checkableObject.GetPosition(), transform.position);
            //
            //     if (distanceToTarget < _checkDistance)
            //     {
            //         checkableObject.ReduceCheckableAmount(1);
            //         if (checkableObject.IsCheckable() == false)
            //             m_GAgent.Inventory.ClearInventory();
            //
            //         m_GAgent.Beliefs.RemoveState("Empty");
            //         m_GAgent.Beliefs.ModifyState("targetAvailable", 1);
            //         Duration = 1f;
            //     }
            //     else
            //         Duration = 0f;
            // }
            
            var distanceToTarget = float.PositiveInfinity;
            _currentPoint = null;
            var buildings = SavingSystemManager.Instance.GetEnvLoader().GetBuildings(FactionType.Player);
            foreach (var building in buildings)
            {
                if (building.TryGetComponent(out ICheckableObject checkableObject))
                {
                    if (checkableObject.IsCheckable())
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
                return false;
            
            distanceToTarget = Vector3.Distance(_currentPoint.GetPosition(), transform.position);
            
            if (distanceToTarget < _checkDistance)
            {
                _currentPoint.ReduceCheckableAmount(1);
            
                m_GAgent.Beliefs.RemoveState("Empty");
                m_GAgent.Beliefs.ModifyState("targetAvailable", 1);
                Duration = 1f;
            }
            else
            {
                Duration = 0f;
                return false;
            }

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}