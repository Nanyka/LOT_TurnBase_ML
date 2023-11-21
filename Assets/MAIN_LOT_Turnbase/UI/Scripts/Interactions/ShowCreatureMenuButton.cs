using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class ShowCreatureMenuButton : FocusableButton
    {
        protected override void Start()
        {
            base.Start();
            MainUI.Instance.OnShowCreatureMenu.AddListener(ShowActiveButton);
        }

        private void ShowActiveButton(List<JIInventoryItem> arg0)
        {
            ActiveButton();
        }

        public void OnOpenCreatureMenu()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            
            _isActive = true; // MUST BE placed this place, it will be executed from MainUI.Instance.OnShowCreatureMenu event
            SavingSystemManager.Instance.OnAskForShowingCreatureMenu();
        }
    }
}