using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class BuildingData: EntityData
    {
        public BuildingType BuildingType;
        public int CurrentDamage;
        public int CurrentShield;
        public int CurrentExp;
        public int CurrentStorage;
        public int StorageCapacity;
        public int TurnCount;
        public CurrencyType StorageCurrency;

        public int GetStoreSpace(CurrencyType currencyType, ref List<BuildingData> buildingDatas)
        {
            if (currencyType == StorageCurrency)
            {
                buildingDatas.Add(this);
                return StorageCapacity - CurrentStorage;
            }

            return 0;
        }
        
        public int GetStoreSpace(CurrencyType currencyType)
        {
            if (currencyType == StorageCurrency)
                return StorageCapacity - CurrentStorage;

            return 0;
        }

        public void StoreCurrency(int amount)
        {
            CurrentStorage += amount;
        }
    }
}