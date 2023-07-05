using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [System.Serializable]
    public class EntityData
    {
        public string EntityName;
        public string SkinAddress;
        public Vector3 Position;
        public Vector3 Rotation;
        public FactionType FactionType;
        public int CurrentHp;
        public int CurrentLevel;
    }
}