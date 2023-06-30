using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuyBuildingMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _buyBuildingMenu;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] private List<BuildingBuyButton> _buyButtons;
        [SerializeField] private Transform _buildPoint;

        private int _layerMask = 1 << 6;
        private BuildingBuyButton _selectedBuilding;
        private IConfirmFunction _currentConfirm;
        private bool _isInADeal;

        private void Start()
        {
            MainUI.Instance.OnBuyBuildingMenu.AddListener(ShowBuildingMenu);
            MainUI.Instance.OnHideAllMenu.AddListener(HideBuildingMenu);
        }

        private void ShowBuildingMenu(List<JIInventoryItem> inventories)
        {
            foreach (var buyButton in _buyButtons)
                buyButton.TurnOff();

            var index = 0;
            var orderedInventories = inventories.OrderBy(t => t.inventoryName);
            foreach (var inventory in orderedInventories)
            {
                if (inventory.inventoryType != InventoryType.Building)
                    continue;
                _buyButtons[index++].TurnOn(inventory, this);
            }

            _buyBuildingMenu.SetActive(true);
        }

        private void HideBuildingMenu()
        {
            _buyBuildingMenu.SetActive(false);
        }

        #region IN A DEAL

        public void StartADeal(string skinAddress)
        {
            _isInADeal = true;
            AddressableManager.Instance.GetAddressableGameObject(skinAddress, _buildPoint);
        }

        public void SelectLocation(Vector3 position)
        {
            _buildPoint.position = position;
        }
        
        public void EndDeal(IConfirmFunction confirmFunction)
        {
            _isInADeal = false;
            _currentConfirm = confirmFunction;
            _confirmPanel.SetActive(true);
        }

        public void OnMakeTheDeal()
        {
            _currentConfirm.ClickYes();
            CleanGhostBuilding();
        }

        private void CleanGhostBuilding()
        {
            foreach (Transform child in _buildPoint)
                Destroy(child.gameObject);
        }

        public bool IsInADeal()
        {
            return _isInADeal;
        }

        #endregion
    }
}