using System.Collections.Generic;
using System.Linq;
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

            // Wood is used as MANA in the AoeBattleMode
            m_LocalBalances.LocalBalances.Find(t => t.CurrencyId == "WOOD").Balance = 0;
            
            // Grant an amount of COIN for TESTING
            m_LocalBalances.LocalBalances.Find(t => t.CurrencyId == "COIN").Balance = 100;

            MainUI.Instance.OnUpdateCurrencies.Invoke();
        }

        public long GetCurrency(string currencyId)
        {
            return m_LocalBalances.LocalBalances.First(t => t.CurrencyId == currencyId).Balance;
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
            m_LocalBalances.LocalBalances.Find(t => t.CurrencyId == currencyId).Balance += currencyAmount;
            MainUI.Instance.OnUpdateCurrencies.Invoke();
            SavingSystemManager.Instance.SaveLocalBalances(m_LocalBalances);
        }

        public void DeductCurrency(string currencyId, int currencyAmount)
        {
            m_LocalBalances.LocalBalances.Find(t => t.CurrencyId == currencyId).Balance -= currencyAmount;
            MainUI.Instance.OnUpdateCurrencies.Invoke();
            SavingSystemManager.Instance.SaveLocalBalances(m_LocalBalances);
        }

        public bool CheckEnoughCurrency(string currencyId, int currencyAmount)
        {
            return m_LocalBalances.LocalBalances.Find(t => t.CurrencyId == currencyId).Balance >= currencyAmount;
        }

        public IEnumerable<LocalBalance> GetCurrencies()
        {
            return m_LocalBalances.LocalBalances;
        }
    }
}