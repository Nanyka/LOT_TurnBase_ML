using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "CollectableStats", menuName = "JumpeeIsland/CollectableStats", order = 7)]
    public class CollectableStats : ScriptableObject
    {
        public CollectableType CollectableType;
        public bool IsLongLasting;
        [ShowIf("@IsLongLasting == false")] public int MaxTurnToDestroy;
        public List<CommandName> Commands;
    }
}