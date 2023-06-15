using System.Collections.Generic;

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

        public void AddBuildingData(BuildingData data)
        {
            BuildingData.Add(data);
        }

        public void AddPlayerData(CreatureData data)
        {
            PlayerData.Add(data);
        }

        public void AddResourceData(ResourceData resourceData)
        {
            ResourceData.Add(resourceData);
        }

        public void PrepareForBattleMode(List<CreatureData> playerData)
        {
            EnemyData.Clear();
            foreach (var creatureData in PlayerData)
                EnemyData.Add(creatureData);

            PlayerData.Clear();
        }
    }
}