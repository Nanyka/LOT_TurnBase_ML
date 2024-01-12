using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class AoeDragDropButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
        IConfirmFunction, IDragDropButton
    {
        [SerializeField] private GameObject m_Container;
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private TextMeshProUGUI m_Price;
        [SerializeField] private CurrencyUnit _constructingCost;

        private AsyncOperationHandle m_UCDObjectLoadingHandle;
        private IDragDropMenu _mBuyBuildingMenu;
        private JIInventoryItem m_BuidlingItem;
        private Vector3 _buildingPosition;
        private int _layerMask = 1 << 6;
        private Camera _camera;

        private void OnEnable()
        {
            _camera = Camera.main;
        }

        public void TurnOn(JIInventoryItem buildingItem, IDragDropMenu buyBuildingMenu)
        {
            m_BuidlingItem = buildingItem;
            m_ItemIcon.sprite = AddressableManager.Instance.GetAddressableSprite(m_BuidlingItem.spriteAddress);
            if (_mBuyBuildingMenu == null)
                _mBuyBuildingMenu = buyBuildingMenu;

            var costs = SavingSystemManager.Instance.GetPurchaseCost(buildingItem.virtualPurchaseId);
            if (costs.Count > 0)
            {
                _constructingCost.currencyId = costs[0].id;
                _constructingCost.amount = costs[0].amount;
            }
            m_Price.text = _constructingCost.amount.ToString();

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

            _buildingPosition = collidedTile.point;
            _buildingPosition = new Vector3(Mathf.RoundToInt(_buildingPosition.x),
                Mathf.RoundToInt(_buildingPosition.y), Mathf.RoundToInt(_buildingPosition.z));
            _mBuyBuildingMenu.SelectLocation(_buildingPosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _mBuyBuildingMenu.EndDeal(this);
        }

        public virtual void ClickYes()
        {
            SavingSystemManager.Instance.OnPlaceABuilding(m_BuidlingItem, _buildingPosition, _constructingCost);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }

    public interface IDragDropButton
    {
        public void TurnOn(JIInventoryItem buildingItem, IDragDropMenu dragDropMenu);
        public void TurnOff();
    }
}