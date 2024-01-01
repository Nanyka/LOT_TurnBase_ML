using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class AoeEnvironmentLoad : MonoBehaviour, IEnvironmentLoad, IStoragesControl
    {
        [SerializeField] private GameObject resources;
        [SerializeField] protected GameObject playerBuildings;
        [SerializeField] private GameObject enemyBuildings;
        [SerializeField] private GameObject playerCreatures;
        [SerializeField] private GameObject neutralCreatures;
        [SerializeField] private GameObject collectable;

        [SerializeField] protected EnvironmentData _environmentData;
        [SerializeField] protected EnvironmentData _playerEnvCache;
        protected List<IStoreResource> _resourceStorages = new();
        private IBuildingLoader playerBuildingLoader;
        protected IBuildingLoader enemyBuildingLoader;
        private IResourceLoader resourceLoader;
        private ICollectableLoader collectableLoader;
        private ICreatureLoader playerLoader;
        protected ICreatureLoader monsterLoader;

        #region ENVIRONMENT LOADER

        private void Start()
        {
            playerBuildingLoader = playerBuildings.GetComponent<IBuildingLoader>();
            enemyBuildingLoader = enemyBuildings.GetComponent<IBuildingLoader>();
            resourceLoader = resources.GetComponent<IResourceLoader>();
            collectableLoader = collectable.GetComponent<ICollectableLoader>();
            playerLoader = playerCreatures.GetComponent<ICreatureLoader>();
            monsterLoader = neutralCreatures.GetComponent<ICreatureLoader>();
        }

        public virtual async void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");

            // Save playerEnv into the cache that will be used for saving at the end of battle
            _playerEnvCache = _environmentData;

            // Load EnemyEnv
            _environmentData = await SavingSystemManager.Instance.GetEnemyEnv();

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

        protected void ExecuteEnvData()
        {
            enemyBuildingLoader.StartUpLoadData(_environmentData.BuildingData);
            GameFlowManager.Instance.OnInitiateObjects.Invoke();
        }

        protected virtual void RemoveDestroyedEntity(IRemoveEntity removeInterface)
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
                    GetData().EnemyData.Remove(entityData as CreatureData);
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

            if (entityData.FactionType == FactionType.Enemy)
                MainUI.Instance.OnUpdateResult.Invoke();
        }

        public void SetData(EnvironmentData environmentData)
        {
            if (environmentData == null)
                return;
            _environmentData = environmentData;
        }

        public EnvironmentData GetData()
        {
            return _environmentData;
        }

        public EnvironmentData GetDataForSave()
        {
            // _playerEnvCache.AbstractInBattleCreatures(_environmentData.PlayerData);
            // _playerEnvCache.RemoveZeroHpPlayerCreatures();
            return _playerEnvCache;
        }

        public void ResetData()
        {
            Debug.Log("Remove all environment to reset...");
            // tileManager.Reset();
            resourceLoader.Reset();
            playerBuildingLoader.Reset();
            playerLoader.Reset();
            enemyBuildingLoader.Reset();
            collectableLoader.Reset();
        }

        public void SpawnResource(ResourceData resourceData)
        {
            _environmentData.AddResourceData(resourceData);
            resourceLoader.PlaceNewObject(resourceData);
        }

        public void SpawnCollectable(CollectableData collectableData)
        {
            _environmentData.AddCollectableData(collectableData);
            collectableLoader.PlaceNewObject(collectableData);
        }

        public void PlaceABuilding(BuildingData buildingData)
        {
            _environmentData.AddBuildingData(buildingData);
            if (buildingData.FactionType == FactionType.Enemy)
                enemyBuildingLoader.PlaceNewObject(buildingData);
            else
            {
                var storage = playerBuildingLoader.PlaceNewObject(buildingData);
                if (storage.TryGetComponent(out IStoreResource storeResource))
                    _resourceStorages.Add(storeResource);
            }
        }

        public GameObject TrainACreature(CreatureData creatureData)
        {
            _environmentData.AddPlayerData(creatureData);
            return playerLoader.PlaceNewObject(creatureData);
        }

        public virtual GameObject SpawnAnEnemy(CreatureData creatureData)
        {
            _environmentData.AddEnemyData(creatureData);
            var spawnedTroop = monsterLoader.PlaceNewObject(creatureData);

            return spawnedTroop;
        }

        public IEnumerable<GameObject> GetBuildings(FactionType faction)
        {
            if (faction == FactionType.Player)
                return playerBuildingLoader.GetBuildings();

            return enemyBuildingLoader.GetBuildings();
        }

        public IEnumerable<GameObject> GetResources()
        {
            return resourceLoader.GetResources();
        }

        #endregion

        public IStoreResource GetRandomStorage()
        {
            var availableStorages = _resourceStorages.FindAll(t => t.IsFullStock() == false);

            if (availableStorages.Count == 0)
                return null;

            float randomValue = Random.Range(0f, availableStorages.Sum(t => t.GetWeight()));

            if (randomValue <= Mathf.Epsilon)
                return availableStorages[Random.Range(0, availableStorages.Count)];

            float cumulativeWeight = 0f;

            foreach (var storage in availableStorages)
            {
                cumulativeWeight += storage.GetWeight();

                if (randomValue <= cumulativeWeight)
                {
                    return storage;
                }
            }

            return null;
        }

        public IEnumerable<IStoreResource> GetStorages()
        {
            return _resourceStorages;
        }
    }

    public interface IStoragesControl
    {
        public IStoreResource GetRandomStorage();
        public IEnumerable<IStoreResource> GetStorages();
    }
}