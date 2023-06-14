using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class GameMasterCondition
    {
        public CurrencyType Currency;
        [Tooltip("True if current balance lower than this amount")]
        public int CurrencyAmount;
        public CurrencyType Storage;
        [Tooltip("True if current storage larger than this amount")]
        public int StorageAmount;

        public bool PassCondition()
        {
            return CheckCurrency() && CheckBuilding();
        }

        private bool CheckCurrency()
        {
            if (CurrencyAmount <= 0)
                return true;
            
            return !SavingSystemManager.Instance.CheckEnoughCurrency(Currency.ToString(), CurrencyAmount);
        }

        private bool CheckBuilding()
        {
            if (StorageAmount <= 0)
                return true;
            
            var buildings = SavingSystemManager.Instance.GetEnvironmentData().BuildingData;
            int totalStorage = 0;
            foreach (var building in buildings)
                if (building.StorageCurrency == Storage)
                    totalStorage += building.StorageCapacity - building.CurrentStorage;
            return totalStorage >= StorageAmount;
        }
    }
}