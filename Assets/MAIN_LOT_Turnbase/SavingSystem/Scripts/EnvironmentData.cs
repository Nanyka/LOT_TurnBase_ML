using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
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

        #region BATTLE MODE

        public void PrepareForBattleMode(List<CreatureData> playerData)
        {
            foreach (var building in BuildingData)
                building.FactionType = FactionType.Enemy;

            EnemyData.Clear();
            foreach (var creatureData in PlayerData)
            {
                creatureData.FactionType = FactionType.Enemy;
                EnemyData.Add(creatureData);
            }

            PlayerData.Clear();
        }

        public void DepositRemainPlayerTroop(List<CreatureData> playerData)
        {
            foreach (var creatureData in playerData)
                PlayerData.Add(creatureData);
        }

        // public void PrepareForBattleSave(List<CreatureData> playerData, bool isFinishPlacing)
        // {
        //     // If finish placing creatures, PlayerData will be empty
        //     // if (isFinishPlacing)
        //     //     if (playerData.Count == 0)
        //     //     {
        //     //         PlayerData = PlayerData.FindAll(t => t.EntityName.Equals("King"));
        //     //         return;
        //     //     }
        //
        //     // If playerData is empty mean player still not place any creature on environment
        //     if (playerData.Count == 0)
        //         return;
        //
        //     // var kingData = PlayerData.Find(t => t.EntityName.Equals("King"));
        //     PlayerData = new List<CreatureData>(playerData);
        //     // if (kingData != null)
        //     // {
        //     //     PlayerData.Add(kingData);
        //     // }
        //
        // }

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

        public int CalculateScore()
        {
            int totalScore = 0;
            foreach (var building in BuildingData)
                totalScore += (building.CurrentHp + building.CurrentDamage + building.CurrentShield) *
                              (1 + building.CurrentLevel);

            foreach (var creature in PlayerData)
                totalScore += (creature.CurrentHp + creature.CurrentDamage + creature.CurrentShield) *
                              (1 + creature.CurrentLevel);

            return totalScore;
        }

        #endregion

        #region WIN MECHANIC

        public bool IsDemolishMainHall()
        {
            return BuildingData.Count(t => t.EntityName == "MainHall") == 0;
        }

        public void GatherCreature(string creatureName)
        {
            var creatureStats =
                (UnitStats)AddressableManager.Instance.GetAddressableSO($"/Stats/Creature/{creatureName}_lv0");

            var newCreature = new CreatureData()
            {
                EntityName = creatureName,
                // Position = GetFreeLocation(),
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
            PlayerData = PlayerData.FindAll(t => t.CurrentHp > 0);
        }
    }
}