using UnityEngine;

namespace JumpeeIsland
{
    public class ShowBossSelectorButton : MonoBehaviour
    {
        public void OnOpenBossSelector()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            MainUI.Instance.OnShowBossSelector.Invoke();
        }
    }
}