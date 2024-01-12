using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AoeWorkerMenu : MonoBehaviour, IDragDropMenu
    {
        public bool _isInADeal { get; set; }

        [SerializeField] private GameObject _creatureMenu;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] protected List<GameObject> _buttons;
        [SerializeField] private Transform _settlePoint;

        private List<IDragDropButton> _workerButtons = new();
        private int _layerMask = 1 << 6;
        private List<JIInventoryItem> m_Troops = new();
        private TroopDropButton _selectedCreature;
        private IConfirmFunction _currentConfirm;
        private bool _isInitiated;

        protected virtual void Start()
        {
            MainUI.Instance.OnShowCreatureMenu.AddListener(ShowWorkerMenu);
            MainUI.Instance.OnHideAllMenu.AddListener(HideCreatureMenu);

            foreach (var button in _buttons)
            {
                if (button.TryGetComponent(out IDragDropButton workerButton))
                    _workerButtons.Add(workerButton);
            }
        }

        private void ShowWorkerMenu(List<JIInventoryItem> inventoryItems)
        {
            if (_isInitiated)
            {
                _creatureMenu.SetActive(true);
                return;
            }

            _isInitiated = true;
            
            foreach (var troopDropButton in _workerButtons)
                troopDropButton.TurnOff();
            
            var orderedInventories = inventoryItems.OrderBy(t => t.inventoryName);

            foreach (var inventory in orderedInventories)
            {
                if (inventory.inventoryType != InventoryType.Worker)
                    continue;

                m_Troops.Add(inventory);
            }

            for (int i = 0; i < m_Troops.Count; i++)
            {
                _workerButtons[i].TurnOn(m_Troops[i], this);
                // _workerButtons[i].gameObject.SetActive(true);
            }

            _creatureMenu.SetActive(true);
        }

        public void HideCreatureMenu()
        {
            _creatureMenu.SetActive(false);
        }

        #region IN A DEAL

        public void StartADeal(string skinAddress)
        {
            _isInADeal = true;
            AddressableManager.Instance.GetAddressableGameObject(skinAddress, _settlePoint);
            MainUI.Instance.IsCameraMovable = false;
        }

        public void SelectLocation(Vector3 position)
        {
            _settlePoint.position = position;
        }

        public void EndDeal(IConfirmFunction confirmFunction)
        {
            _currentConfirm = confirmFunction;
            _confirmPanel.SetActive(true);
            _isInADeal = false;
        }

        public void MakeTheDeal()
        {
            _currentConfirm.ClickYes();
            CleanGhostCreature();
        }
        
        public void OnCancelTheDeal()
        {
            _confirmPanel.SetActive(false);
            _isInADeal = false;
            CleanGhostCreature();
        }

        private void CleanGhostCreature()
        {
            foreach (Transform child in _settlePoint)
                Destroy(child.gameObject);
            MainUI.Instance.IsCameraMovable = true;
        }

        #endregion
    }
    
    public interface IDragDropMenu
    {
        public void StartADeal(string skinAddress);
        public void SelectLocation(Vector3 position);
        public void EndDeal(IConfirmFunction confirmFunction);
    }
}