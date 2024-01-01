using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class EnvironmentLoad : MonoBehaviour, IEnvironmentLoad, IResourceStock
    {
        [SerializeField] protected TileManager tileManager;
        [SerializeField] private ResourceLoader resourceLoader;
        [SerializeField] protected BuildingLoader buildingLoader;
        [SerializeField] private CreatureLoader playerLoader;
        [SerializeField] private CreatureLoader enemyLoader;
        [SerializeField] private CollectableObjectLoader collectableLoader;
        [SerializeField] protected EnvironmentData _environmentData;

        public virtual void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");
            tileManager.Init(_environmentData.mapSize);
            ExecuteEnvData();
        }

        public void ResetData()
        {
            Debug.Log("Remove all environment to reset...");
            tileManager.Reset();
            resourceLoader.Reset();
            buildingLoader.Reset();
            playerLoader.Reset();
            enemyLoader.Reset();
            collectableLoader.Reset();
        }

        protected void ExecuteEnvData()
        {
            resourceLoader.StartUpLoadData(_environmentData.ResourceData);
            buildingLoader.StartUpLoadData(_environmentData.BuildingData);
            playerLoader.StartUpLoadData(_environmentData.PlayerData);
            enemyLoader.StartUpLoadData(_environmentData.EnemyData);
            collectableLoader.StartUpLoadData(_environmentData.CollectableData);
            SavingSystemManager.Instance.OnSyncEnvData();
            GameFlowManager.Instance.OnInitiateObjects.Invoke();
        }

        public virtual EnvironmentData GetData()
        {
            return _environmentData;
        }

        // Use a distinct function to get data for saving and override it in BattleEnvLoader
        public virtual EnvironmentData GetDataForSave()
        {
            _environmentData.RemoveZeroHpPlayerCreatures();
            return _environmentData;
        }

        public void SetData(EnvironmentData environmentData)
        {
            if (environmentData == null)
                return;
            _environmentData = environmentData;
        }

        protected void RemoveDestroyedEntity(IRemoveEntity removeInterface)
        {
            var entityData = removeInterface.GetEntityData();

            switch (entityData.EntityType)
            {
                case EntityType.BUILDING:
                {
                    GetData().BuildingData.Remove(entityData as BuildingData);
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
            
            // removeInterface.Remove(this);
        }

        #region RESOURCE

        public void SpawnResource(ResourceData resourceData)
        {
            _environmentData.AddResourceData(resourceData);
            resourceLoader.PlaceNewObject(resourceData);
        }

        public IEnumerable<GameObject> GetResources()
        {
            return resourceLoader.GetResources();
        }

        public EnvironmentLoader GetSpecificEnvLoader<EnvironmentLoader>()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region COLLECTABLE

        public void SpawnCollectable(CollectableData collectableData)
        {
            _environmentData.AddCollectableData(collectableData);
            collectableLoader.PlaceNewObject(collectableData);
        }

        #endregion

        #region BUILDINGS

        public virtual void PlaceABuilding(BuildingData buildingData)
        {
            _environmentData.AddBuildingData(buildingData);
            buildingLoader.PlaceNewObject(buildingData);
        }

        public void StoreRewardAtBuildings(string currencyId, int amount)
        {
            buildingLoader.GetController().StoreRewardAtBuildings(currencyId, amount);
        }

        public void DeductCurrencyFromBuildings(string currencyId, int amount)
        {
            buildingLoader.GetController().DeductCurrencyFromBuildings(currencyId, amount);
        }

        public virtual IEnumerable<GameObject> GetBuildings(FactionType faction)
        {
            return buildingLoader.GetBuildings();
        }

        #endregion

        #region PLAYER

        public GameObject TrainACreature(CreatureData creatureData)
        {
            _environmentData.AddPlayerData(creatureData);
            return playerLoader.PlaceNewObject(creatureData);
        }

        #endregion

        #region ENEMY

        public GameObject SpawnAnEnemy(CreatureData creatureData)
        {
            _environmentData.AddEnemyData(creatureData);
            return enemyLoader.PlaceNewObject(creatureData);
        }

        #endregion
    }
    
    public interface IEnvironmentLoad
    {
        public void Init();
        public void SetData(EnvironmentData environmentData);
        public EnvironmentData GetData();
        public EnvironmentData GetDataForSave();
        public void ResetData();
        public void SpawnResource(ResourceData resourceData);
        public void SpawnCollectable(CollectableData collectableData);
        public void PlaceABuilding(BuildingData buildingData);
        public GameObject TrainACreature(CreatureData creatureData);
        public GameObject SpawnAnEnemy(CreatureData creatureData);
        public IEnumerable<GameObject> GetBuildings(FactionType faction);
        public IEnumerable<GameObject> GetResources();
    }

    public interface IResourceStock
    {
        public void StoreRewardAtBuildings(string currencyId, int amount);
        public void DeductCurrencyFromBuildings(string currencyId, int amount);
    }
}