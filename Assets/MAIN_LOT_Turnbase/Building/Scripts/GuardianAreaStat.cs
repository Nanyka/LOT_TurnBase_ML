using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [Serializable]
    public class GuardianAreaStat
    {
        public BuildingType BuildingType;
        public CurrencyType UpgradeCurrency;
        public int PriceToUpdate;
        public TroopType GuardianType = TroopType.NONE;
        public List<Guardian> Guardians = new();
    }

    [Serializable]
    public class Guardian
    {
        public string EntityName;
        public int Level;
    }
}