using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class CurrenciesInfo : MonoBehaviour
    {
        [SerializeField] private List<CurrencyButton> _currencyButtons;

        private void Start()
        {
            MainUI.Instance.OnUpdateCurrencies.AddListener(ShowCurrencies);
        }

        private void ShowCurrencies()
        {
            var currencies = SavingSystemManager.Instance.GetCurrencies();

            foreach (var currency in currencies)
            {
                var button = _currencyButtons.Find(t => t.m_Currency.Equals(currency.CurrencyId));
                if (button == null)
                    continue;

                button.UpdateCurrency(currency.Balance.ToString(),
                    SavingSystemManager.Instance.GetCurrencySprite(currency.CurrencyId));
            }
        }
    }
}