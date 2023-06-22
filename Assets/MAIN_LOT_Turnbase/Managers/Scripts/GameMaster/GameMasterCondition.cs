using Sirenix.OdinInspector;
using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class GameMasterCondition
    {
        [Tooltip("True if map size equal or larger than this amount")]
        public int MapSize;
        
        public CurrencyType Currency;
        [Tooltip("True if current balance lower than this amount")]
        [HideIf("Currency", CurrencyType.NONE)] public int CurrencyAmount;
        
        public CurrencyType Storage;
        [Tooltip("True if current storage larger than this amount")]
        [HideIf("Storage", CurrencyType.NONE)] public int StorageAmount;
        
        public CurrencyType Resource;
        [Tooltip("True if current resource lower than this amount")]
        [HideIf("Resource", CurrencyType.NONE)] public int ResourceAmount;

        public bool PassCondition()
        {
            return CheckCurrency() && CheckBuilding() && CheckResource();
        }

        private bool CheckMapSize()
        {
            return SavingSystemManager.Instance.GetEnvironmentData().mapSize >= MapSize;
        }

        private bool CheckCurrency()
        {
            if (Currency == CurrencyType.NONE)
                return true;
            
            return !SavingSystemManager.Instance.CheckEnoughCurrency(Currency.ToString(), CurrencyAmount);
        }

        private bool CheckBuilding()
        {
            if (Storage == CurrencyType.NONE)
                return true;
            
            var buildings = SavingSystemManager.Instance.GetEnvironmentData().BuildingData;
            int totalStorage = 0;
            foreach (var building in buildings)
                if (building.StorageCurrency == Storage)
                    totalStorage += building.StorageCapacity - building.CurrentStorage;
            return totalStorage >= StorageAmount;
        }

        private bool CheckResource()
        {
            if (Resource == CurrencyType.NONE)
                return true;
            
            var resources = SavingSystemManager.Instance.GetEnvironmentData().ResourceData;
            int totalAmount = 0;
            foreach (var resource in resources)
                if (resource.CollectedCurrency == Resource)
                    totalAmount++;
            return totalAmount < ResourceAmount;
        }
    }
}