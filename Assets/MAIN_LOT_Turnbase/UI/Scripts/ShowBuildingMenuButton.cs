using UnityEngine;

namespace JumpeeIsland
{
    public class ShowBuildingMenuButton : MonoBehaviour
    {
        public void OnOpenBuildingMenu()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            SavingSystemManager.Instance.OnAskForShowingBuildingMenu();
        }
    }
}