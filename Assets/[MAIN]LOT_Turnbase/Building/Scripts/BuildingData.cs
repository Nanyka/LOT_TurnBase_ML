using UnityEngine;

namespace LOT_Turnbase
{
    [System.Serializable]
    public class BuildingData
    {
        public Vector3 Position;
        public BuildingType BuildingType;
        public int CurrentHp;
        public int CurrentShield;
        public int CurrentExp;
        public int CurrentStorage;
        public int TurnCount;
    }
}