using System.Collections.Generic;
using System.Linq;
using GOAP;
using UnityEngine;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class MoveToBuilding : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        [SerializeField] private bool _isConstructor;
        [SerializeField] private string _addState;
        [SerializeField] private string _removeState;

        private ICheckableObject _currentPoint;

        public override bool PrePerform()
        {
            // var distanceToTarget = float.PositiveInfinity;
            // if (m_GAgent.Inventory.IsEmpty())
            // {
            //     _currentPoint = null;
            //     var buildings = SavingSystemManager.Instance.GetEnvLoader().GetBuildings(FactionType.Player);
            //     foreach (var building in buildings)
            //     {
            //         if (building.TryGetComponent(out ICheckableObject checkableObject))
            //         {
            //             if (checkableObject.IsCheckable() == _isConstructor)
            //                 continue;
            //
            //             var curDis = Vector3.Distance(transform.position, checkableObject.GetPosition());
            //             if (curDis < distanceToTarget)
            //             {
            //                 distanceToTarget = curDis;
            //                 _currentPoint = checkableObject;
            //             }
            //         }
            //     }
            //
            //     if (_currentPoint != null)
            //         m_GAgent.Inventory.AddItem(_currentPoint.GetGameObject());
            // }
            //
            // Target = m_GAgent.Inventory.items[0];
            // m_GAgent.SetIProcessUpdate(this);
            
            var distanceToTarget = float.PositiveInfinity;
            _currentPoint = null;
            var buildings = SavingSystemManager.Instance.GetEnvLoader().GetBuildings(FactionType.Player);
            foreach (var building in buildings)
            {
                if (building.TryGetComponent(out ICheckableObject checkableObject))
                {
                    if (checkableObject.IsCheckable() == _isConstructor)
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

            m_GAgent.SetIProcessUpdate(this, _currentPoint.GetPosition());

            return true;
        }

        public override bool PostPerform()
        {
            if (_addState.IsNullOrEmpty() == false)
                m_GAgent.Beliefs.ModifyState(_addState, 1);

            if (_removeState.IsNullOrEmpty() == false)
                m_GAgent.Beliefs.RemoveState(_removeState);

            // If this is a constructor, its moveToBuilding is actually moveToInProcessArea
            // if (_isConstructor == false)
            //     m_GAgent.Inventory.ClearInventory();
            
            return true;
        }

        public void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_GAgent.FinishFromOutside();
        }
    }
}