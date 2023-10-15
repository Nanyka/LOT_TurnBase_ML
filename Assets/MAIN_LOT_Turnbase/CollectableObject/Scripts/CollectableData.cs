using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public enum CollectableType
    {
        NONE,
        REWARD,
        TRAP,
        TARGET
    }

    [System.Serializable]
    public class CollectableData: EntityData
    {
        public CollectableType CollectableType;
        public int AccumulatedStep;

        public CollectableData() { }

        public CollectableData(CollectableData collectableData)
        {
            EntityName = collectableData.EntityName;
            SkinAddress = collectableData.SkinAddress;
            Position = collectableData.Position;
            Rotation = collectableData.Rotation;
            FactionType = collectableData.FactionType;
            CollectableType = collectableData.CollectableType;
            AccumulatedStep = collectableData.AccumulatedStep;
        }
    }
}