using GOAP;
using UnityEngine;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class ContributeResource : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;

        private IStoreResource _currentPoint;

        public override bool PrePerform()
        {
            if (m_GAgent.Inventory.IsEmpty())
            {
                var storage = SavingSystemManager.Instance.GetStorageController().GetRandomStorage();
                if (storage.IsFullStock() == false)
                {
                    _currentPoint = storage;
                    m_GAgent.Inventory.AddItem(storage.GetGameObject());
                }
                else
                {
                    ResetGOAPState();
                    return true;
                }
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
            m_GAgent.Beliefs.ModifyState("Empty", 1);
            m_GAgent.Beliefs.RemoveState("targetAvailable");
            m_GAgent.Inventory.ClearInventory();
        }
    }
}