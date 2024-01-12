using System;
using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTutorialBuildingEntity : AoeTutorialEntity
    {
        [SerializeField] private BuildingData m_BuildingData;
        [SerializeField] private bool _isUpdatePos;

        private IBuildingConstruct m_Constructor;
        private ISkinComp m_SkinComp;
        private IEntityUIUpdate m_UIUpdator;

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
            Init();
            gameObject.SetActive(false);
        }

        private void Init()
        {
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

        // public override void TakeDamage()
        // {
        //     gameObject.SetActive(true);
        //     
        //     if (_isUpdatePos == false)
        //         return;
        //
        //     var touchPoint = InputManager.Instance.GetTouchPoint();
        //     if (touchPoint != Vector3.zero)
        //         transform.position = touchPoint;
        // }
        //
        // public override void Die()
        // {
        //     gameObject.SetActive(false);
        // }
    }
}