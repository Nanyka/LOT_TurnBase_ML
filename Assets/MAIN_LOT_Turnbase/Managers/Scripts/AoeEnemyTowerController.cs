using UnityEngine;

namespace JumpeeIsland
{
    public class AoeEnemyTowerController : MonoBehaviour, IBuildingController
    {
        public void Init()
        {
            Debug.Log("Enemy building controller do nothhing");
        }

        public void AddBuildingToList(BuildingInGame building)
        {
            
        }

        public void RemoveBuilding(BuildingInGame building)
        {
            throw new System.NotImplementedException();
        }
        
        // TODO: move it to a distinct interface
        public void StoreRewardAtBuildings(string currencyId, int amount)
        {
            throw new System.NotImplementedException();
        }

        public void DeductCurrencyFromBuildings(string currencyId, int amount)
        {
            throw new System.NotImplementedException();
        }
    }
}