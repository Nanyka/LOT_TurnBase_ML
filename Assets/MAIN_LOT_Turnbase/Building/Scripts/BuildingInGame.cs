using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingInGame: MonoBehaviour
    {
        [SerializeField] private BuildingEntity m_Entity;

        private BuildingController _buildingController;
        
        public void Init(BuildingData buildingData, BuildingController buildingController)
        {
            m_Entity.Init(buildingData);
            
            _buildingController = buildingController;
            _buildingController.AddBuildingToList(this);
        }

        public int GetStoreSpace(CurrencyType currency, ref Queue<BuildingEntity> selectedBuildings)
        {
            return m_Entity.GetStorageSpace(currency, ref selectedBuildings);
        }

        public EntityData GetData()
        {
            return m_Entity.GetData();
        }

        public void StoreCurrency(int amount)
        {
            m_Entity.StoreCurrency(amount);
        }
    }
}