using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "ResourceStats", menuName = "JumpeeIsland/ResourceStats", order = 3)]
    public class ResourceStats : ScriptableObject
    {
        public int MaxHp;
        public bool IsLongLasting;
        [ShowIf("@IsLongLasting == false")] public int MaxTurnToDestroy;
        public CurrencyType CollectedCurrency;
        public List<CommandName> Commands;
        [Tooltip("Amount of exp that entity destroying this resource can collect")]
        public int ExpReward;
    }
}