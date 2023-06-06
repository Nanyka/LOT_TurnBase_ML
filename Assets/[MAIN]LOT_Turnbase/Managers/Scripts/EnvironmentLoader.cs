using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class EnvironmentLoader : MonoBehaviour
    {
        [SerializeField] private TileManager _tileManager;
        [SerializeField] private ResourceLoader resourceLoader;
        [SerializeField] private BuildingLoader buildingLoader;
        [SerializeField] private CreatureLoader playerLoader;
        [SerializeField] private CreatureLoader enemyLoader;

        [Header("Test data")] 
        [SerializeField] private int _mapSize;
        [SerializeField] private EnvironmentData _environmentData;
        
        // private void Awake()
        // {
        //     StartUpProcessor.Instance.OnResetData.AddListener(ResetData);
        // }

        public void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");
            ExecuteEnvData();
        }

        public void ResetData()
        {
            Debug.Log("Remove all environment to reset...");
            _tileManager.Reset();
            resourceLoader.Reset();
            playerLoader.Reset();
            enemyLoader.Reset();
            buildingLoader.Reset();
        }

        private void ExecuteEnvData()
        {
            resourceLoader.StartUpLoadData(_environmentData._testResourceData);
            buildingLoader.StartUpLoadData(_environmentData._testBuildingData);
            playerLoader.StartUpLoadData(_environmentData._testPlayerData);
            enemyLoader.StartUpLoadData(_environmentData._testEnemyData);

            _tileManager.Init(_mapSize);
        }

        public EnvironmentData GetData()
        {
            return _environmentData;
        }

        public void SetData(EnvironmentData environmentData)
        {
            _environmentData = environmentData;
        }
        
        private void RemoveDestroyedEntity(IRemoveEntity removeInterface)
        {
            removeInterface.Remove(_environmentData);
        }
    }
}
