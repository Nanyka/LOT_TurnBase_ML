using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Economy.Model;
using UnityEngine;

namespace JumpeeIsland
{
    [Serializable]
    public class LocalBalance
    {
        public string CurrencyId;
        public int Balance;

        public LocalBalance(string currencyId, int balance)
        {
            CurrencyId = currencyId;
            Balance = balance;
        }
    }

    public class LocalBalancesData
    {
        public List<LocalBalance> LocalBalances = new();
    }

    public class CurrencyLoader : MonoBehaviour
    {
        private List<PlayerBalance> m_Currencies;
        private LocalBalancesData m_LocalBalances;
        private string m_MoveId = "MOVE";

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
                if (localBalance.CurrencyId == "MOVE") // MOVE is a special currency that is controlled by cloudCode only
                    continue;

                var currency = m_Currencies.Find(t => t.CurrencyId == localBalance.CurrencyId);
                if (localBalance.Balance != currency.Balance)
                {
                    Debug.Log($"{localBalance.CurrencyId} change {localBalance.Balance - currency.Balance}");
                    currency.Balance = localBalance.Balance;
                    SavingSystemManager.Instance.OnSetCurrency(localBalance.CurrencyId,localBalance.Balance);
                }
            }

            MainUI.Instance.OnUpdateCurrencies.Invoke();
        }

        public long GetCurrency(string currencyId)
        {
            return m_Currencies.Find(t => t.CurrencyId == currencyId).Balance;
        }

        public IEnumerable<PlayerBalance> GetCurrencies()
        {
            return m_Currencies;
        }

        public long GetMoveAmount()
        {
            return GetCurrency(m_MoveId);
        }

        public void SetData(List<PlayerBalance> currencies)
        {
            m_Currencies = currencies;
            MainUI.Instance.OnUpdateCurrencies.Invoke();
        }

        // Update local currencies to have it match with cloud data when the command is still not flushed up
        public void IncrementCurrency(string currencyId, int currencyAmount)
        {
            m_Currencies.Find(t => t.CurrencyId == currencyId).Balance += currencyAmount;
            MainUI.Instance.OnUpdateCurrencies.Invoke();

            //Save currency
            SavingSystemManager.Instance.SaveLocalBalances(BalanceForSaving());
        }

        public void DeductCurrency(string currencyId, int currencyAmount)
        {
            m_Currencies.Find(t => t.CurrencyId == currencyId).Balance -= currencyAmount;
            MainUI.Instance.OnUpdateCurrencies.Invoke();

            //Save currency 
            SavingSystemManager.Instance.SaveLocalBalances(BalanceForSaving());
        }

        // Check amount of a particular currency is affordable for a purchase or not
        public bool CheckEnoughCurrency(string currencyId, int currencyAmount)
        {
            return m_Currencies.Find(t => t.CurrencyId == currencyId).Balance >= currencyAmount;
        }

        private LocalBalancesData BalanceForSaving()
        {
            var localBalances = new LocalBalancesData();

            foreach (var currency in m_Currencies)
                localBalances.LocalBalances.Add(new LocalBalance(currency.CurrencyId, (int)currency.Balance));
            return localBalances;
        }
    }
}