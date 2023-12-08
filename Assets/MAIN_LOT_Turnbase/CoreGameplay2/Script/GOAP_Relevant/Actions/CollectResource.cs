using System;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectResource : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Character;
        [SerializeField] private float _checkDistance = 1f;

        public override bool PrePerform()
        {
            if (m_GAgent.Inventory.IsEmpty())
                return false;

            var target = m_GAgent.Inventory.items[0];

            if (target.TryGetComponent(out ICheckableObject checkableObject))
            {
                var distanceToTarget = Vector3.Distance(checkableObject.GetPosition(), transform.position);
            
                if (distanceToTarget < _checkDistance)
                {
                    Target = target;
                    m_GAgent.SetIProcessUpdate(this);
                    
                    Duration = 1f;
                }
                else
                    Duration = 0f;
            }

            return true;
        }

        public override bool PostPerform()
        {
            m_GAgent.Inventory.ClearInventory();
            return true;
        }

        // public void ExecuteAttack(GameObject target)
        // {
        //     // Get capacity of the resource
        //     // If the resource is empty --> clear the inventory
        //     // Get strength data
        //     // Execute harvesting animation
        //     // and remaining resource amount
        //     
        //     if (target.TryGetComponent(out ICheckableObject checkableObject))
        //     {
        //         var myData = m_Character.GetData() as CreatureData;
        //         var myStrength = m_Character.GetStats().Strength;
        //         myStrength = myData.TurnCount > 0 ? myStrength - myData.TurnCount : myStrength;
        //         var remainResource = checkableObject.GetRemainAmount();
        //         var collectedAmount = remainResource > myStrength ? myStrength : remainResource;
        //
        //         if (remainResource > myStrength)
        //         {
        //             m_GAgent.Beliefs.RemoveState("Empty");
        //             m_GAgent.Beliefs.ModifyState("targetAvailable", 1);
        //         }
        //         myData.TurnCount += collectedAmount;
        //         checkableObject.ReduceCheckableAmount(collectedAmount);
        //     }
        // }

        public async void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            myTransform.LookAt(new Vector3(targetPos.x, myTransform.position.y, targetPos.z));
            m_Character.StartAttack();
            await WaitToCompleteTheAction();
        }
        
        private async Task WaitToCompleteTheAction()
        {
            await Task.Delay(Mathf.RoundToInt(Duration * 1000));
            StopProcess();
        }

        public void StopProcess()
        {
            m_GAgent.FinishFromOutside();
        }
    }
}