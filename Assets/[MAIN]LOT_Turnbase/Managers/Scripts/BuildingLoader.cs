using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingLoader : MonoBehaviour, IStartUpLoadData
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
                StartUpProcessor.Instance.OnDomainRegister.Invoke(buildingObj, building.CreatureType);

                if (buildingObj.TryGetComponent(out BuildingInGame buildingInGame))
                {
                    buildingInGame.gameObject.SetActive(true);
                    buildingInGame.Init(building);
                }
            }
        }
        
        public void Reset()
        {
            _buildingPool.ResetPool();
            _buildingDatas = new();
        }
    }
}
