using System.Collections.Generic;
using Unity.Services.Economy.Model;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeBattleCurrencyLoader : MonoBehaviour, ICurrencyLoader
    {
        private LocalBalancesData m_LocalBalances;
        private List<PlayerBalance> m_Currencies;

        public void SetLocalBalances(LocalBalancesData localBalancesData)
        {
            m_LocalBalances = localBalancesData;
        }

        public void Init(List<PlayerBalance> currencies)
        {
            Debug.Log("Load currencies...");

            m_Currencies = currencies;

            // Wood is used as MANA in the AoeBattleMode
            m_Currencies.Find(t => t.CurrencyId == "WOOD").Balance = 0;

            MainUI.Instance.OnUpdateCurrencies.Invoke();
        }

        public long GetCurrency(string currencyId)
        {
            return 0;
        }

        public void ResetCurrencies(List<PlayerBalance> currencies)
        {
            throw new System.NotImplementedException();
        }

        public void GrantMove(long moveAmount)
        {
            Debug.Log($"Battle not grant MOVE");
        }

        public void RefreshCurrencies(List<PlayerBalance> currencies)
        {
            throw new System.NotImplementedException();
        }

        public void GainCurrency(string currencyId, int currencyAmount)
        {
            m_Currencies.Find(t => t.CurrencyId == currencyId).Balance += currencyAmount;
            MainUI.Instance.OnUpdateCurrencies.Invoke();
        }

        public void DeductCurrency(string currencyId, int currencyAmount)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckEnoughCurrency(string currencyId, int currencyAmount)
        {
            return m_Currencies.Find(t => t.CurrencyId == currencyId).Balance >= currencyAmount;
        }

        public IEnumerable<PlayerBalance> GetCurrencies()
        {
            return m_Currencies;
        }
    }
}