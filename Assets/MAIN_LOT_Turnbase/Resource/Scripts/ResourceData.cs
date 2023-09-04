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

        public ResourceData() { }
        
        public ResourceData(ResourceData resourceData)
        {
            EntityName = resourceData.EntityName;
            SkinAddress = resourceData.SkinAddress;
            Position = resourceData.Position;
            Rotation = resourceData.Rotation;
            FactionType = resourceData.FactionType;
            AccumulatedStep = resourceData.AccumulatedStep;
            Level = resourceData.Level;
            ResourceType = resourceData.ResourceType;
            CollectedCurrency = resourceData.CollectedCurrency;
        }
    }
}