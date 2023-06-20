using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class SellBuildingMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _sellBuildingMenu;
        [SerializeField] private GameObject _confirmPanel;

        private IConfirmFunction _currentConfirm;
        
        private void Start()
        {
            MainUI.Instance.OnSellBuildingMenu.AddListener(ShowSellBuildingMenu);
        }

        private void ShowSellBuildingMenu(IConfirmFunction confirmFunction)
        {
            _currentConfirm = confirmFunction;
            _sellBuildingMenu.SetActive(true);
        }

        public void OnClickSell()
        {
            _sellBuildingMenu.SetActive(false);
            _confirmPanel.SetActive(true);
        }

        public void OnMakeTheDeal()
        {
            _currentConfirm.ClickYes();
        }
    }
}