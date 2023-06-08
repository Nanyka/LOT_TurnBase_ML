using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingLoader : MonoBehaviour, IStartUpLoadData
    {
        [SerializeField] private ObjectPool _buildingPool;

        private BuildingController _buildingController;
        private List<BuildingData> _buildingDatas;
        
        public void StartUpLoadData<T>(T data)
        {
            _buildingDatas = (List<BuildingData>)Convert.ChangeType(data, typeof(List<BuildingData>));
        }
        
        private void Start()
        {
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(Init);
            _buildingController = GetComponent<BuildingController>();
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
                    buildingInGame.Init(building, _buildingController);
                }
            }
        }
        
        public void Reset()
        {
            _buildingPool.ResetPool();
            _buildingDatas = new();
        }

        public BuildingController GetController()
        {
            return _buildingController;
        }
    }
}
