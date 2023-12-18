using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeBuildingLoader : MonoBehaviour, IBuildingLoader
    {
        [SerializeField] protected ObjectPool _buildingPool;
        [SerializeField] private FactionType _faction;

        private IBuildingController _buildingController;
        private List<BuildingData> _buildingDatas = new();
        
        private void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _buildingController = GetComponent<IBuildingController>();
        }
        
        public void Init()
        {
            var mapIndex = SavingSystemManager.Instance.GetEnvironmentData().mapSize;
            
            foreach (var building in _buildingDatas)
            {
                if (building.BuildingType == BuildingType.GUARDIANAREA && building.CurrentStorage != mapIndex)
                    continue;
                
                building.FactionType = _faction;
                ConstructBuilding(building);
            }
        }
        
        public void StartUpLoadData(List<BuildingData> data)
        {
            _buildingDatas = data;
        }

        public GameObject PlaceNewObject(BuildingData data)
        { 
            // var buildingData = (BuildingData)Convert.ChangeType(data, typeof(BuildingData));
            var building = ConstructBuilding(data);
            return building;
        }
        
        public void Reset()
        {
            _buildingPool.ResetPool();
            _buildingDatas = new();
        }

        public IBuildingController GetController()
        {
            return _buildingController;
        }
        
        public GameObject ConstructBuilding(BuildingData building)
        {
            building.EntityType = EntityType.BUILDING;
            var buildingObj = _buildingPool.GetObject(building.EntityName);
            // GameFlowManager.Instance.OnDomainRegister.Invoke(buildingObj, building.FactionType);
            
            if (buildingObj.TryGetComponent(out IBuildingDealer buildingEntity))
                buildingEntity.Init(building, _buildingController);

            // Debug.Log($"Building type of {building.EntityName}: {building.EntityType}");

            buildingObj.SetActive(true);
            return buildingObj;
        }

        public IEnumerable<GameObject> GetBuildings()
        {
            return _buildingPool.GetActiveItemList();
            // return _buildingController.GetBuildings();
        }
    }
}