using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingController : MonoBehaviour
    {
        private List<BuildingInGame> m_buildings = new();

        public void AddBuildingToList(BuildingInGame building)
        {
            m_buildings.Add(building);
        }

        public void StoreRewardToBuildings(string currencyId, int amount, Vector3 fromPos)
        {
            // Check if enough storage space
            int currentStorage = 0;
            int nearestIndex = 0;
            Queue<BuildingEntity> selectedBuildings = new Queue<BuildingEntity>();
            float nearestDistance = Mathf.Infinity;
            if (Enum.TryParse(currencyId, out CurrencyType currency))
                foreach (var t in m_buildings)
                    currentStorage += t.GetStoreSpace(currency, ref selectedBuildings);
            
            amount = amount > currentStorage ? currentStorage : amount;

            if (amount == 0 || selectedBuildings.Count == 0)
                return;

            // Stock currency to building and grain exp
            while (amount > 0)
            {
                var building = selectedBuildings.Dequeue();
                var storeAmount = building.GetStorageSpace(currency);
                storeAmount = storeAmount > amount ? amount : storeAmount;
                building.StoreCurrency(storeAmount);
                SavingSystemManager.Instance.IncrementLocalCurrency(currencyId, storeAmount); // Update UI
                amount -= storeAmount;
            }
        }
    }
}