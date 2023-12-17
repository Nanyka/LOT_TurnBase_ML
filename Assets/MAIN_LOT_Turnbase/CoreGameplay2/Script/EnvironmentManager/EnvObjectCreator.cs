using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class EnvObjectCreator : MonoBehaviour, IEnvironmentCreator
    {
        [SerializeField] private List<Vector3> _stones;
        [SerializeField] private List<Vector3> _enemies;

        public void CreateEnvObjects()
        {
            foreach (var stone in _stones)
            {
                SavingSystemManager.Instance.OnSpawnResource("Tree1", stone);
            }

            foreach (var enemyPos in _enemies)
            {
                var enemyObj = SavingSystemManager.Instance.OnSpawnMovableEntity("Boss0", enemyPos);

                if (enemyObj.TryGetComponent(out CharacterEntity characterEntity))
                {
                    var creatureData = new CreatureData()
                    {
                        EntityName = "Boss0",
                        Position = enemyPos,
                        FactionType = FactionType.Enemy,
                        CurrentLevel = 0
                    };
                    
                    characterEntity.Init(creatureData);
                }
                
                if (enemyObj.TryGetComponent(out ITroopAssembly character))
                    character.SetAssemblyPoint(enemyPos);
            }
        }
    }

    public interface IEnvironmentCreator
    {
        public void CreateEnvObjects();
    }
}