using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class RangeUnitChasePlayerTroop : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        
        private float distanceToAttack;

        private ICheckableObject _currentPoint;
        private ISensor _sensor;

        private void Start()
        {
            _sensor = GetComponent<ISensor>();
            distanceToAttack = _sensor.DetectRange() - 1;
        }

        public override bool PrePerform()
        {
            var target = _sensor.ExecuteSensor();
            if (target == null)
            {
                // _sensor.ResetSensor();
                return false;
            }

            var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget > distanceToAttack)
            {
                float offset = distanceToAttack / distanceToTarget;
                var interpolatePos = Vector3.Lerp(target.transform.position, transform.position, offset);
                m_GAgent.SetIProcessUpdate(this,interpolatePos);
            }

            return true;
        }

        public override bool PostPerform()
        {
            m_GAgent.Inventory.ClearInventory();

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