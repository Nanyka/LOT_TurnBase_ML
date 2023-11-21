using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeEnvironmentLoader : EnvironmentLoader
    {
        [SerializeField] private BuildingLoader _playerBuildingLoader;
        [SerializeField] protected EnvironmentData _playerEnvCache;

        public override async void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");
            tileManager.Init(_environmentData.mapSize);

            // Save playerEnv into the cache that will be used for saving at the end of battle
            _playerEnvCache = _environmentData;
            
            // Load EnemyEnv
            _environmentData = await SavingSystemManager.Instance.GetEnemyEnv();
            
            // Customize battle env from enemy env and player env
            _environmentData.PrepareForBattleMode(_playerEnvCache.PlayerData);
            
            // Update currency UI
            MainUI.Instance.OnUpdateCurrencies.Invoke();

            ExecuteEnvData();
            Debug.Log("----GAME START!!!----");
            // SavingSystemManager.Instance.OnAskForShowingCreatureMenu();
        }
        
        public override EnvironmentData GetData()
        {
            return _environmentData;
        }

        public override EnvironmentData GetDataForSave()
        {
            _playerEnvCache.AbstractInBattleCreatures(_environmentData.PlayerData);
            _playerEnvCache.RemoveZeroHpPlayerCreatures();
            return _playerEnvCache;
        }

        public List<CreatureData> GetSpawnList()
        {
            return _playerEnvCache.PlayerData;
        }
        
        protected override void ExecuteEnvData()
        {
            buildingLoader.StartUpLoadData(_environmentData.BuildingData);
            GameFlowManager.Instance.OnInitiateObjects.Invoke();

            // resourceLoader.StartUpLoadData(_environmentData.ResourceData);
            // buildingLoader.StartUpLoadData(_environmentData.BuildingData);
            // playerLoader.StartUpLoadData(_environmentData.PlayerData);
            // enemyLoader.StartUpLoadData(_environmentData.EnemyData);
            // collectableLoader.StartUpLoadData(_environmentData.CollectableData);
            
            // GameFlowManager.Instance.OnInitiateObjects.Invoke();
        }
        
        public override void PlaceABuilding(BuildingData buildingData)
        {
            Debug.Log($"Place {buildingData.EntityName}");

            _environmentData.AddBuildingData(buildingData);
            
            if (buildingData.FactionType == FactionType.Enemy)
                buildingLoader.PlaceNewObject(buildingData);
            else
                _playerBuildingLoader.PlaceNewObject(buildingData);
        }
    }
}