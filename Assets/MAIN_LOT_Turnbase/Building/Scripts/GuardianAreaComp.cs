using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class GuardianAreaComp : MonoBehaviour, IGuardianArea
    {
        [SerializeField] private float _spawnArea = 1f;
        
        public void SpawnGuardians(List<Guardian> guardians)
        {
            switch (GameFlowManager.Instance.GameMode)
            {
                case GameMode.ECONOMY:
                {
                    foreach (var guardian in guardians)
                    {
                        CreatureData guardianData = new CreatureData
                        {
                            EntityName = guardian.EntityName,
                            CurrentLevel = guardian.Level,
                            Position = transform.position + GenerateRandomVector()
                        };

                        var troop = SavingSystemManager.Instance.OnTrainACreature(guardianData);
                    }
                    
                    break;
                }

                case GameMode.AOE:
                {
                    foreach (var guardian in guardians)
                    {
                        var enemyPos = transform.position + GenerateRandomVector();
                        var enemyObj = SavingSystemManager.Instance.OnSpawnMovableEntity(guardian.EntityName, enemyPos);
            
                        if (enemyObj.TryGetComponent(out CharacterEntity characterEntity))
                        {
                            var creatureData = new CreatureData()
                            {
                                EntityName = guardian.EntityName,
                                Position = enemyPos,
                                FactionType = FactionType.Enemy,
                                CurrentLevel = guardian.Level
                            };
                
                            characterEntity.Init(creatureData);
                        }
            
                        if (enemyObj.TryGetComponent(out ITroopAssembly character))
                            character.SetAssemblyPoint(enemyPos);
                    }
                    
                    break;
                }
            }
        }

        Vector3 GenerateRandomVector()
        {
            float randomX = Random.Range(-_spawnArea, _spawnArea); // Adjust the range based on your requirements
            float randomZ = Random.Range(-_spawnArea, _spawnArea); // Adjust the range based on your requirements

            // Set the y value to 0
            Vector3 randomVector = new Vector3(randomX, 0f, randomZ);

            return randomVector;
        }
    }

    public interface IGuardianArea
    {
        public void SpawnGuardians(List<Guardian> guardians);
    }
}