using Sirenix.OdinInspector;

namespace JumpeeIsland
{
    [System.Serializable]
    public class TutorialStep
    {
        public bool Pointer;
        [ShowIf("@Pointer == true")] public bool ArrowSign;
        [ShowIf("@Pointer == true && ArrowSign == true")] public int Direction;
        [ShowIf("@Pointer == true && ArrowSign == false")] public bool EntitySelection;
        [ShowIf("@EntitySelection == true")] public EntityType EntityType;
        [ShowIf("EntityType", EntityType.RESOURCE)] public ResourceType ResourceType;
        [ShowIf("EntityType", EntityType.BUILDING)] public BuildingType BuildingType;
        [ShowIf("EntityType", EntityType.PLAYER)] public CreatureType PlayerType;
        [ShowIf("EntityType", EntityType.ENEMY)] public CreatureType EnemyType;
        [ShowIf("@EntitySelection == false && Pointer == true")] public bool UISelection;
        [ShowIf("@UISelection == true")] public string UIName;
        public bool LockAction;
        public bool Conversation;
        [ShowIf("@Conversation == true")] public string Message;
    }
}