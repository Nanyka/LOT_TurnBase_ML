using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    [Serializable]
    public class EnvironmentData
    {
        public long timestamp;
        public long lastTimestamp;
        public long moveTimestamp;
        public int mapSize;
        public List<ResourceData> ResourceData;
        public List<BuildingData> BuildingData;
        public List<CreatureData> PlayerData;
        public List<CreatureData> EnemyData;
        public List<CollectableData> CollectableData;

        public EnvironmentData() { }

        public EnvironmentData(EnvironmentData cloneParent)
        {
            ResourceData = new();
            foreach (var data in cloneParent.ResourceData)
                ResourceData.Add(new ResourceData(data));

            BuildingData = new();
            foreach (var data in cloneParent.BuildingData)
                BuildingData.Add(new BuildingData(data));

            PlayerData = new();
            foreach (var data in cloneParent.PlayerData)
                PlayerData.Add(new CreatureData(data));

            EnemyData = new();
            foreach (var data in cloneParent.EnemyData)
                EnemyData.Add(new CreatureData(data));

            CollectableData = new();
            foreach (var data in cloneParent.CollectableData)
                CollectableData.Add(new CollectableData(data));
        }

        // Shallow copy method
        public EnvironmentData DeepCopy()
        {
            return new EnvironmentData(this);
        }

        public void AddBuildingData(BuildingData data)
        {
            BuildingData.Add(data);
        }

        public void AddPlayerData(CreatureData data)
        {
            PlayerData.Add(data);
        }

        public void AddResourceData(ResourceData data)
        {
            ResourceData.Add(data);
        }

        public void AddCollectableData(CollectableData data)
        {
            CollectableData.Add(data);
        }

        public void AddEnemyData(CreatureData data)
        {
            EnemyData.Add(data);
        }

        public bool CheckStorable()
        {
            return ResourceData.Any() || BuildingData.Any();
        }

        public bool CheckFullCapacity()
        {
            var totalSpace = BuildingData.Count(t => t.BuildingType == BuildingType.TOWNHOUSE);
            return (1 + totalSpace) * SavingSystemManager.Instance.GetTownhouseSpace() <= PlayerData.Count;
        }

        public int GetTownhouseSpace()
        {
            var totalSpace = BuildingData.Count(t => t.BuildingType == BuildingType.TOWNHOUSE);
            return (1 + totalSpace) * SavingSystemManager.Instance.GetTownhouseSpace();
        }

        public bool CheckEnemy(Vector3 atPos)
        {
            return EnemyData.Any(t => Vector3.Distance(t.Position, atPos) < 0.1f);
        }

        public bool CheckEnemy(Vector3 atPos, FactionType fromFaction)
        {
            if (fromFaction == FactionType.Player)
                return EnemyData.Any(t => Vector3.Distance(t.Position, atPos) < 0.1f);
            
            return PlayerData.Any(t => Vector3.Distance(t.Position, atPos) < 0.1f);
        }
        
        public bool CheckCohort(Vector3 atPos, FactionType fromFaction)
        {
            if (fromFaction == FactionType.Enemy)
                return EnemyData.Any(t => Vector3.Distance(t.Position, atPos) < 0.1f);
            
            return PlayerData.Any(t => Vector3.Distance(t.Position, atPos) < 0.1f);
        }

        public bool CheckBuilding(Vector3 atPos)
        {
            return BuildingData.Any(t => Vector3.Distance(t.Position, atPos) < 0.1f);
        }

        public bool CheckResource(Vector3 atPos)
        {
            return ResourceData.Any(t => Vector3.Distance(t.Position, atPos) < 0.1f);
        }

        #region BATTLE MODE

        public void PrepareForBattleMode(List<CreatureData> playerData)
        {
            foreach (var building in BuildingData)
                building.FactionType = FactionType.Enemy;

            EnemyData.Clear();
            PlayerData.Clear();
        }

        public void DepositRemainPlayerTroop(List<CreatureData> playerData)
        {
            foreach (var creatureData in playerData)
                PlayerData.Add(creatureData);
        }

        public void StoreRewardAtBuildings(string currencyId, int amount)
        {
            if (currencyId.Equals("GOLD") || currencyId.Equals("GEM"))
                return;

            // Check if enough storage space
            int currentStorage = 0;
            List<BuildingData> selectedBuildings = new List<BuildingData>();
            if (Enum.TryParse(currencyId, out CurrencyType currencyType))
                foreach (var t in BuildingData)
                    currentStorage += t.GetStoreSpace(currencyType, ref selectedBuildings);

            if (amount > currentStorage)
                Debug.Log($"Lack of {currencyId} STORAGE. Current storage is {currentStorage} and need for {amount}");

            amount = amount > currentStorage ? currentStorage : amount;

            if (amount == 0 || selectedBuildings.Count == 0)
                return;

            GeneralAlgorithm.Shuffle(selectedBuildings); // Shuffle buildings to ensure random selection

            // Stock currency to building and gain exp
            foreach (var building in selectedBuildings)
            {
                if (amount <= 0)
                    break;
                var storeAmount = building.GetStoreSpace(currencyType);
                storeAmount = storeAmount > amount ? amount : storeAmount;
                building.StoreCurrency(storeAmount);
                SavingSystemManager.Instance.IncrementLocalCurrency(currencyId, storeAmount);
                amount -= storeAmount;
            }
        }

        #endregion

        #region SCORE

        // public int CalculateScore()
        // {
        //     int totalScore = 0;
        //     foreach (var building in BuildingData)
        //         totalScore += (building.CurrentHp + building.CurrentDamage + building.CurrentShield) *
        //                       (1 + building.CurrentLevel);
        //
        //     foreach (var creature in PlayerData)
        //         totalScore += (creature.CurrentHp + creature.CurrentDamage + creature.CurrentShield) *
        //                       (1 + creature.CurrentLevel);
        //
        //     return totalScore;
        // }

        #endregion

        #region WIN MECHANIC

        public bool IsDemolishMainHall()
        {
            return BuildingData.Count(t => t.EntityName == "MainHall") == 0;
        }

        public int CountEnemyBuilding(FactionType enemyFaction)
        {
            return BuildingData.Count(t => t.FactionType == enemyFaction);
        }

        public void GatherCreature(string creatureName)
        {
            var creatureLevel = SavingSystemManager.Instance.GetInventoryLevel(creatureName);
            var creatureStats =
                (UnitStats)AddressableManager.Instance.GetAddressableSO(
                    $"/Stats/Creature/{creatureName}_lv{creatureLevel}");

            var newCreature = new CreatureData()
            {
                EntityName = creatureName,
                CurrentLevel = 0,
                FactionType = FactionType.Player,
                CreatureType = CreatureType.PLAYER,
                CurrentHp = creatureStats.HealthPoint,
                CurrentDamage = creatureStats.Strengh
            };

            PlayerData.Add(newCreature);
        }

        #endregion

        public void RemoveZeroHpPlayerCreatures()
        {
            PlayerData = PlayerData.FindAll(t => t.CurrentHp > 0 || t.EntityName.Equals("King"));
        }

        public void AbstractInBattleCreatures(List<CreatureData> inbattleCreatures)
        {
            foreach (var creature in inbattleCreatures)
            {
                if (PlayerData.Contains(creature) && creature.EntityName.Equals("King") == false)
                    PlayerData.Remove(creature);
            }
        }
    }
}