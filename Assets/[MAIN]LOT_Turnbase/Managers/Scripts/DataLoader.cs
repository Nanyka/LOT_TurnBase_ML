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
        [SerializeField] private CreatureManager _creatureManager;

        [Header("Test data")]
        [SerializeField] private ResourceData _testResourceData;
        [SerializeField] private BuildingData _testBuildingData;
        [SerializeField] private List<CreatureData> _testCreatureData;
        
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
            // Load creatures
            // List<CreatureData> testCreatures = new List<CreatureData> {_testCreatureData};

            _resourceManager.StartUpLoadData(testResources);
            _buildingManager.StartUpLoadData(testBuildings);
            _creatureManager.StartUpLoadData(_testCreatureData);
            
            _tileManager.Init(7);
        }
    }
}
