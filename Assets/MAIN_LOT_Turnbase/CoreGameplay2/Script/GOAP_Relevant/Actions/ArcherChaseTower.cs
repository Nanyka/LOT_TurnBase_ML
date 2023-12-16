using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ArcherChaseTower : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;

        private ICheckableObject _currentPoint;
        private ISensor _sensor;
        private float _distanceToAttack;
        
        private void Start()
        {
            _sensor = GetComponent<ISensor>();
            _distanceToAttack = _sensor.DetectRange() + 1f;
        }

        public override bool PrePerform()
        {
            if (m_GAgent.Inventory.IsEmpty())
            {
                _currentPoint = null;
                var distanceToTarget = float.PositiveInfinity;
                var buildings = SavingSystemManager.Instance.GetEnvLoader().GetBuildings(FactionType.Enemy);
                
                foreach (var building in buildings)
                {
                    if (building.TryGetComponent(out ICheckableObject checkableObject))
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

            if (m_GAgent.Inventory.items.Count == 0)
                return false;
            
            // Target = m_GAgent.Inventory.items[0];
            // m_GAgent.SetIProcessUpdate(this);
            
            var pointB = m_GAgent.Inventory.items[0].transform.position;
            var pointA = transform.position;
            var distanceToTower = Vector3.Distance(pointA, pointB);

            if (distanceToTower > _distanceToAttack)
            {
                Vector3 vectorAB = pointB - pointA;
                Vector3 scaledDirection = vectorAB.normalized * _distanceToAttack;
                Vector3 midpoint = pointA + scaledDirection;
                m_GAgent.SetIProcessUpdate(this, midpoint);
                // Debug.Log($"Archer move to {midpoint} to attack tower");
            }
            else
                m_GAgent.SetIProcessUpdate(this, transform.position);

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }

        public void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            if (Vector3.Distance(myTransform.position, targetPos) < m_Entity.GetStopDistance())
                m_GAgent.FinishFromOutside();
            else
                m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_GAgent.FinishFromOutside();
        }
    }
}