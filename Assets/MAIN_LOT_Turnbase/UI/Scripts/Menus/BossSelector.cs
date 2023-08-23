using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class BossSelector : MonoBehaviour
    {
        [SerializeField] private GameObject _bossSelector;
        [SerializeField] private List<BossDoorButton> _doorButtons;
        [SerializeField] private GameObject _selectBossPanel;
        private IConfirmFunction m_Confirm;

        private void Start()
        {
            MainUI.Instance.OnShowBossSelector.AddListener(ShowSelector);
        }

        private void ShowSelector()
        {
            var currentBoss = SavingSystemManager.Instance.GetGameProcess().bossUnlock + 1; // bossUnlock is cached in zero-based order
            
            Debug.Log("Current boss: " + currentBoss);
            for (int i = 0; i < _doorButtons.Count; i++)
            {
                if (i < currentBoss)
                    _doorButtons[i].Init(false,this);
                else
                    _doorButtons[i].Init(true,this);
            }
            _bossSelector.SetActive(true);
        }

        public void OpenSelectBossPanel(IConfirmFunction confirmFunction)
        {
            m_Confirm = confirmFunction;
            _selectBossPanel.SetActive(true);
            MainUI.Instance.OnShowAnUI.Invoke();
        }

        public void OnClickYes()
        {
            m_Confirm.ClickYes();
        }
    }
}