using System;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ChasePlayerTroop : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        [SerializeField] private float distanceFromAssembly;

        private ICheckableObject _currentPoint;
        private ISensor _sensor;

        private void Start()
        {
            _sensor = GetComponent<ISensor>();
        }

        public override bool PrePerform()
        {
            var target = _sensor.ExecuteSensor();
            if (target == null)
                return false;

            // if (m_GAgent.Inventory.IsEmpty() ||
            //     Vector3.Distance(m_Entity._assemblyPoint, m_GAgent.Inventory.items[0].transform.position) >
            //     distanceFromAssembly)
            // {
            //     m_GAgent.Beliefs.ModifyState("SeePlayer", -1);
            //     return false;
            // }

            Target = target;
            m_GAgent.SetIProcessUpdate(this);

            return true;
        }

        public override bool PostPerform()
        {
            // m_GAgent.Beliefs.ModifyState("SeePlayer", -1);
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