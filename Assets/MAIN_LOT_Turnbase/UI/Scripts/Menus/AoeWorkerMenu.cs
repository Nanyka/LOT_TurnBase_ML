using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AoeWorkerMenu : MonoBehaviour
    {
        public bool _isInADeal { get; set; }

        [SerializeField] private GameObject _creatureMenu;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] protected List<WorkerButton> _workerButtons;
        [SerializeField] private Transform _settlePoint;

        private int _layerMask = 1 << 6;
        private List<JIInventoryItem> m_Troops = new();
        private TroopDropButton _selectedCreature;
        private IConfirmFunction _currentConfirm;
        private bool _isInitiated;

        protected virtual void Start()
        {
            MainUI.Instance.OnShowCreatureMenu.AddListener(ShowWorkerMenu);
            // MainUI.Instance.OnHideAllMenu.AddListener(HideCreatureMenu);
            // MainUI.Instance.OnShowDropTroopMenu.AddListener(ShowMenu);
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

            foreach (var inventory in inventoryItems)
            {
                if (inventory.inventoryType != InventoryType.Worker)
                    continue;

                m_Troops.Add(inventory);
            }

            for (int i = 0; i < m_Troops.Count; i++)
            {
                _workerButtons[i].TurnOn(m_Troops[i], this);
                _workerButtons[i].gameObject.SetActive(true);
            }

            _creatureMenu.SetActive(true);
        }

        // private void ShowMenu(List<CreatureData> troops)
        // {
        //     m_Troops = troops;
        //     foreach (var troopDropButton in _workerButtons)
        //         troopDropButton.TurnOff();
        //
        //     for (int i = 0; i < m_Troops.Count; i++)
        //     {
        //         _workerButtons[i].TurnOn(m_Troops[i], this);
        //         _workerButtons[i].gameObject.SetActive(true);
        //     }
        //
        //     _creatureMenu.SetActive(true);
        // }

        public void HideCreatureMenu()
        {
            _creatureMenu.SetActive(false);
        }

        #region MANAGE MENU

        // Just use this in BattleMode
        // public void CheckEmptyMenu()
        // {
        //     if (_workerButtons.Count(t => t.CheckActive()) == 0)
        //     {
        //         HideCreatureMenu();
        //         StartCoroutine(WaitToStartGame());
        //     }
        // }
        //
        // private IEnumerator WaitToStartGame()
        // {
        //     yield return new WaitForSeconds(1f);
        //     GameFlowManager.Instance.OnKickOffEnv.Invoke();
        // }

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
            
            // if (GameFlowManager.Instance.GetEnvManager().FreeToMove(position))
            //     _settlePoint.position = position + Vector3.up * 0.5f;
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

        private void CleanGhostCreature()
        {
            foreach (Transform child in _settlePoint)
                Destroy(child.gameObject);
        }

        #endregion
    }
}