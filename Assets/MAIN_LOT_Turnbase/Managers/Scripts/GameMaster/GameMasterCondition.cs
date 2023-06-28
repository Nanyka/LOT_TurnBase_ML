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

        [VerticalGroup("Currency",VisibleIf = "@Currency != JumpeeIsland.CurrencyType.NONE")]
        [VerticalGroup("Currency/Row2")] public CompareType CurrencyCompare;
        [VerticalGroup("Currency/Row1")] public int CurrencyAmount;

        public CurrencyType Storage;

        [VerticalGroup("Storage",VisibleIf = "@Storage != JumpeeIsland.CurrencyType.NONE")]
        [VerticalGroup("Storage/Row2")] public CompareType StorageCompare;
        [VerticalGroup("Storage/Row1")] public int StorageAmount;

        public CurrencyType Resource;

        [VerticalGroup("Resource",VisibleIf = "@Resource != JumpeeIsland.CurrencyType.NONE")]
        [VerticalGroup("Resource/Row2")] public CompareType ResourceCompare;
        [VerticalGroup("Resource/Row1")] public int ResourceAmount;

        public CollectableType Collectable;

        [VerticalGroup("CollectableType",VisibleIf = "@Collectable != JumpeeIsland.CollectableType.NONE")]
        [VerticalGroup("CollectableType/Row2")] public CompareType CollectableCompare;
        [VerticalGroup("CollectableType/Row1")] public int CollectableAmount;

        public BuildingType Building;

        [VerticalGroup("Building",VisibleIf = "@Building != JumpeeIsland.BuildingType.NONE")]
        [VerticalGroup("Building/Row2")] public CompareType BuildingCompare;
        [VerticalGroup("Building/Row1")] public int BuildingAmount;

        public bool IsUICondition;
        [ShowIf("@IsUICondition == true")] public string UIElement;
        
        public bool IsScoreCondition;
        [VerticalGroup("Score",VisibleIf = "@IsScoreCondition == true")]
        [VerticalGroup("Score/Row2")] public CompareType ScoreCompare;
        [VerticalGroup("Score/Row1")] public int ScoreAmount;

        public bool PassCondition()
        {
            return CheckCurrency() && CheckStorageSpace() && CheckResource() && CheckCollectable() && CheckBuildingType() && CheckUICondition() && CheckScore();
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
                if (collectable.CollectableType == Collectable)
                    totalAmount++;

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

        private bool CheckUICondition()
        {
            if (IsUICondition == false)
                return true;

            return MainUI.Instance.CheckUIActive(UIElement);
        }

        private bool CheckScore()
        {
            if (IsScoreCondition == false)
                return true;

            var totalScore = SavingSystemManager.Instance.CalculateEnvScore();
            Debug.Log($"Check current score: {totalScore}");
            switch (BuildingCompare)
            {
                case CompareType.Higher:
                    return totalScore > ScoreAmount;
                case CompareType.Lower:
                    return totalScore < ScoreAmount;
                case CompareType.Equal:
                    return totalScore == ScoreAmount;
            }

            return true;
        }
    }
}