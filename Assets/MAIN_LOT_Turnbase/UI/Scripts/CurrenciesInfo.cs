using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class CurrenciesInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currenciesText;
        
        private void Start()
        {
            MainUI.Instance.OnUpdateCurrencies.AddListener(Show);
        }

        private void Show()
        {
            var currencies = SavingSystemManager.Instance.GetCurrencies();
            string currenciesText = "";

            foreach (var currency in currencies)
                currenciesText += $"{currency.CurrencyId}:{currency.Balance} |";
            
            _currenciesText.text = $"{currenciesText}";
        }
    }
}