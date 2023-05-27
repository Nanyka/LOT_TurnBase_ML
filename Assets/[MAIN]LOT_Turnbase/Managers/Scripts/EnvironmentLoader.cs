using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class EnvironmentLoader : MonoBehaviour
    {
        [SerializeField] private TileManager _tileManager;
        [SerializeField] private ResourceManager _resourceManager;
        [SerializeField] private BuildingManager _buildingManager;
        [SerializeField] private CreatureManager _playerManager;
        [SerializeField] private CreatureManager _enemyManager;

        [Header("Test data")] 
        [SerializeField] private EnvironmentData _environmentData;
        
        private void Start()
        {
            StartUpProcessor.Instance.OnLoadData.AddListener(StartUpLoadData);
        }

        private void StartUpLoadData()
        {
            Debug.Log("Load data into managers");
            _environmentData = SavingSystemManager.Instance.LoadEnvironment();

            _resourceManager.StartUpLoadData(_environmentData._testResourceData);
            _buildingManager.StartUpLoadData(_environmentData._testBuildingData);
            _playerManager.StartUpLoadData(_environmentData._testPlayerData);
            _enemyManager.StartUpLoadData(_environmentData._testEnemyData);
            
            _tileManager.Init(7);
        }

        public EnvironmentData GetData()
        {
            return _environmentData;
        }
    }
}
