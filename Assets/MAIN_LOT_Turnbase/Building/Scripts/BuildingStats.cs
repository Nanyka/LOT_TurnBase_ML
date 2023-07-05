using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "BuildingStats", menuName = "JumpeeIsland/BuildingStats", order = 4)]
    public class BuildingStats : ScriptableObject
    {
        public BuildingType BuildingType;
        public CurrencyType StoreCurrency;
        public int MaxHp;
        public int Level;
        public int ExpToUpdate;
        public int StorageCapacity;
        public int AttackDamage;
        public float AttackRange;
        public int Shield;
    }
}
