using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeBossEnvLoad : AoeEnvironmentLoad, IMonsterControl
    {
        [SerializeField] private EnvironmentData _bossEnvData;

        private GameObject _bossObject;

        public override void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");

            // Save playerEnv into the cache that will be used for saving at the end of battle
            _playerEnvCache = _environmentData;

            // Load EnemyEnv
            _environmentData = _bossEnvData;

            // Customize battle env from enemy env and player env
            _environmentData.PrepareForBattleMode(_playerEnvCache.PlayerData);

            // Update currency UI
            MainUI.Instance.OnUpdateCurrencies.Invoke();

            ExecuteEnvData();
            // Create environment-relevant objects that not include in the saving data
            GetComponent<IEnvironmentCreator>().CreateEnvObjects();

            GameFlowManager.Instance.OnKickOffEnv.Invoke();
            Debug.Log("----GAME START!!!----");
        }

        public override GameObject SpawnAnEnemy(CreatureData creatureData)
        {
            _environmentData.AddEnemyData(creatureData);
            var spawnedTroop = monsterLoader.PlaceNewObject(creatureData);

            return spawnedTroop;
        }

        protected override void RemoveDestroyedEntity(IRemoveEntity removeInterface)
        {
            var entityData = removeInterface.GetEntityData();

            switch (entityData.EntityType)
            {
                case EntityType.BUILDING:
                {
                    GetData().BuildingData.Remove(entityData as BuildingData);
                    if (removeInterface.GetRemovedObject().TryGetComponent(out IStoreResource storeResource))
                        _resourceStorages.Remove(storeResource);

                    break;
                }
                case EntityType.PLAYER:
                {
                    GetData().PlayerData.Remove(entityData as CreatureData);
                    break;
                }
                case EntityType.ENEMY:
                {
                    {
                        var creatureData = entityData as CreatureData;
                        GetData().EnemyData.Remove(creatureData);
                        if (creatureData != null && creatureData.CreatureType == CreatureType.ECOBOSS)
                            MainUI.Instance.OnUpdateResult.Invoke();
                    }
                    break;
                }
                case EntityType.RESOURCE:
                {
                    GetData().ResourceData.Remove(entityData as ResourceData);
                    break;
                }
                case EntityType.COLLECTABLE:
                {
                    GetData().CollectableData.Remove(entityData as CollectableData);
                    break;
                }
            }
        }

        public void RegisterBossObject(GameObject bossObject)
        {
            _bossObject = bossObject;
        }

        public IEnumerable<GameObject> GetMonsters()
        {
            List<GameObject> returnList = new List<GameObject>();
            foreach (var building in enemyBuildingLoader.GetBuildings())
                returnList.Add(building);

            returnList.Add(_bossObject);

            return returnList;
        }
    }

    public interface IMonsterControl
    {
        public void RegisterBossObject(GameObject bossObject);
        public IEnumerable<GameObject> GetMonsters();
    }
}