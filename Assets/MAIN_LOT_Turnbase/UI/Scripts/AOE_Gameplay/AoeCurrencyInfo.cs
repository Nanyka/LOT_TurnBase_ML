using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeCurrencyInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _profitText;
        [SerializeField] private List<CurrencyButton> _currencyButtons;

        private void Start()
        {
            MainUI.Instance.OnUpdateCurrencies.AddListener(UpdateCurrencies);
            MainUI.Instance.OnBossMapProfitUpdate.AddListener(UpdateProfitPanel);
        }

        private void UpdateCurrencies()
        {
            // Update player's coin
            var coinCurrency = SavingSystemManager.Instance.GetCurrencyById("COIN");

            var coinButton = _currencyButtons.Find(t => t.m_Currency.Equals("COIN"));
            coinButton.UpdateCurrency(coinCurrency.ToString(), SavingSystemManager.Instance.GetCurrencySprite("COIN"));
            
            // Update player's troop amount
            // var troopAmount = SavingSystemManager.Instance.GetStorageController().GetStorages()
            //     .Sum(t => t.GetSpawnableAmount());
            // var troopButton = _currencyButtons.Find(t => t.m_Currency.Equals("TROOP"));
            // troopButton.UpdateCurrency(troopAmount.ToString());
            
            // Update collected mana
            var manaCurrency = SavingSystemManager.Instance.GetCurrencyById("WOOD");

            var manaButton = _currencyButtons.Find(t => t.m_Currency.Equals("MANA"));
            manaButton.UpdateCurrency(manaCurrency.ToString());
        }

        private void UpdateProfitPanel(float profit)
        {
            _profitText.text = $"{Mathf.RoundToInt(profit * 100)}%";
        }
    }
}