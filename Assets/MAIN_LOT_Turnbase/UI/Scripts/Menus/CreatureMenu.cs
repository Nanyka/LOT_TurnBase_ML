using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class CreatureMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _creatureMenu;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] protected List<CreatureBuyButton> _buyButtons;
        [SerializeField] private Transform _settlePoint;

        private int _layerMask = 1 << 6;
        private List<JIInventoryItem> m_Inventory;
        private Dictionary<string, int> m_Menu = new();
        private CreatureBuyButton _selectedCreature;
        private IConfirmFunction _currentConfirm;
        private bool _isInADeal;

        protected virtual void Start()
        {
            MainUI.Instance.OnShowCreatureMenu.AddListener(ShowCreatureMenu);
            MainUI.Instance.OnHideAllMenu.AddListener(HideCreatureMenu);
        }

        private void ShowCreatureMenu(List<JIInventoryItem> inventories)
        {
            m_Inventory = inventories;
            foreach (var buyButton in _buyButtons)
                buyButton.TurnOff();

            foreach (var inventory in m_Inventory)
            {
                if (inventory.inventoryType != InventoryType.Creature)
                    continue;

                if (m_Menu.ContainsKey(inventory.inventoryName) == false)
                    m_Menu.Add(inventory.inventoryName, 1);
                else
                    m_Menu[inventory.inventoryName]++;
            }

            var index = 0;
            foreach (var item in m_Menu)
                _buyButtons[index++].TurnOn(m_Inventory.Find(t => t.inventoryName == item.Key), this);

            _creatureMenu.SetActive(true);
        }

        private void HideCreatureMenu()
        {
            _creatureMenu.SetActive(false);
        }

        #region MANAGE MENU

        public int GetAmountById(string itemId)
        {
            return m_Menu[itemId];
        }

        public void DecreaseAmount(string itemId)
        {
            m_Menu[itemId]--;
        }

        // Just use this in BattleMode
        public void CheckEmptyMenu()
        {
            if (m_Menu.Sum(t => t.Value) > 0)
                return;
            
            HideCreatureMenu();
            MainUI.Instance.OnEnableInteract.Invoke();
            GameFlowManager.Instance.OnKickOffEnv.Invoke();
        }

        #endregion

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

        public void OnMakeTheDeal()
        {
            _currentConfirm.ClickYes();
            CleanGhostCreature();
        }
        
        public void OnCancelTheDeal()
        {
            _confirmPanel.SetActive(false);
            CleanGhostCreature();
        }

        private void CleanGhostCreature()
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