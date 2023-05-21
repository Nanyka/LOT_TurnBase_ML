using UnityEngine;

namespace LOT_Turnbase
{
    [System.Serializable]
    public class CreatureData
    {
        public Vector3 Position;
        public FactionType CreatureType;
        public int CurrentHp;
        public int CurrentShield;
        public int CurrentExp;
        public int TurnCount;
    }
}