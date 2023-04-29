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
        [SerializeField] private UnitManager _unitManager;

        [Header("Test data")]
        [SerializeField] private ResourceData _testResourceData;
        [SerializeField] private BuildingData _testBuildingData;
        
        private void Start()
        {
            StartUpProcessor.Instance.OnLoadData.AddListener(StartUpLoadData);
        }

        private void StartUpLoadData()
        {
            Debug.Log("Load data into managers");
            // Load resources
            List<ResourceData> testResources = new List<ResourceData> {_testResourceData};
            // Load buildings
            List<BuildingData> testBuildings = new List<BuildingData> {_testBuildingData};

            _resourceManager.StartUpLoadData(testResources);
            _buildingManager.StartUpLoadData(testBuildings);
            _unitManager.StartUpLoadData("unit");
            
            _tileManager.Init(7);
        }
    }
}
