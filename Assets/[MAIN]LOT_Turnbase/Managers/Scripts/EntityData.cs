using UnityEngine;

namespace LOT_Turnbase
{
    [System.Serializable]
    public class EntityData
    {
        public string EntityName;
        public Vector3 Position;
        public FactionType CreatureType;
        public int CurrentHp;
    }
}