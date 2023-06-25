using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class ResourceData: EntityData
    {
        public int AccumulatedStep;
        public int Level;
        public ResourceType ResourceType;
        public CurrencyType CollectedCurrency;
    }
}