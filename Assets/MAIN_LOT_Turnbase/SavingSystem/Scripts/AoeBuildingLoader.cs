using UnityEngine;

namespace JumpeeIsland
{
    public class AoeBuildingLoader : BuildingLoader
    {
        protected override void ConstructBuilding(BuildingData building)
        {
            Debug.Log($"Build {building.EntityName}");
            var buildingObj = _buildingPool.GetObject(building.EntityName);
            GameFlowManager.Instance.OnDomainRegister.Invoke(buildingObj, building.FactionType);
            
            // buildingObj.SetActive(true);

            if (buildingObj.TryGetComponent(out BuildingEntity buildingEntity))
            {
                buildingEntity.gameObject.SetActive(true);
                buildingEntity.Init(building);
            }
        }
    }
}