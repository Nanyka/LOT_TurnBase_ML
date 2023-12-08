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
        // private MainHallTier _currentTier;
        // private MainHallTier _upcomingTier;
        
        public void StartUpLoadData<T>(T data)
        {
            _buildingDatas = (List<BuildingData>)Convert.ChangeType(data, typeof(List<BuildingData>));
        }
        
        private void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _buildingController = GetComponent<BuildingController>();
        }
        
        private void Init()
        {
            foreach (var building in _buildingDatas)
            {
                building.FactionType = _faction;
                ConstructBuilding(building);
            }

            _buildingController.Init();
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
            building.EntityType = EntityType.BUILDING;
            var buildingObj = _buildingPool.GetObject(building.EntityName);
            GameFlowManager.Instance.OnDomainRegister.Invoke(buildingObj, building.FactionType);

            if (buildingObj.TryGetComponent(out IBuildingDealer buildingInGame))
            {
                // buildingInGame.gameObject.SetActive(true);
                buildingObj.SetActive(true);
                buildingInGame.Init(building, _buildingController);
            }

            return buildingObj;
        }

        public IEnumerable<GameObject> GetBuildings()
        {
            return _buildingPool.GetActiveItemList();
            // return _buildingController.GetBuildings();
        }
    }
}
