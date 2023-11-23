using System.Collections.Generic;
using GOAP;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CheckInProcessArea : GAction
    {
        [SerializeField] private float _checkDistance = 1f;

        // [SerializeField] private BuildingConstructComp[] _testTarget; // TODO: refactor to get it from buildingManager
        // private List<ICheckableObject> _checkableObjects = new();
        // private ICheckableObject _currentPoint;

        // private void Start()
        // {
        //     _testTarget = FindObjectsByType<BuildingConstructComp>(FindObjectsSortMode.None);
        //
        //     foreach (var target in _testTarget)
        //     {
        //         if (target.TryGetComponent(out ICheckableObject checkableObject))
        //             _checkableObjects.Add(checkableObject);
        //     }
        // }

        public override bool PrePerform()
        {
            // Check if any available area in inventory or not
            // If it is, set it as target
            // If not, select it from envData and add it into inventory

            // var distanceToTarget = float.PositiveInfinity;
            // foreach (var target in _checkableObjects)
            // {
            //     if (target.IsCheckable())
            //         continue;
            //
            //     var curDis = Vector3.Distance(transform.position, target.GetPosition());
            //     if (curDis < distanceToTarget)
            //     {
            //         distanceToTarget = curDis;
            //         _currentPoint = target;
            //     }
            // }

            // var distanceToTarget = float.PositiveInfinity;
            // if (m_GAgent.Inventory.IsEmpty())
            // {
            //     _currentPoint = null;
            //     var buildings = SavingSystemManager.Instance.GetEnvironmentData().BuildingData;
            //     foreach (var building in buildings)
            //     {
            //         var buildingObj = GameFlowManager.Instance.GetEnvManager()
            //             .GetObjectByPosition(building.Position, FactionType.Player);
            //
            //         if (buildingObj.TryGetComponent(out ICheckableObject checkableObject))
            //         {
            //             if (checkableObject.IsCheckable())
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