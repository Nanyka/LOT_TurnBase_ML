using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class VirtualPurchaseButton : MonoBehaviour, IConfirmFunction
    {
        [SerializeField] private string _purchaseId;
        
        private ShoppingMenu m_ShoppingMenu;
        
        public void Init(ShoppingMenu shoppingMenu)
        {
            m_ShoppingMenu = shoppingMenu;
        }

        public void OnClick()
        {
            m_ShoppingMenu.OnStartADeal(this, _purchaseId);
        }

        public async void ClickYes()
        {
            await SavingSystemManager.Instance.OnConductVirtualPurchase(_purchaseId);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}