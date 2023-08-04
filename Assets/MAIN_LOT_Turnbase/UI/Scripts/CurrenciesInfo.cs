using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class CurrenciesInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currenciesText;
        [SerializeField] private List<CurrencyButton> _currencyButtons;

        private void Start()
        {
            MainUI.Instance.OnUpdateCurrencies.AddListener(Show);
        }

        private void Show()
        {
            var currencies = SavingSystemManager.Instance.GetCurrencies();
            string currenciesText = "";

            foreach (var currency in currencies)
            {
                var button = _currencyButtons.Find(t => t.m_Currency.Equals(currency.CurrencyId));
                if (button == null)
                    continue;

                button.UpdateCurrency(currency.Balance.ToString(),
                    SavingSystemManager.Instance.GetCurrencySprite(currency.CurrencyId));

                // currenciesText += $"{currency.CurrencyId}:{currency.Balance} |";
            }

            // _currenciesText.text = $"{currenciesText}";
        }
    }
}