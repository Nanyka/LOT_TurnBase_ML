using System.Collections.Generic;
using Unity.Services.Economy.Model;

namespace JumpeeIsland
{
    public class ShowShoppingMenuButton : FocusableButton
    {
        protected override void Start()
        {
            base.Start();
            MainUI.Instance.OnShowShoppingMenu.AddListener(ShowActiveButton);
        }

        private void ShowActiveButton(List<VirtualPurchaseDefinition> arg0)
        {
            ActiveButton();
        }
        
        public void OnOpenShoppingMenu()
        {
            _isActive = true; // MUST BE placed this place, it will be executed from MainUI.Instance.OnShowCreatureMenu event
            MainUI.Instance.OnHideAllMenu.Invoke();
            SavingSystemManager.Instance.OnAskForShowingShoppingMenu();
        }
    }
}