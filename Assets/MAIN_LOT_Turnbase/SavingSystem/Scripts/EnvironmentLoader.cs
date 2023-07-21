using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class EnvironmentLoader : MonoBehaviour
    {
        [SerializeField] private TileManager tileManager;
        [SerializeField] private ResourceLoader resourceLoader;
        [SerializeField] private BuildingLoader buildingLoader;
        [SerializeField] private CreatureLoader playerLoader;
        [SerializeField] private CreatureLoader enemyLoader;
        [FormerlySerializedAs("_collectableLoader")] [SerializeField] private CollectableObjectLoader collectableLoader;
        [SerializeField] protected EnvironmentData _environmentData;

        public virtual void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");
            ExecuteEnvData();
        }

        public void ResetData()
        {
            Debug.Log("Remove all environment to reset...");
            tileManager.Reset();
            resourceLoader.Reset();
            playerLoader.Reset();
            enemyLoader.Reset();
            collectableLoader.Reset();
            buildingLoader.Reset();
        }

        protected void ExecuteEnvData()
        {
            resourceLoader.StartUpLoadData(_environmentData.ResourceData);
            buildingLoader.StartUpLoadData(_environmentData.BuildingData);
            playerLoader.StartUpLoadData(_environmentData.PlayerData);
            enemyLoader.StartUpLoadData(_environmentData.EnemyData);
            collectableLoader.StartUpLoadData(_environmentData.CollectableData);

            tileManager.Init(_environmentData.mapSize);
        }

        public EnvironmentData GetData()
        {
            return _environmentData;
        }

        // Use a distinct function to get data for saving and override it in BattleEnvLoader
        public virtual EnvironmentData GetDataForSave()
        {
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
            removeInterface.Remove(_environmentData);
        }

        #region TILES

        public List<Transform> GetTiles()
        {
            return tileManager.GetTiles();
        }

        #endregion

        #region RESOURCE

        public void SpawnResource(ResourceData resourceData)
        {
            _environmentData.AddResourceData(resourceData);
            resourceLoader.PlaceNewObject(resourceData);
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

        public void PlaceABuilding(BuildingData buildingData)
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
            buildingLoader.GetController().DeductCurrencyFromBuildings(currencyId,amount);
        }

        #endregion

        #region CREATURES

        public void TrainACreature(CreatureData creatureData)
        {
            _environmentData.AddPlayerData(creatureData);
            playerLoader.PlaceNewObject(creatureData);
        }

        #region ENEMY

        public void SpawnAnEnemy(CreatureData creatureData)
        {
            _environmentData.AddEnemyData(creatureData);
            enemyLoader.PlaceNewObject(creatureData);
        }

        #endregion

        #endregion
    }
}