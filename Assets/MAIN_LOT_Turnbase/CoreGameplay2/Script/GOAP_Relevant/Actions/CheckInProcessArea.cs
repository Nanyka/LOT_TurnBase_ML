using System.Collections.Generic;
using GOAP;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CheckInProcessArea : GAction
    {
        [SerializeField] private float _checkDistance = 1f;

        private BuildingConstructComp[] _testTarget; // TODO: refactor to get it from buildingManager
        private List<ICheckableObject> _checkableObjects = new();
        private ICheckableObject _currentPoint;

        private void Start()
        {
            _testTarget = FindObjectsByType<BuildingConstructComp>(FindObjectsSortMode.None);

            foreach (var target in _testTarget)
            {
                if (target.TryGetComponent(out ICheckableObject checkableObject))
                    _checkableObjects.Add(checkableObject);
            }
        }

        public override bool PrePerform()
        {
            var distanceToTarget = float.PositiveInfinity;
            foreach (var target in _checkableObjects)
            {
                if (target.IsCheckable() == true)
                    continue;
                
                var curDis = Vector3.Distance(transform.position, target.GetPosition());
                if (curDis < distanceToTarget)
                {
                    distanceToTarget = curDis;
                    _currentPoint = target;
                }
            }

            if (distanceToTarget < _checkDistance)
            {
                m_GAgent.Inventory.AddItem(_currentPoint.GetGameObject());
                _currentPoint.ReduceCheckableAmount(1);
                m_GAgent.Beliefs.RemoveState("Empty");
                m_GAgent.Beliefs.ModifyState("targetAvailable", 1);
                Duration = 1f;
            }
            else
                Duration = 0f;

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}