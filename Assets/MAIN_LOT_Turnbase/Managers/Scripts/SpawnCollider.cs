using Sirenix.OdinInspector;
using UnityEngine;

namespace JumpeeIsland
{
    public class SpawnCollider : MonoBehaviour
    {
        [Header("Entity rewards")] 
        public EntityType SpawnedEntityType;
        [ShowIf("@SpawnedEntityType != EntityType.NONE")] public string EntityName;
        
        private void OnParticleCollision(GameObject other)
        {
            switch (SpawnedEntityType)
            {
                case EntityType.BUILDING:
                    SavingSystemManager.Instance.OnPlaceABuilding(EntityName, other.transform.position,
                        true);
                    break;
                case EntityType.ENEMY:
                    SavingSystemManager.Instance.OnSpawnMovableEntity(EntityName, other.transform.position);
                    break;
                case EntityType.RESOURCE:
                    SavingSystemManager.Instance.OnSpawnResource(EntityName, other.transform.position);
                    break;
                case EntityType.COLLECTABLE:
                    SavingSystemManager.Instance.OnSpawnCollectable(EntityName, other.transform.position,0);
                    break;
            }
        }
    }
}