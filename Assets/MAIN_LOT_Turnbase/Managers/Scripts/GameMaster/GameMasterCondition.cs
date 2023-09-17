using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public enum CompareType
    {
        Higher,
        Lower,
        Equal
    }

    [System.Serializable]
    public class GameMasterCondition
    {
        [Tooltip("True if map size equal or larger than this amount")]
        public int MapSize;

        public CurrencyType Currency;

        [VerticalGroup("Currency", VisibleIf = "@Currency != JumpeeIsland.CurrencyType.NONE")]
        [VerticalGroup("Currency/Row1")]
        public CompareType CurrencyCompare;

        [VerticalGroup("Currency/Row2")] public int CurrencyAmount;

        public CurrencyType Storage;

        [VerticalGroup("Storage", VisibleIf = "@Storage != JumpeeIsland.CurrencyType.NONE")]
        [VerticalGroup("Storage/Row1")]
        public CompareType StorageCompare;

        [VerticalGroup("Storage/Row2")] public int StorageAmount;

        public CurrencyType Resource;

        [VerticalGroup("Resource", VisibleIf = "@Resource != JumpeeIsland.CurrencyType.NONE")]
        [VerticalGroup("Resource/Row1")]
        public CompareType ResourceCompare;

        [VerticalGroup("Resource/Row2")] public int ResourceAmount;

        public CollectableType Collectable;

        [VerticalGroup("CollectableType", VisibleIf = "@Collectable != JumpeeIsland.CollectableType.NONE")]
        [VerticalGroup("CollectableType/Row1")]
        public CompareType CollectableCompare;

        [VerticalGroup("CollectableType/Row2")]
        public int CollectableAmount;

        public BuildingType Building;

        [VerticalGroup("Building", VisibleIf = "@Building != JumpeeIsland.BuildingType.NONE")]
        [VerticalGroup("Building/Row1")]
        public CompareType BuildingCompare;

        [VerticalGroup("Building/Row2")] public int BuildingAmount;

        public CreatureType Creature;

        [VerticalGroup("Creature", VisibleIf = "@Creature != JumpeeIsland.CreatureType.NONE")]
        [VerticalGroup("Creature/Row1")]
        public CompareType CreatureCompare;

        [VerticalGroup("Creature/Row2")] public int CreatureAmount;

        public bool IsExpCondition;
        [ShowIf("@IsExpCondition == true")] public int playerExp;

        public bool IsUICondition;
        [ShowIf("@IsUICondition == true")] public string UIElement;

        public bool IsSelectEntity;
        [ShowIf("@IsSelectEntity == true")] public string EntityName;

        public bool IsCheckBattle;
        [ShowIf("@IsCheckBattle == true")] public int BattleCount;

        public bool IsCheckBossUnlock;
        [ShowIf("@IsCheckBossUnlock == true")] public int BossUnlockAmount;

        public bool CheckPass()
        {
            return CheckMapSize() && CheckCurrency() && CheckStorageSpace() && CheckResource() && CheckCollectable() &&
                   CheckBuildingType() && CheckCreatureType() && CheckUICondition() && CheckBattleCount() &&
                   CheckBossUnlock() && CheckEntityCondition() && CheckPlayerExp();
        }

        private bool CheckMapSize()
        {
            return SavingSystemManager.Instance.GetEnvironmentData().mapSize >= MapSize;
        }

        private bool CheckCurrency()
        {
            if (Currency == CurrencyType.NONE)
                return true;

            var checkAmount = SavingSystemManager.Instance.GetCurrencyById(Currency.ToString());

            switch (CurrencyCompare)
            {
                case CompareType.Higher:
                    return checkAmount > CurrencyAmount;
                case CompareType.Lower:
                    return checkAmount < CurrencyAmount;
                case CompareType.Equal:
                    return checkAmount == CurrencyAmount;
            }

            return true;
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

            switch (StorageCompare)
            {
                case CompareType.Higher:
                    return totalStorage > StorageAmount;
                case CompareType.Lower:
                    return totalStorage < StorageAmount;
                case CompareType.Equal:
                    return totalStorage == StorageAmount;
            }

            return true;
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

            switch (ResourceCompare)
            {
                case CompareType.Higher:
                    return totalAmount > ResourceAmount;
                case CompareType.Lower:
                    return totalAmount < ResourceAmount;
                case CompareType.Equal:
                    return totalAmount == ResourceAmount;
            }

            return true;
        }

        private bool CheckCollectable()
        {
            if (Collectable == CollectableType.NONE)
                return true;

            var collectables = SavingSystemManager.Instance.GetEnvironmentData().CollectableData;
            if (collectables == null)
                return true;

            int totalAmount = 0;
            foreach (var collectable in collectables)
            {
                if (collectable.CollectableType == Collectable)
                    totalAmount++;
            }
            
            switch (CollectableCompare)
            {
                case CompareType.Higher:
                    return totalAmount > CollectableAmount;
                case CompareType.Lower:
                    return totalAmount < CollectableAmount;
                case CompareType.Equal:
                    return totalAmount == CollectableAmount;
            }

            return true;
        }

        private bool CheckBuildingType()
        {
            if (Building == BuildingType.NONE)
                return true;

            var buildings = SavingSystemManager.Instance.GetEnvironmentData().BuildingData;
            int totalAmount = 0;
            foreach (var building in buildings)
                if (building.BuildingType == Building)
                    totalAmount++;

            switch (BuildingCompare)
            {
                case CompareType.Higher:
                    return totalAmount > BuildingAmount;
                case CompareType.Lower:
                    return totalAmount < BuildingAmount;
                case CompareType.Equal:
                    return totalAmount == BuildingAmount;
            }

            return true;
        }

        private bool CheckCreatureType()
        {
            if (Creature == CreatureType.NONE)
                return true;

            var creatures = SavingSystemManager.Instance.GetEnvironmentData().EnemyData;
            if (Creature == CreatureType.PLAYER)
                creatures = SavingSystemManager.Instance.GetEnvironmentData().PlayerData;

            int totalAmount = 0;
            foreach (var creature in creatures)
                if (creature.CreatureType == Creature)
                    totalAmount++;

            switch (CreatureCompare)
            {
                case CompareType.Higher:
                    return totalAmount > CreatureAmount;
                case CompareType.Lower:
                    return totalAmount < CreatureAmount;
                case CompareType.Equal:
                    return totalAmount == CreatureAmount;
            }

            return true;
        }

        private bool CheckUICondition()
        {
            if (IsUICondition == false)
                return true;

            return MainUI.Instance.CheckUIActive(UIElement);
        }

        private bool CheckEntityCondition()
        {
            if (IsSelectEntity == false)
                return true;

            if (MainUI.Instance.GetSelectedEntity() == null)
                return false;

            return MainUI.Instance.GetSelectedEntity().GetData().EntityName.Equals(EntityName);
        }

        private bool CheckBattleCount()
        {
            if (IsCheckBattle == false)
                return true;

            return SavingSystemManager.Instance.GetGameProcess().battleCount >= BattleCount;
        }

        private bool CheckBossUnlock()
        {
            if (IsCheckBossUnlock == false)
                return true;

            return SavingSystemManager.Instance.GetGameProcess().bossUnlock >= BossUnlockAmount;
        }

        private bool CheckPlayerExp()
        {
            if (IsExpCondition == false)
                return true;

            return SavingSystemManager.Instance.GetGameProcess().CalculateExp() > playerExp;
        }
    }
}