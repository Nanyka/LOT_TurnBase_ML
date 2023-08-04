using System;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace JumpeeIsland
{
    public class ShowBuildingMenuButton : FocusableButton
    {
        public void OnOpenBuildingMenu()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            SavingSystemManager.Instance.OnAskForShowingBuildingMenu();
            ActiveButton();
        }
    }
}