using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoePlayerBuildingController : MonoBehaviour, IBuildingController
    {
        private List<BuildingInGame> _buildingInGames = new();
        private Camera _camera;
        private int _layerMask = 1 << 9;
        
        public void Init()
        {
            Debug.Log("Player building controller have no any initiation");
        }

        public void AddBuildingToList(BuildingInGame building)
        {
            _buildingInGames.Add(building);
        }

        public void RemoveBuilding(BuildingInGame building)
        {
            throw new System.NotImplementedException();
        }

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