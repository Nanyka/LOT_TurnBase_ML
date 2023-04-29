using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class BuildingManager : MonoBehaviour, IStartUpLoadData
    {
        [SerializeField] private ObjectPool _buildingPool;
        
        private List<BuildingData> _buildingDatas;
        
        public void StartUpLoadData<T>(T data)
        {
            _buildingDatas = (List<BuildingData>)Convert.ChangeType(data, typeof(List<BuildingData>));
            // Debug.Log($"Building manager loaded {data} data");
        }
        
        private void Start()
        {
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(Init);
        }
        
        private void Init()
        {
            foreach (var building in _buildingDatas)
            {
                var buildingObj = _buildingPool.GetObject();

                if (buildingObj.TryGetComponent(out BuildingInGame buildingInGame))
                {
                    buildingInGame.gameObject.SetActive(true);
                    Debug.Log(buildingInGame);
                    buildingInGame.Init(building);
                }
            }
        }
    }
}
