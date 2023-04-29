using UnityEngine;

namespace LOT_Turnbase
{
    [System.Serializable]
    public class ResourceData
    {
        public Vector3 Position;
        public ResourceType ResourceType;
        public int CurrentHealth;
        public int CurrentTurn;
        public int Level;
    }
}