using System.Collections.Generic;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class ShoppingMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _shoppingMenu;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] private CurrencyGroup _rewardGroup;
        [SerializeField] private CurrencyGroup _costGroup;
        [SerializeField] private List<VirtualPurchaseButton> _virtualPurchases;

        private IConfirmFunction _currentConfirm;
        
        protected virtual void Start()
        {
            MainUI.Instance.OnShowShoppingMenu.AddListener(ShowShoppingMenu);
            MainUI.Instance.OnHideAllMenu.AddListener(HideShoppingMenu);

            foreach (var purchase in _virtualPurchases)
            {
                purchase.Init(this);
            }
        }

        private void ShowShoppingMenu(List<VirtualPurchaseDefinition> virtualPurchases)
        {
            _shoppingMenu.SetActive(true);
        }

        private void HideShoppingMenu()
        {
            _shoppingMenu.SetActive(false);
        }

        public void OnStartADeal(IConfirmFunction confirmFunction, string purchaseId)
        {
            _currentConfirm = confirmFunction;
            _confirmPanel.SetActive(true);
            
            var purchaseReward = SavingSystemManager.Instance.GetPurchaseReward(purchaseId);
            _rewardGroup.VisualCurrency(purchaseReward[0].id,purchaseReward[0].amount);
            
            var purchaseCost = SavingSystemManager.Instance.GetPurchaseCost(purchaseId);
            if (purchaseCost.Count == 0)
                _costGroup.VisualCurrency("Buy");
            else
                _costGroup.VisualCurrency(purchaseCost[0].id,purchaseCost[0].amount);
        }
        
        public void OnMakeTheDeal()
        {
            _currentConfirm.ClickYes();
        }
    }
}