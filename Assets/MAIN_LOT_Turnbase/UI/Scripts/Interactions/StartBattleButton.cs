using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class StartBattleButton : MonoBehaviour
    {
        [SerializeField] private GameObject[] _inGameButtons;
        
        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(HideButton);
        }

        private void HideButton()
        {
            gameObject.SetActive(false);
        }

        public void OnClick()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            MainUI.Instance.OnEnableInteract.Invoke();

            foreach (var button in _inGameButtons)
                button.SetActive(true);
            
            GameFlowManager.Instance.OnKickOffEnv.Invoke();
        }
    }
}