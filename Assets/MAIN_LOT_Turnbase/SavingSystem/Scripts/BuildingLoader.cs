using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingLoader : MonoBehaviour, IBuildingLoader
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
            foreach (var building in _buildingDatas)
            {
                building.FactionType = _faction;
                ConstructBuilding(building);
            }

            _buildingController.Init();
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
        
        public virtual GameObject ConstructBuilding(BuildingData building)
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

    public interface IBuildingLoader
    {
        public void Init();
        public void StartUpLoadData(List<BuildingData> data);
        public IEnumerable<GameObject> GetBuildings();
        public GameObject ConstructBuilding(BuildingData building);
        public GameObject PlaceNewObject(BuildingData data);
        public IBuildingController GetController();
        public void Reset();
    }
}
