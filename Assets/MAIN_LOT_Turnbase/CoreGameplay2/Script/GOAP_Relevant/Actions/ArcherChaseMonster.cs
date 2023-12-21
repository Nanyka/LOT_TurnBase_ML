using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ArcherChaseMonster : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;

        private ICheckableObject _currentPoint;
        private ISensor _sensor;
        private float distanceToAttack;

        private void Start()
        {
            _sensor = GetComponent<ISensor>();
            distanceToAttack = _sensor.DetectRange() -1f;
        }

        public override bool PrePerform()
        {
            var target = _sensor.ExecuteSensor();
            if (target == null)
                return false;

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
            m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_GAgent.FinishFromOutside();
        }
    }
}