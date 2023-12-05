using System.Linq;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class MoveToResource : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        
        private ICheckableObject _currentPoint;

        public override bool PrePerform()
        {
            var distanceToTarget = float.PositiveInfinity;
            if (m_GAgent.Inventory.IsEmpty())
            {
                _currentPoint = null;
                var resources = SavingSystemManager.Instance.GetEnvLoader().GetResources();
                foreach (var resource in resources)
                {
                    if (resource.TryGetComponent(out ICheckableObject checkableObject))
                    {
                        if (checkableObject.IsCheckable() == false)
                            continue;

                        var curDis = Vector3.Distance(transform.position, checkableObject.GetPosition());
                        if (curDis < distanceToTarget)
                        {
                            distanceToTarget = curDis;
                            _currentPoint = checkableObject;
                        }
                    }
                }

                if (_currentPoint != null)
                    m_GAgent.Inventory.AddItem(_currentPoint.GetGameObject());
            }

            Target = m_GAgent.Inventory.items[0];
            m_GAgent.SetIProcessUpdate(this);
            
            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }

        public void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            // m_Entity.StopMoving();
            m_GAgent.FinishFromOutside();
        }
    }
}