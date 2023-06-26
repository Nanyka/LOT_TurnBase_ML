using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [System.Serializable]
    public class GameMasterCondition
    {
        [Tooltip("True if map size equal or larger than this amount")]
        public int MapSize;

        [Tooltip("True if current balance lower than this amount")]
        public CurrencyType Currency;

        [HideIf("Currency", CurrencyType.NONE)]
        public int CurrencyAmount;

        [Tooltip("True if current storage larger than this amount")]
        public CurrencyType Storage;

        [HideIf("Storage", CurrencyType.NONE)] public int StorageAmount;

        [Tooltip("True if current resource lower than this amount")]
        public CurrencyType Resource;

        [HideIf("Resource", CurrencyType.NONE)]
        public int ResourceAmount;

        [FormerlySerializedAs("Collectable")] [Tooltip("True if current collectable lower than this amount")]
        public CollectableType CollectableType;

        [HideIf("CollectableType", CollectableType.NONE)]
        public int CollectableAmount;

        [Tooltip("True if a specific building is lower than this amount")]
        public BuildingType BuildingType;

        [HideIf("BuildingType", BuildingType.NONE)]
        public int BuildingAmount;

        public bool PassCondition()
        {
            return CheckCurrency() && CheckStorageSpace() && CheckResource() && CheckCollectable() &&
                   CheckBuildingType();
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

        private bool CheckStorageSpace()
        {
            if (Storage == CurrencyType.NONE)
                return true;

            var buildings = SavingSystemManager.Instance.GetEnvironmentData().BuildingData;
            int totalStorage = 0;
            foreach (var building in buildings)
                if (building.StorageCurrency == Storage || building.StorageCurrency == CurrencyType.MULTI)
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

        private bool CheckCollectable()
        {
            if (CollectableType == CollectableType.NONE)
                return true;

            var collectables = SavingSystemManager.Instance.GetEnvironmentData().CollectableData;
            if (collectables == null)
                return true;
            
            int totalAmount = 0;
            foreach (var collectable in collectables)
                if (collectable.CollectableType == CollectableType)
                    totalAmount++;

            return totalAmount < CollectableAmount;
        }

        private bool CheckBuildingType()
        {
            if (BuildingType == BuildingType.NONE)
                return true;

            var buildings = SavingSystemManager.Instance.GetEnvironmentData().BuildingData;
            int totalAmount = 0;
            foreach (var building in buildings)
                if (building.BuildingType == BuildingType)
                    totalAmount++;

            return totalAmount < BuildingAmount;
        }
    }
}