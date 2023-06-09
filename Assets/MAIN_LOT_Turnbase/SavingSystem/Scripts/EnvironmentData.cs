using System.Collections.Generic;
using System.Linq;

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

        public void PrepareForBattleSave(List<CreatureData> playerData, bool isFinishPlacing)
        {
            // If finish placing creatures, PlayerData will be empty
            if (isFinishPlacing)
                if (playerData.Count == 0)
                {
                    PlayerData = new List<CreatureData>();
                    return;
                }

            // If playerData is empty mean player still not place any creature on environment
            if (playerData.Count == 0)
                return;

            int checkingIndex = 0;
            foreach (var data in PlayerData)
            {
                if (checkingIndex >= playerData.Count)
                {
                    data.CurrentHp = 0;
                    continue;
                }

                if (data.EntityName.Equals(playerData[checkingIndex].EntityName))
                {
                    data.CurrentHp = playerData[checkingIndex].CurrentHp;
                    data.CurrentExp = playerData[checkingIndex].CurrentExp;
                    checkingIndex++;
                }
                else
                    data.CurrentHp = 0;
            }

            PlayerData = PlayerData.FindAll(t => t.CurrentHp > 0);
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
    }
}