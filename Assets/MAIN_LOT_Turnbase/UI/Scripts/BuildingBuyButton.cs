using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class BuildingBuyButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IConfirmFunction
    {
        [SerializeField] private GameObject m_Container;
        [SerializeField] private TextMeshProUGUI m_ItemName;
        [SerializeField] private Image m_ItemIcon;
        

        private AsyncOperationHandle m_UCDObjectLoadingHandle;
        private BuildingMenu m_BuildingMenu;
        private JIInventoryItem m_BuidlingItem;
        private Vector3 _buildingPosition;
        private int _layerMask = 1 << 6;
        private Camera _camera;

        private void OnEnable()
        {
            _camera = Camera.main;
        }

        public void TurnOn(JIInventoryItem buildingItem, BuildingMenu buildingMenu)
        {
            m_BuidlingItem = buildingItem;
            m_ItemName.text = m_BuidlingItem.inventoryName;
            m_ItemIcon.sprite = AddressableManager.Instance.GetAddressableSprite(m_BuidlingItem.spriteAddress);
            if (m_BuildingMenu == null)
                m_BuildingMenu = buildingMenu;

            m_Container.SetActive(true);
        }

        public void TurnOff()
        {
            m_Container.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_BuildingMenu.StartADeal(m_BuidlingItem.skinAddress);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(eventData.position);
            if (!Physics.Raycast(ray, out var collidedTile, 100f, _layerMask))
                return;
            
            m_BuildingMenu.SelectLocation(collidedTile.transform.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var collidedTile, 100f, _layerMask))
                return;

            _buildingPosition = collidedTile.transform.position;
            m_BuildingMenu.EndDeal(this);
        }

        public void ClickYes()
        {
            SavingSystemManager.Instance.OnPlaceABuilding(m_BuidlingItem.inventoryName,m_BuidlingItem.skinAddress,_buildingPosition);
        }
    }
}