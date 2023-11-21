using System;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace JumpeeIsland
{
    public class ShowBuildingMenuButton : FocusableButton
    {
        protected override void Start()
        {
            base.Start();
            MainUI.Instance.OnBuyBuildingMenu.AddListener(ShowActiveButton);
        }
        
        private void ShowActiveButton(List<JIInventoryItem> arg0)
        {
            ActiveButton();
        }
        
        public void OnOpenBuildingMenu()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            
            _isActive = true; // MUST BE placed this place, it will be executed from MainUI.Instance.OnShowCreatureMenu event
            SavingSystemManager.Instance.OnAskForShowingBuildingMenu();
        }
    }
}