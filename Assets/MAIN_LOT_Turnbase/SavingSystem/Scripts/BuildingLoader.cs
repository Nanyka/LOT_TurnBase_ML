using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingLoader : MonoBehaviour, ILoadData
    {
        [SerializeField] protected ObjectPool _buildingPool;
        [SerializeField] private FactionType _faction;

        protected BuildingController _buildingController;
        private List<BuildingData> _buildingDatas = new();
        private MainHallTier _currentTier;
        private MainHallTier _upcomingTier;
        
        public void StartUpLoadData<T>(T data)
        {
            _buildingDatas = (List<BuildingData>)Convert.ChangeType(data, typeof(List<BuildingData>));
        }
        
        private void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _buildingController = GetComponent<BuildingController>();
        }
        
        private async void Init()
        {
            foreach (var building in _buildingDatas)
            {
                building.FactionType = _faction;
                if (building.BuildingType == BuildingType.MAINHALL)
                    await UpdateTierInfo(building);

                ConstructBuilding(building);
            }

            _buildingController.Init();
        }

        private async Task UpdateTierInfo(BuildingData building)
        {
            _currentTier = await SavingSystemManager.Instance.GetMainHallTier(building.CurrentLevel);
            _upcomingTier = await SavingSystemManager.Instance.GetMainHallTier(building.CurrentLevel+1);
        }

        public MainHallTier GetCurrentTier()
        {
            return _currentTier;
        }
        
        public MainHallTier GetUpcomingTier()
        {
            return _upcomingTier;
        }

        public GameObject PlaceNewObject<T>(T data)
        { 
            var buildingData = (BuildingData)Convert.ChangeType(data, typeof(BuildingData));
            var building = ConstructBuilding(buildingData);
            return building;
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
        
        protected virtual GameObject ConstructBuilding(BuildingData building)
        {
            var buildingObj = _buildingPool.GetObject(building.EntityName);
            GameFlowManager.Instance.OnDomainRegister.Invoke(buildingObj, building.FactionType);

            if (buildingObj.TryGetComponent(out BuildingInGame buildingInGame))
            {
                buildingInGame.gameObject.SetActive(true);
                buildingInGame.Init(building, _buildingController);
            }

            return buildingObj;
        }

        public IEnumerable<BuildingInGame> GetBuildings()
        {
            return _buildingController.GetBuildings();
        }
    }
}
