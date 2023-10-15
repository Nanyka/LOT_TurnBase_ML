using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [System.Serializable]
    public class RuntimeMetadata
    {
        public bool IsInLoadingPhase;
        [FormerlySerializedAs("RecordInfo")] public BattleRecord BattleRecord;
    }
}