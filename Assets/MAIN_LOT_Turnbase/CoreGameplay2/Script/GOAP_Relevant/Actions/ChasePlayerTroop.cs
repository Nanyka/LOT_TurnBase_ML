using System;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ChasePlayerTroop : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        
        private float distanceToAttack;

        private ICheckableObject _currentPoint;
        private ISensor _sensor;

        private void Start()
        {
            _sensor = GetComponent<ISensor>();
            distanceToAttack = _sensor.DetectRange();
        }

        public override bool PrePerform()
        {
            var target = _sensor.ExecuteSensor();
            if (target == null)
            {
                // _sensor.ResetSensor();
                return false;
            }

            Target = target;
            m_GAgent.SetIProcessUpdate(this);

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