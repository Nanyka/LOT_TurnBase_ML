using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeHarvestComp : MonoBehaviour, IAttackComp
    {
        [SerializeField] private HarvesterBrain m_Brain;

        private IGetEntityData<CreatureStats> m_Data;

        private void Start()
        {
            m_Data = GetComponent<IGetEntityData<CreatureStats>>();
        }

        public void SuccessAttack(GameObject target)
        {
            // Get capacity of the resource
            // If the resource is empty --> clear the inventory
            // Get strength data
            // Execute harvesting animation
            // and remaining resource amount
            
            if (target.TryGetComponent(out ICheckableObject checkableObject))
            {
                var myData = m_Data.GetData() as CreatureData;
                var myStrength = m_Data.GetStats().Strength;
                myStrength = myData.TurnCount > 0 ? myStrength - myData.TurnCount : myStrength;
                var remainResource = checkableObject.GetRemainAmount();
                var collectedAmount = remainResource > myStrength ? myStrength : remainResource;

                if (remainResource > myStrength)
                {
                    m_Brain.Beliefs.RemoveState("Empty");
                    m_Brain.Beliefs.ModifyState("targetAvailable", 1);
                }
                myData.TurnCount += collectedAmount;
                checkableObject.ReduceCheckableAmount(collectedAmount);
            }
        }
    }
}