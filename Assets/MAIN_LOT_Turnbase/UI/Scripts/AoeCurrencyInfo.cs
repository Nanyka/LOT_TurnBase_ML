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
            MainUI.Instance.OnUpdateCurrencies.AddListener(UpdateCurrencies);
        }

        private void UpdateCurrencies()
        {
            // Update player's coin
            var coinCurrency = SavingSystemManager.Instance.GetCurrencies().First(t => t.CurrencyId.Equals("COIN"));

            var coinButton = _currencyButtons.Find(t => t.m_Currency.Equals("COIN"));
            coinButton.UpdateCurrency(coinCurrency.Balance.ToString(), SavingSystemManager.Instance.GetCurrencySprite("COIN"));
            
            // Update player's troop amount
            var troopAmount = SavingSystemManager.Instance.GetStorageController().GetStorages()
                .Sum(t => t.GetSpawnableAmount());
            var troopButton = _currencyButtons.Find(t => t.m_Currency.Equals("TROOP"));
            troopButton.UpdateCurrency(troopAmount.ToString());
            
            // Update collected mana
            var manaCurrency = SavingSystemManager.Instance.GetCurrencies().First(t => t.CurrencyId.Equals("WOOD"));

            var manaButton = _currencyButtons.Find(t => t.m_Currency.Equals("MANA"));
            manaButton.UpdateCurrency(manaCurrency.Balance.ToString());
        }
    }
}