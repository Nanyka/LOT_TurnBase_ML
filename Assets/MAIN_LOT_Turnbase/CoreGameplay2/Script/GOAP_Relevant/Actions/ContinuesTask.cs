using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ContinuesTask : GAction
    {
        [SerializeField] private float _checkDistance = 1f;
        [SerializeField] private string _targetTag;
        private ICheckableObject _currentPoint;

        public override bool PrePerform()
        {
            // var currentTarget = m_GAgent.Inventory.FindItemWithTag(_targetTag);
            //
            // if (currentTarget != null)
            // {
            //     if (currentTarget.TryGetComponent(out ICheckableObject checkableObject))
            //     {
            //         _currentPoint = checkableObject;
            //         if (Vector3.Distance(transform.position, _currentPoint.GetPosition()) < _checkDistance &&
            //             _currentPoint.IsCheckable() == false)
            //         {
            //             _currentPoint.ReduceCheckableAmount(1);
            //             Duration = 1f;
            //             return false;
            //         }
            //     }
            // }
            //
            // m_GAgent.Beliefs.RemoveState("targetAvailable");
            // m_GAgent.Inventory.ClearInventory();
            // Duration = 0f;
            
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
            {
                m_GAgent.Beliefs.RemoveState("targetAvailable");
                return false;
            }
            
            if (Vector3.Distance(transform.position, _currentPoint.GetPosition()) < _checkDistance &&
                _currentPoint.IsCheckable() == false)
            {
                _currentPoint.ReduceCheckableAmount(1);
                Duration = 1f;
                return false;
            }
            
            m_GAgent.Beliefs.RemoveState("targetAvailable");
            m_GAgent.Inventory.ClearInventory();
            Duration = 0f;
            
            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}