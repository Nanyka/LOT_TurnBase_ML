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
        
        private void Start()
        {
            StartUpProcessor.Instance.OnLoadData.AddListener(StartUpLoadData);
        }

        private void StartUpLoadData()
        {
            Debug.Log("Load data into managers");
            // Load resources
            var testResource = new ResourceInGame();
            testResource.SetPosition(new Vector3(1,2,3));
            List<ResourceInGame> testResources = new List<ResourceInGame>();
            testResources.Add(testResource);
            
            _resourceManager.StartUpLoadData(testResources);
            _buildingManager.StartUpLoadData("building");
            _unitManager.StartUpLoadData("unit");
            
            _tileManager.Init(7);
        }
    }
}
