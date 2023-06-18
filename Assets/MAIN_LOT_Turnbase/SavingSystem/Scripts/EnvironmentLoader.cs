using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class EnvironmentLoader : MonoBehaviour
    {
        [SerializeField] private TileManager tileManager;
        [SerializeField] private ResourceLoader resourceLoader;
        [SerializeField] private BuildingLoader buildingLoader;
        [SerializeField] private CreatureLoader playerLoader;
        [SerializeField] private CreatureLoader enemyLoader;
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
            buildingLoader.Reset();
        }

        protected void ExecuteEnvData()
        {
            resourceLoader.StartUpLoadData(_environmentData.ResourceData);
            buildingLoader.StartUpLoadData(_environmentData.BuildingData);
            playerLoader.StartUpLoadData(_environmentData.PlayerData);
            enemyLoader.StartUpLoadData(_environmentData.EnemyData);

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

        #region RESOURCE

        public void SpawnResource(ResourceData resourceData)
        {
            _environmentData.AddResourceData(resourceData);
            resourceLoader.PlaceNewObject(resourceData);
        }

        #endregion

        #region BUILDINGS

        public void PlaceABuilding(BuildingData buildingData)
        {
            _environmentData.AddBuildingData(buildingData);
            buildingLoader.PlaceNewObject(buildingData);
        }

        public void StoreRewardToBuildings(string currencyId, int amount, Vector3 fromPos)
        {
            buildingLoader.GetController().StoreRewardToBuildings(currencyId, amount, fromPos);
        }

        #endregion

        #region CREATURES

        public void TrainACreature(CreatureData creatureData)
        {
            _environmentData.AddPlayerData(creatureData);
            playerLoader.PlaceNewObject(creatureData);
        }

        #endregion
    }
}