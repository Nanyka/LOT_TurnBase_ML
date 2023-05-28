using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingManager : MonoBehaviour, IStartUpLoadData
    {
        [SerializeField] private ObjectPool _buildingPool;
        [SerializeField] private FactionType _faction;
        
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
                StartUpProcessor.Instance.OnDomainRegister.Invoke(buildingObj, _faction);

                if (buildingObj.TryGetComponent(out BuildingInGame buildingInGame))
                {
                    buildingInGame.gameObject.SetActive(true);
                    buildingInGame.Init(building);
                }
            }
        }
    }
}
