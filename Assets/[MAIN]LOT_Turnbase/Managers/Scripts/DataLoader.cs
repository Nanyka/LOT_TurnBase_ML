using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class DataLoader : MonoBehaviour
    {
        [SerializeField] private TileManager _tileManager;
        [SerializeField] private ResourceManager _resourceManager;
        [SerializeField] private BuildingManager _buildingManager;
        [SerializeField] private CreatureManager _playerManager;
        [SerializeField] private CreatureManager _enemyManager;

        [Header("Test data")]
        [SerializeField] private List<ResourceData> _testResourceData;
        [SerializeField] private List<BuildingData> _testBuildingData;
        [SerializeField] private List<CreatureData> _testPlayerData;
        [SerializeField] private List<CreatureData> _testEnemyData;
        
        private void Start()
        {
            StartUpProcessor.Instance.OnLoadData.AddListener(StartUpLoadData);
        }

        private void StartUpLoadData()
        {
            Debug.Log("Load data into managers");
            // Load resources
            // List<ResourceData> testResources = new List<ResourceData> {_testResourceData};
            // Load buildings
            // List<BuildingData> testBuildings = new List<BuildingData> {_testBuildingData};
            // Load creatures
            // List<CreatureData> testCreatures = new List<CreatureData> {_testCreatureData};

            _resourceManager.StartUpLoadData(_testResourceData);
            _buildingManager.StartUpLoadData(_testBuildingData);
            _playerManager.StartUpLoadData(_testPlayerData);
            _enemyManager.StartUpLoadData(_testEnemyData);
            
            _tileManager.Init(7);
        }
    }
}
