using System.Collections.Generic;

namespace LOT_Turnbase
{
    [System.Serializable]
    public class EnvironmentData
    {
        public List<ResourceData> _testResourceData;
        public List<BuildingData> _testBuildingData;
        public List<CreatureData> _testPlayerData;
        public List<CreatureData> _testEnemyData;
    }
}