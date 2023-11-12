using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ConstructorContinuesTask : GAction
    {
        [SerializeField] private float _checkDistance = 1f;
        private ICheckableObject _currentPoint;

        public override bool PrePerform()
        {
            var currentTarget = m_GAgent.Inventory.FindItemWithTag("Factory");

            if (currentTarget != null)
            {
                if (currentTarget.TryGetComponent(out ICheckableObject checkableObject))
                {
                    _currentPoint = checkableObject;
                    if (Vector3.Distance(transform.position, _currentPoint.GetPosition()) < _checkDistance &&
                        _currentPoint.IsCheckable() == false)
                    {
                        _currentPoint.ReduceCheckableAmount(1);
                        Duration = 1f;
                        Debug.Log($"Constructing {currentTarget}");
                        return false;
                    }
                }
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