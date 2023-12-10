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

            foreach (var localBalance in m_LocalBalances.LocalBalances)
            {
                var currency = m_Currencies.Find(t => t.CurrencyId == localBalance.CurrencyId);
                if (localBalance.Balance != currency.Balance)
                {
                    currency.Balance = localBalance.Balance;
                    SavingSystemManager.Instance.OnSetCloudCurrency(localBalance.CurrencyId, localBalance.Balance);
                }
            }

            MainUI.Instance.OnUpdateCurrencies.Invoke();
        }

        public long GetCurrency(string currencyId)
        {
            throw new System.NotImplementedException();
        }

        public void ResetCurrencies(List<PlayerBalance> currencies)
        {
            throw new System.NotImplementedException();
        }

        public void GrantMove(long moveAmount)
        {
            throw new System.NotImplementedException();
        }

        public void RefreshCurrencies(List<PlayerBalance> currencies)
        {
            throw new System.NotImplementedException();
        }

        public void GainCurrency(string currencyId, int currencyAmount)
        {
            throw new System.NotImplementedException();
        }

        public void DeductCurrency(string currencyId, int currencyAmount)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckEnoughCurrency(string currencyId, int currencyAmount)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<PlayerBalance> GetCurrencies()
        {
            throw new System.NotImplementedException();
        }
    }
}