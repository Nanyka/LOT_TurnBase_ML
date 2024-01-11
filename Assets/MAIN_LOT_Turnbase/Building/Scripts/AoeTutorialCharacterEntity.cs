using System;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class AoeTutorialCharacterEntity : AoeTutorialEntity
    {
        [HideInInspector] public UnityEvent<IAttackRelated> OnUnitDie = new();

        private IHealthComp m_HealthComp;
        private IAnimateComp m_AnimateComp;
        private ISkinComp m_SkinComp;
        
        private CreatureData m_CreatureData;

        private void Awake()
        {
            m_HealthComp = GetComponent<IHealthComp>();
            m_AnimateComp = GetComponent<IAnimateComp>();
            m_SkinComp = GetComponent<ISkinComp>();
        }
        
        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(WaitForSkin);
        }

        private void WaitForSkin()
        {
            Init(new CreatureData()
            {
                EntityName = "Zombie0",
                FactionType = FactionType.Player,
                CurrentLevel = 0
            });
            
            gameObject.SetActive(false);
        }

        public override void Init(EntityData entityData)
        {
            m_CreatureData = entityData as CreatureData;
            
            // Download skin
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
            m_CreatureData.SkinAddress =
                inventoryItem.skinAddress[
                    Mathf.Clamp(m_CreatureData.CurrentLevel, 0, inventoryItem.skinAddress.Count - 1)];
            
            m_SkinComp.Init(m_CreatureData.SkinAddress, m_AnimateComp);
            m_HealthComp.Init(inventoryItem.creatureStats[m_CreatureData.CurrentLevel].HealthPoint, OnUnitDie, m_CreatureData);
        }
    }
}