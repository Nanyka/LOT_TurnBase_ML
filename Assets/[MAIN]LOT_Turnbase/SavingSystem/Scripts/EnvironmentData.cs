using System.Collections.Generic;

namespace JumpeeIsland
{
    [System.Serializable]
    public class EnvironmentData
    {
        public long timestamp;
        public long currencyBalance;
        public List<ResourceData> _testResourceData;
        public List<BuildingData> _testBuildingData;
        public List<CreatureData> _testPlayerData;
        public List<CreatureData> _testEnemyData;
    }
}