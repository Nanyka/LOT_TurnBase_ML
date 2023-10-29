using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class ProduceComp : MonoBehaviour
    {
        private BuildingEntity m_BuildingEntity;

        private void Start()
        {
            m_BuildingEntity = GetComponent<BuildingEntity>();

            if (GameFlowManager.Instance.GameMode == GameMode.ECONOMY)
                GameFlowManager.Instance.GetEnvManager().OnChangeFaction.AddListener(ProduceResources);
        }

        private void ProduceResources()
        {
            if (GameFlowManager.Instance.GetEnvManager().GetCurrFaction() == FactionType.Player)
            {
                var buildingData = m_BuildingEntity.GetData() as BuildingData;
                if (buildingData == null || buildingData.CurrentHp <= 0)
                    return;
                
                var product = m_BuildingEntity.GetStats().Productivity;
                product = product <= buildingData.GetStoreSpace() ? product : buildingData.GetStoreSpace();

                m_BuildingEntity.StoreCurrency(product);
                SavingSystemManager.Instance.IncrementLocalCurrency(buildingData.StorageCurrency.ToString(), product);
            }
        }
    }
}