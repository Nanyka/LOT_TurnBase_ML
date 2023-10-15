using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class BuyBuildingMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _tabHolder;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] private TabButton[] _buildingTas;
        [SerializeField] private List<BuildingBuyButton> _functionBuildings;
        [SerializeField] private List<BuildingBuyButton> _towers;
        [SerializeField] private List<BuildingBuyButton> _trap;
        [SerializeField] private List<BuildingBuyButton> _decoration;
        [SerializeField] private Transform _buildPoint;

        private int _layerMask = 1 << 6;
        private BuildingBuyButton _selectedBuilding;
        private IConfirmFunction _currentConfirm;
        private bool _isInADeal;

        private void Start()
        {
            SavingSystemManager.Instance.OnSetUpBuildingMenus.AddListener(Init);
            MainUI.Instance.OnBuyBuildingMenu.AddListener(ShowBuildingMenu);
            MainUI.Instance.OnHideAllMenu.AddListener(HideBuildingMenu);
        }

        private void Init(List<JIInventoryItem> inventories)
        {
            foreach (var buyButton in _functionBuildings)
                buyButton.TurnOff();
            foreach (var buyButton in _towers)
                buyButton.TurnOff();
            foreach (var buyButton in _trap)
                buyButton.TurnOff();
            foreach (var buyButton in _decoration)
                buyButton.TurnOff();

            // Split building into categories
            
            var orderedInventories = inventories.OrderBy(t => t.inventoryName);
            
            var index = 0;
            foreach (var inventory in orderedInventories)
                if (inventory.inventoryType == InventoryType.Building)
                    _functionBuildings[index++].TurnOn(inventory, this);

            index = 0;
            foreach (var inventory in orderedInventories)
                if (inventory.inventoryType == InventoryType.Tower)
                    _towers[index++].TurnOn(inventory, this);
            
            index = 0;
            foreach (var inventory in orderedInventories)
                if (inventory.inventoryType == InventoryType.Trap)
                    _trap[index++].TurnOn(inventory, this);
            
            index = 0;
            foreach (var inventory in orderedInventories)
                if (inventory.inventoryType == InventoryType.Decoration)
                    _decoration[index++].TurnOn(inventory, this);
        }


        private void ShowBuildingMenu(List<JIInventoryItem> inventories)
        {
            _tabHolder.SetActive(true);
            _buildingTas[0].OnActiveTab();
        }

        private void HideBuildingMenu()
        {
            _tabHolder.SetActive(false);
            foreach (var tabButton in _buildingTas)
                tabButton.OnDeactiveTab();
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
        
        public void EndSelectionPhase(IConfirmFunction confirmFunction)
        {
            _isInADeal = false;
            _currentConfirm = confirmFunction;
            _confirmPanel.SetActive(true);
        }

        public async void OnMakeTheDeal()
        {
            await SavingSystemManager.Instance.RefreshEconomy();
            _currentConfirm.ClickYes();
            CleanGhostBuilding();
        }

        public void OnCancelTheDeal()
        {
            _confirmPanel.SetActive(false);
            CleanGhostBuilding();
        }

        private void CleanGhostBuilding()
        {
            foreach (Transform child in _buildPoint)
                Destroy(child.gameObject);
            
            MainUI.Instance.OnHideAllMenu.Invoke();
        }

        public bool IsInADeal()
        {
            return _isInADeal;
        }

        #endregion
    }
}