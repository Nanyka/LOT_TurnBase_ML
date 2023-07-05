using UnityEngine;

namespace JumpeeIsland
{
    public class ShowCreatureMenuButton : MonoBehaviour
    {
        public void OnOpenCreatureMenu()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            SavingSystemManager.Instance.OnAskForShowingCreatureMenu();
        }
    }
}