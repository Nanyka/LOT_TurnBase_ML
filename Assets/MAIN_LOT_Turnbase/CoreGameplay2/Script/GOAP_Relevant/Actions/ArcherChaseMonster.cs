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
            distanceToAttack = _sensor.DetectRange();
        }

        public override bool PrePerform()
        {
            var target = _sensor.ExecuteSensor();
            if (target == null)
                return false;

            // Target = target;

            // Calculate the distance between the object and the monster
            // If the distance shorter than distanceToAttack --> FinishFromOutside
            // Else, calculate the position that place the object a position equal distanceToAttack

            var pointB = target.transform.position;
            var pointA = transform.position;
            var distanceToMonster = Vector3.Distance(pointA, pointB);

            if (distanceToMonster > distanceToAttack)
            {
                Vector3 vectorAB = pointB - pointA;
                float mag = vectorAB.magnitude;
                Vector3 normalizedDirection = vectorAB / mag;
                Vector3 scaledDirection = normalizedDirection * distanceToAttack;
                Vector3 midpoint = pointA + scaledDirection;
                m_GAgent.SetIProcessUpdate(this, midpoint);
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