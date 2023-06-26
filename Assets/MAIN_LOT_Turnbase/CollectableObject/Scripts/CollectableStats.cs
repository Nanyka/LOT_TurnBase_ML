using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "CollectableStats", menuName = "JumpeeIsland/CollectableStats", order = 7)]
    public class CollectableStats : ScriptableObject
    {
        public bool IsLongLasting;
        [ShowIf("@IsLongLasting == false")] public int MaxTurnToDestroy;
        public CollectableType CollectableType;
        
        [Header("Currency rewards")]
        public List<CommandName> Commands;

        [FormerlySerializedAs("EntityType")] [Header("Entity rewards")] 
        public EntityType SpawnedEntityType;
        public string EntityName;
    }
}