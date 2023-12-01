using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeCurrencyInfo : MonoBehaviour
    {
        [SerializeField] private List<CurrencyButton> _currencyButtons;

        private void Start()
        {
            MainUI.Instance.OnUpdateCurrencies.AddListener(Show);
        }

        private void Show()
        {
            // Update player's coin
            var coinCurrency = SavingSystemManager.Instance.GetCurrencies().First(t => t.CurrencyId.Equals("COIN"));

            var coinButton = _currencyButtons.Find(t => t.m_Currency.Equals("COIN"));
            coinButton.UpdateCurrency(coinCurrency.Balance.ToString(), SavingSystemManager.Instance.GetCurrencySprite("COIN"));
            
            // Update player's troop amount
            
            // Update collected mana

        }
    }
}