using Sirenix.OdinInspector;
using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class TutorialStep
    {
        public bool Pointer;
        [ShowIf("@Pointer == true")] public bool EntitySelection;
        [ShowIf("@EntitySelection == true")] public EntityType EntityType;
        [ShowIf("EntityType", EntityType.RESOURCE)] public CurrencyType CurrencyFromResource;
        [ShowIf("EntityType", EntityType.BUILDING)] public BuildingType BuildingType;
        [ShowIf("EntityType", EntityType.ENEMY)] public CreatureType EnemyType;
        [ShowIf("EntityType", EntityType.PLAYER)] public bool ArrowSign;
        [ShowIf("@EntitySelection == true && ArrowSign == true")] public int MinJump;
        [ShowIf("@EntitySelection == false && Pointer == true")] public bool ButtonSelection;
        [ShowIf("@ButtonSelection == true")] public string ButtonName;
        [ShowIf("@EntitySelection == true")] public bool CheckPosition;
        [ShowIf("@EntitySelection == true && ArrowSign == false && CheckPosition == false")] public bool CheckEntity;
        public bool Spawner;
        [ShowIf("@Spawner == true")] public EntityType SpawnType;
        [ShowIf("@Spawner == true")] public bool IsDesignatedPos;
        [ShowIf("@Spawner == true && IsDesignatedPos == true")] public Vector3 DesignatedPos;
        [VerticalGroup("ResourceSpawn",VisibleIf = "@SpawnType == JumpeeIsland.EntityType.RESOURCE")]
        [VerticalGroup("ResourceSpawn/Row1")]
        public string SpawnResource;
        [VerticalGroup("CollectableSpawn",VisibleIf = "@SpawnType == JumpeeIsland.EntityType.COLLECTABLE")]
        [VerticalGroup("CollectableSpawn/Row1")]
        public string SpawnCollectable;
        [VerticalGroup("CollectableSpawn/Row2")]
        public int SpawnCollectableLevel;
        public bool CheckEndCondition;
        [ShowIf("@CheckEndCondition == true")] public GameMasterCondition EndCondition;
        public bool Conversation;
        [ShowIf("@Conversation == true")] public string Message;
    }
}