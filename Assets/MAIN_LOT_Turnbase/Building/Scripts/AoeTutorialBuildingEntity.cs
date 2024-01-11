using System;
using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTutorialBuildingEntity : AoeTutorialEntity
    {
        private IBuildingConstruct m_Constructor;
        private ISkinComp m_SkinComp;
        private IEntityUIUpdate m_UIUpdator;
        private BuildingData m_BuildingData { get; set; }

        private void Awake()
        {
            m_Constructor = GetComponent<IBuildingConstruct>();
            m_SkinComp = GetComponent<ISkinComp>();
            m_UIUpdator = GetComponent<IEntityUIUpdate>();
        }

        // For Debug purpose
        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(WaitForSkin);
        }

        private void WaitForSkin()
        {
            Init(new BuildingData()
            {
                EntityName = "WarriorForge",
                FactionType = FactionType.Player,
                CurrentLevel = 0
            });
            
            gameObject.SetActive(false);
        }

        public override void Init(EntityData entityData)
        {
            m_BuildingData = entityData as BuildingData;
            
            RefreshEntity();
        }

        private void RefreshEntity()
        {
            m_Constructor.Init(m_BuildingData.FactionType);
            m_UIUpdator.ShowBars(false,true,false);
            
            // Download skin
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_BuildingData.EntityName);
            m_BuildingData.SkinAddress = inventoryItem.skinAddress.Count > m_BuildingData.CurrentLevel
                ? inventoryItem.skinAddress[m_BuildingData.CurrentLevel]
                : inventoryItem.skinAddress[0];
            m_SkinComp.Init(m_BuildingData.SkinAddress);
        }
    }
}