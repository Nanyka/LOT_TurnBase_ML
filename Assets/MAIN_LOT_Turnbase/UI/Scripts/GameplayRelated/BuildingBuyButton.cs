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
        // [SerializeField] private TextMeshProUGUI m_ItemName;
        [SerializeField] private Image m_ItemIcon;
        

        private AsyncOperationHandle m_UCDObjectLoadingHandle;
        private BuyBuildingMenu _mBuyBuildingMenu;
        private JIInventoryItem m_BuidlingItem;
        private Vector3 _buildingPosition;
        private int _layerMask = 1 << 6;
        private Camera _camera;

        private void OnEnable()
        {
            _camera = Camera.main;
        }

        public void TurnOn(JIInventoryItem buildingItem, BuyBuildingMenu buyBuildingMenu)
        {
            m_BuidlingItem = buildingItem;
            // m_ItemName.text = m_BuidlingItem.inventoryName;
            m_ItemIcon.sprite = AddressableManager.Instance.GetAddressableSprite(m_BuidlingItem.spriteAddress);
            if (_mBuyBuildingMenu == null)
                _mBuyBuildingMenu = buyBuildingMenu;

            m_Container.SetActive(true);
        }

        public void TurnOff()
        {
            m_Container.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _mBuyBuildingMenu.StartADeal(m_BuidlingItem.skinAddress[0]);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(eventData.position);
            if (!Physics.Raycast(ray, out var collidedTile, 100f, _layerMask))
                return;
            
            _mBuyBuildingMenu.SelectLocation(collidedTile.transform.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var collidedTile, 100f, _layerMask))
                return;

            _buildingPosition = collidedTile.transform.position;
            _mBuyBuildingMenu.EndDeal(this);
        }

        public void ClickYes()
        {
            SavingSystemManager.Instance.OnPlaceABuilding(m_BuidlingItem,_buildingPosition);
        }

        public Entity GetEntity()
        {
            throw new NotImplementedException();
        }
    }
}