using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LOT_Turnbase
{
    [CreateAssetMenu(fileName = "ResourceStats", menuName = "TurnBase/ResourceStats", order = 3)]
    public class ResourceStats : ScriptableObject
    {
        public int MaxHealth;
        public bool IsLongLasting;
        [ShowIf("@IsLongLasting == false")] public int MaxTurnToDestroy;
        public ResourceType ResourceType;
        [ShowIf("@ResourceType == LOT_Turnbase.ResourceType.Reward")]
        public CurrencyAndAmount[] Reward =
        {
            new(CurrencyType.Food, 0), new(CurrencyType.Wood, 0), new(CurrencyType.Gold, 0), 
            new(CurrencyType.Diamond, 0), new(CurrencyType.Step, 0)
        };
    }
}