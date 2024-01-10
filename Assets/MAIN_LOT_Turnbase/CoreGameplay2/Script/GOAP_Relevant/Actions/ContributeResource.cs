using GOAP;
using UnityEngine;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class ContributeResource : GAction, IProcessUpdate
    {
        [SerializeField] private AoeHarvesterEntity m_Entity;

        private IStoreResource _currentPoint;

        public override bool PrePerform()
        {
            _currentPoint = SavingSystemManager.Instance.GetStorageController().GetRandomStorage();
            if (_currentPoint == null)
            {
                ResetGOAPState();
                return false;
            }

            m_GAgent.SetIProcessUpdate(this, _currentPoint.GetGameObject().transform.position);

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }

        public void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            m_Entity.SetCarryingState(true);
            m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            // Store resource in factory & Reset stock
            var myData = m_Entity.GetData() as CreatureData;
            _currentPoint.StoreResource(myData.TurnCount);
            myData.TurnCount = 0;

            // Modify GOAP state
            ResetGOAPState();

            m_GAgent.FinishFromOutside();
        }

        private void ResetGOAPState()
        {
            m_GAgent.Beliefs.ClearStates();
            m_GAgent.Beliefs.AddState("Empty");
            
            // m_GAgent.Beliefs.RemoveState("targetAvailable");
        }
    }
}