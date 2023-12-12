using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ChasePlayerTroop : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        [SerializeField] private float distanceFromAssembly;

        private ICheckableObject _currentPoint;

        public override bool PrePerform()
        {
            if (m_GAgent.Inventory.IsEmpty())
                return false;

            if (Vector3.Distance(m_Entity._assemblyPoint, m_GAgent.Inventory.items[0].transform.position) >
                distanceFromAssembly)
                return false;

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
            if (Vector3.Distance(myTransform.position, targetPos) < m_Entity.GetStopDistance())
                m_GAgent.FinishFromOutside();
            else
                m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_GAgent.Inventory.ClearInventory();
            m_GAgent.FinishFromOutside();
        }
    }
}