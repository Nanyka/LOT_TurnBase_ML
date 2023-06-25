using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public enum CollectableType
    {
        NONE,
        REWARD,
        TRAP
    }

    [System.Serializable]
    public class CollectableData: EntityData
    {
        [FormerlySerializedAs("ResourceType")] public CollectableType CollectableType;
        public int AccumulatedStep;
    }
}