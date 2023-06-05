using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class EnvironmentLoader : MonoBehaviour
    {
        [SerializeField] private TileManager _tileManager;
        [SerializeField] private ResourceManager _resourceManager;
        [SerializeField] private BuildingManager _buildingManager;
        [SerializeField] private CreatureManager _playerManager;
        [SerializeField] private CreatureManager _enemyManager;

        [Header("Test data")] 
        [SerializeField] private int _mapSize;
        [SerializeField] private EnvironmentData _environmentData;
        
        // private void Awake()
        // {
        //     StartUpProcessor.Instance.OnResetData.AddListener(ResetData);
        // }

        public void Init()
        {
            Debug.Log("Load data into managers...");
            ExecuteEnvData();
        }

        public void ResetData()
        {
            Debug.Log("Remove all environment to reset...");
            _tileManager.Reset();
            _resourceManager.Reset();
            _playerManager.Reset();
            _enemyManager.Reset();
            _buildingManager.Reset();
        }

        private void ExecuteEnvData()
        {
            _resourceManager.StartUpLoadData(_environmentData._testResourceData);
            _buildingManager.StartUpLoadData(_environmentData._testBuildingData);
            _playerManager.StartUpLoadData(_environmentData._testPlayerData);
            _enemyManager.StartUpLoadData(_environmentData._testEnemyData);

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
    }
}
