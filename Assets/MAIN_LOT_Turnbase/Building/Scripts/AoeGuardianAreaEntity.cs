using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeGuardianAreaEntity : Entity, IBuildingDealer, IBuildingUpgrade, IBuildingSale, IGetEntityData<GuardianAreaStat>
    {
        private ISkinComp m_SkinComp;
        private IGuardianArea m_GuardianArea;

        private BuildingData m_BuildingData { get; set; }
        private List<GuardianAreaStat> m_AreaStats;
        private GuardianAreaStat m_CurrentStat;

        public void Init(BuildingData buildingData, IBuildingController buildingController)
        {
            m_SkinComp = GetComponent<ISkinComp>();
            m_GuardianArea = GetComponent<IGuardianArea>();
            m_BuildingData = buildingData;
            RefreshEntity();
        }

        public override void Relocate(Vector3 position)
        {
            m_Transform.position = position;
        }

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            m_Transform.position = position;
            m_Transform.eulerAngles = rotation;

            m_BuildingData.Position = position;
            m_BuildingData.Rotation = rotation;
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override EntityData GetData()
        {
            return m_BuildingData;
        }

        public GuardianAreaStat GetStats()
        {
            return m_CurrentStat;
        }

        public override FactionType GetFaction()
        {
            return m_BuildingData.FactionType;
        }

        public override ISkinComp GetSkin()
        {
            return m_SkinComp;
        }

        public IChangeWorldState GetWorldStateChanger()
        {
            Debug.Log("Guardian area is not a constructable object");
            return null;
        }

        #region GENERAL

        protected virtual void RefreshEntity()
        {
            ResetEntity();

            m_Transform.position = m_BuildingData.Position;
            m_GuardianArea.SpawnGuardians(m_CurrentStat.Guardians);
        }

        private void ResetEntity()
        {
            // Set entity stats
            var inventory = SavingSystemManager.Instance.GetInventoryItemByName(m_BuildingData.EntityName);
            m_AreaStats = inventory.areaStats;
            m_CurrentStat = m_AreaStats[m_BuildingData.CurrentLevel];

            // Set initiate data if it's new
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_BuildingData.EntityName);
            m_BuildingData.SkinAddress = inventoryItem.skinAddress.Count > m_BuildingData.CurrentLevel
                ? inventoryItem.skinAddress[m_BuildingData.CurrentLevel]
                : inventoryItem.skinAddress[0];
            m_SkinComp.Init(m_BuildingData.SkinAddress);
        }

        #endregion

        #region BUILDING INTERACT

        public BuildingType GetBuildingType()
        {
            return m_BuildingData.BuildingType;
        }

        public int GetUpgradePrice()
        {
            return m_CurrentStat.PriceToUpdate;
        }

        public CurrencyType GetUpgradeCurrency()
        {
            return m_CurrentStat.UpgradeCurrency;
        }

        public int CalculateUpgradePrice()
        {
            return m_CurrentStat.PriceToUpdate;
        }

        public void BuildingUpdate()
        {
            if (m_BuildingData.CurrentLevel + 1 >= m_AreaStats.Count)
                return;

            m_BuildingData.CurrentLevel++;

            // Reset stats and appearance
            ResetEntity();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public int CalculateSellingPrice()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}