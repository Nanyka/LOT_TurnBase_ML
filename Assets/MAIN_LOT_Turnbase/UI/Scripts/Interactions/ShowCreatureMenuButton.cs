using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class ShowCreatureMenuButton : FocusableButton
    {
        public void OnOpenCreatureMenu()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            SavingSystemManager.Instance.OnAskForShowingCreatureMenu();
            ActiveButton();
        }
    }
}