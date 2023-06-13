using System.Collections.Generic;

namespace JumpeeIsland
{
    [System.Serializable]
    public class EnvironmentData
    {
        public long timestamp;
        public long lastTimestamp;
        public long moveTimestamp;
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
    }
}