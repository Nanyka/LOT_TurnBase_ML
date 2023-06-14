using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class CreatureMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _creatureMenu;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] private List<CreatureBuyButton> _buyButtons;
        [SerializeField] private Transform _settlePoint;
        
        private int _layerMask = 1 << 6;
        private CreatureBuyButton _selectedCreature;
        private IConfirmFunction _currentConfirm;
        private bool _isInADeal;

        private void Start()
        {
            MainUI.Instance.OnShowCreatureMenu.AddListener(ShowCreatureMenu);
            MainUI.Instance.OnHideAllMenu.AddListener(HideCreatureMenu);
        }

        private void ShowCreatureMenu(List<JIInventoryItem> inventories)
        {
            foreach (var buyButton in _buyButtons)
                buyButton.TurnOff();

            var index = 0;
            foreach (var inventory in inventories)
            {
                if (inventory.inventoryType != InventoryType.Creature)
                    continue;
                _buyButtons[index++].TurnOn(inventory, this);
            }

            _creatureMenu.SetActive(true);
        }

        private void HideCreatureMenu()
        {
            _creatureMenu.SetActive(false);
        }

        #region IN A DEAL

        public void StartADeal(string skinAddress)
        {
            _isInADeal = true;
            AddressableManager.Instance.GetAddressableGameObject(skinAddress, _settlePoint);
        }

        public void SelectLocation(Vector3 position)
        {
            _settlePoint.position = position;
        }
        
        public void EndDeal(IConfirmFunction confirmFunction)
        {
            _isInADeal = false;
            _currentConfirm = confirmFunction;
            _confirmPanel.SetActive(true);
        }

        public void MakeTheDeal()
        {
            _currentConfirm.ClickYes();
            CleanGhostBuilding();
        }

        private void CleanGhostBuilding()
        {
            foreach (Transform child in _settlePoint)
                Destroy(child.gameObject);
        }

        public bool IsInADeal()
        {
            return _isInADeal;
        }

        #endregion
    }
}