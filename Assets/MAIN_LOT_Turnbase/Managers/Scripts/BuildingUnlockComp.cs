using System;
using Sirenix.Utilities;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingUnlockComp : MonoBehaviour
    {
        [SerializeField] private BuildingEntity m_BuildingEntity;

        public async void OnAskForUnlock()
        {
            if (m_BuildingEntity.GetBuildingType() != BuildingType.MAINHALL)
                return;
            
            if (GameFlowManager.Instance.GameMode != GameMode.ECONOMY) return;
            
            var upcomingTier = SavingSystemManager.Instance.GetUpcomingTier();

            foreach (var item in upcomingTier.TierItems)
                if (item.inventoryId.IsNullOrWhitespace())
                    await SavingSystemManager.Instance.GrantInventory(item.inventoryId);
            
            await SavingSystemManager.Instance.RefreshEconomy();
        }
    }
}