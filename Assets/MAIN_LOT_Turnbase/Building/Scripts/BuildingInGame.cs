using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingInGame: MonoBehaviour, IShowInfo, IConfirmFunction, IRemoveEntity
    {
        [SerializeField] private BuildingEntity m_Entity;

        private BuildingController _buildingController;
        
        public void Init(BuildingData buildingData, BuildingController buildingController)
        {
            m_Entity.Init(buildingData);
            
            _buildingController = buildingController;
            _buildingController.AddBuildingToList(this);
        }

        public void DurationDeduct(FactionType currentFaction)
        {
            if (currentFaction != m_Entity.GetFaction())
                return;
            
            m_Entity.DurationDeduct();
        }

        public int GetStoreSpace(CurrencyType currency, ref Queue<BuildingEntity> selectedBuildings)
        {
            return m_Entity.GetStorageSpace(currency, ref selectedBuildings);
        }

        public string ShowInfo()
        {
            var data = (BuildingData)m_Entity.GetData();
            return
                $"{data.EntityName}\nHp:{data.CurrentHp}\nStore:{data.StorageCurrency}\nSpace:{data.CurrentStorage}/{data.StorageCapacity}";
        }

        public void ClickYes()
        {
            DestroyBuilding(this.m_Entity);
        }

        private void DestroyBuilding(Entity killedByEntity)
        {
            // just contribute resource when it is killed by player faction as sell out this building
            if (killedByEntity.GetFaction() == FactionType.Player)
                SavingSystemManager.Instance.GrantCurrency(CurrencyType.GOLD.ToString(), m_Entity.CalculateSellingPrice());
            
            // Add exp for entity who killed this resource
            if (killedByEntity != m_Entity)
                killedByEntity.CollectExp(m_Entity.GetExpReward());
            
            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            StartCoroutine(DestroyVisual());
        }

        private IEnumerator DestroyVisual()
        {
            // VFX
            yield return new WaitForSeconds(1f);
            _buildingController.RemoveBuilding(this);
            gameObject.SetActive(false);
        }

        public void Remove(EnvironmentData environmentData)
        {
            environmentData.BuildingData.Remove((BuildingData)m_Entity.GetData());
        }
    }
}