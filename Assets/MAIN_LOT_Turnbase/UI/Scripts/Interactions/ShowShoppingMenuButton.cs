namespace JumpeeIsland
{
    public class ShowShoppingMenuButton : FocusableButton
    {
        public void OnOpenShoppingMenu()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            SavingSystemManager.Instance.OnAskForShowingShoppingMenu();
            ActiveButton();
        }
    }
}