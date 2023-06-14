using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "ResourceStats", menuName = "TurnBase/ResourceStats", order = 3)]
    public class ResourceStats : ScriptableObject
    {
        public int MaxHp;
        public bool IsLongLasting;
        [ShowIf("@IsLongLasting == false")] public int MaxTurnToDestroy;
        public CurrencyType CollectedCurrency;
        public CommandName Command;
        [Tooltip("Amount of exp that entity destroying this resource can collect")]
        public int ExpReward;

        // public ResourceType ResourceType;
        // [ShowIf("@ResourceType == JumpeeIsland.ResourceType.Reward")]
        // public CurrencyAndAmount[] Reward =
        // {
        //     new(CurrencyType.Food, 0), new(CurrencyType.Wood, 0), new(CurrencyType.Gold, 0), 
        //     new(CurrencyType.Diamond, 0), new(CurrencyType.Step, 0)
        // };
    }
}