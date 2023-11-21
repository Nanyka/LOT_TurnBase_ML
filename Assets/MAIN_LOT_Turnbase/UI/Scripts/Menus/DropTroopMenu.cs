using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class DropTroopMenu : MonoBehaviour
    {
        public bool _isInADeal { get; set; }

        [SerializeField] private GameObject _creatureMenu;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] protected List<TroopDropButton> _troopDropButtons;
        [SerializeField] private Transform _settlePoint;

        private int _layerMask = 1 << 6;
        private List<CreatureData> m_Troops;
        private TroopDropButton _selectedCreature;
        private IConfirmFunction _currentConfirm;

        protected virtual void Start()
        {
            // MainUI.Instance.OnShowCreatureMenu.AddListener(ShowCreatureMenu);
            MainUI.Instance.OnHideAllMenu.AddListener(HideCreatureMenu);
            MainUI.Instance.OnShowDropTroopMenu.AddListener(ShowMenu);
        }

        private void ShowMenu(List<CreatureData> troops)
        {
            m_Troops = troops;
            foreach (var troopDropButton in _troopDropButtons)
                troopDropButton.TurnOff();

            for (int i = 0; i < m_Troops.Count; i++)
            {
                _troopDropButtons[i].TurnOn(m_Troops[i], this);
                _troopDropButtons[i].gameObject.SetActive(true);
            }

            _creatureMenu.SetActive(true);
        }

        private void HideCreatureMenu()
        {
            _creatureMenu.SetActive(false);
        }

        #region MANAGE MENU

        // Just use this in BattleMode
        public void CheckEmptyMenu()
        {
            if (_troopDropButtons.Count(t => t.CheckActive()) == 0)
            {
                HideCreatureMenu();
                StartCoroutine(WaitToStartGame());
                return;
            }

            if (GameFlowManager.Instance.GameMode == GameMode.BOSS)
            {
                if (SavingSystemManager.Instance.GetEnvironmentData().PlayerData.Count >=
                    GameFlowManager.Instance.GetQuest().maxTroop)
                {
                    HideCreatureMenu();
                    StartCoroutine(WaitToStartGame());
                }
            }
        }

        private IEnumerator WaitToStartGame()
        {
            yield return new WaitForSeconds(1f);
            // MainUI.Instance.OnEnableInteract.Invoke();
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
            if (GameFlowManager.Instance.GetEnvManager().FreeToMove(position))
                _settlePoint.position = position + Vector3.up * 0.5f;
        }

        public void EndDeal(IConfirmFunction confirmFunction)
        {
            _currentConfirm = confirmFunction;
            _confirmPanel.SetActive(true);
            _isInADeal = false;
            Debug.Log($"Current confirm1: {_currentConfirm}");
        }

        public void MakeTheDeal()
        {
            Debug.Log($"Current confirm2: {_currentConfirm}");
            _currentConfirm.ClickYes();
            CleanGhostCreature();
        }

        public void CancelTheDeal()
        {
            _confirmPanel.SetActive(false);
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