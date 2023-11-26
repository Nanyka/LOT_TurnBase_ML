using UnityEngine;

namespace JumpeeIsland
{
    public class AoeBuildingLoader : BuildingLoader
    {
        protected override GameObject ConstructBuilding(BuildingData building)
        {
            var buildingObj = _buildingPool.GetObject(building.EntityName);
            GameFlowManager.Instance.OnDomainRegister.Invoke(buildingObj, building.FactionType);
            
            if (buildingObj.TryGetComponent(out BuildingInGame buildingEntity))
            {
                buildingEntity.gameObject.SetActive(true);
                buildingEntity.Init(building, _buildingController);
            }

            return buildingObj;
        }
    }
}