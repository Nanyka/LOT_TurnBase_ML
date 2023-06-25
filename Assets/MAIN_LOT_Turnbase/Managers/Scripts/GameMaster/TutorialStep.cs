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
        [ShowIf("EntityType", EntityType.RESOURCE)] public ResourceType ResourceType;
        [ShowIf("EntityType", EntityType.BUILDING)] public BuildingType BuildingType;
        [ShowIf("EntityType", EntityType.ENEMY)] public CreatureType EnemyType;
        [ShowIf("EntityType", EntityType.PLAYER)] public bool ArrowSign;
        [ShowIf("@EntitySelection == true && ArrowSign == true")] public int MinJump;
        [ShowIf("@EntitySelection == false && Pointer == true")] public bool UISelection;
        [ShowIf("@UISelection == true")] public string UIName;
        [ShowIf("@EntitySelection == true")] public bool CheckPosition;
        [ShowIf("@EntitySelection == true && ArrowSign == false && CheckPosition == false")] public bool CheckEntity;
        public bool Spawner;
        [ShowIf("@Spawner == true")] public EntityType SpawnType;
        [ShowIf("@Spawner == true")] public bool IsDesignatedPos;
        [ShowIf("@Spawner == true && IsDesignatedPos == true")] public Vector3 DesignatedPos;
        [HorizontalGroup("ResourceSpawn",VisibleIf = "@SpawnType == JumpeeIsland.EntityType.RESOURCE")]
        [HorizontalGroup("ResourceSpawn/Col1")]
        public string SpawnResource;
        [HorizontalGroup("CollectableSpawn",VisibleIf = "@SpawnType == JumpeeIsland.EntityType.COLLECTABLE")]
        [HorizontalGroup("CollectableSpawn/Col1")]
        public string SpawnCollectable;
        [HorizontalGroup("CollectableSpawn/Col2")]
        public int SpawnCollectableLevel;
        public bool Conversation;
        [ShowIf("@Conversation == true")] public string Message;
    }
}