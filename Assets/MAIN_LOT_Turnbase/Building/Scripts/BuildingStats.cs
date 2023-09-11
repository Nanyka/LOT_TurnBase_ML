using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    // [CreateAssetMenu(fileName = "BuildingStats", menuName = "JumpeeIsland/BuildingStats", order = 4)]
    [System.Serializable]
    public class BuildingStats
    {
        public BuildingType BuildingType;
        public CurrencyType StoreCurrency;
        public int MaxHp;
        public int Level;
        public CurrencyType UpgradeCurrency;
        public int PriceToUpdate;
        public int StorageCapacity;
        public int AttackDamage;
        public float AttackRange;
        public int Shield;
        public int ExpReward;
    }

    [System.Serializable]
    public class BuildingStatsCollection
    {
        public List<BuildingStats> statsCollection;
    }
}
