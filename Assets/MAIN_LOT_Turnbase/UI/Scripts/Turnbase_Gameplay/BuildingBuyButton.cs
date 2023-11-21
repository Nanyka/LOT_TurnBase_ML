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
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private TextMeshProUGUI m_Price;
        
        private AsyncOperationHandle m_UCDObjectLoadingHandle;
        private BuyBuildingMenu _mBuyBuildingMenu;
        protected JIInventoryItem m_BuidlingItem;
        protected Vector3 _buildingPosition;
        private int _layerMask = 1 << 6;
        private Camera _camera;

        private void OnEnable()
        {
            _camera = Camera.main;
        }

        public void TurnOn(JIInventoryItem buildingItem, BuyBuildingMenu buyBuildingMenu)
        {
            m_BuidlingItem = buildingItem;
            m_ItemIcon.sprite = AddressableManager.Instance.GetAddressableSprite(m_BuidlingItem.spriteAddress);
            if (_mBuyBuildingMenu == null)
                _mBuyBuildingMenu = buyBuildingMenu;

            var costs = SavingSystemManager.Instance.GetPurchaseCost(buildingItem.virtualPurchaseId);
            if (costs.Count > 0)
                m_Price.text = costs[0].amount.ToString();
            
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
            _mBuyBuildingMenu.EndSelectionPhase(this);
        }

        public virtual void ClickYes()
        {
            SavingSystemManager.Instance.OnPlaceABuilding(m_BuidlingItem,_buildingPosition, true);
        }

        public Entity GetEntity()
        {
            throw new NotImplementedException();
        }
    }
}