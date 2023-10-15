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

        // Update cloud currency based on local currency
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

        public void RefreshCurrencies(List<PlayerBalance> currencies)
        {
            RefreshLocalBalances();
            Init(currencies);
        }

        public void GrantMove(long moveAmount)
        {
            Debug.Log($"Grant {moveAmount} MOVE");
            
            if (moveAmount <= 0)
                return;

            var moveCurrency = m_Currencies.Find(t => t.CurrencyId == m_MoveId);
            moveAmount = Mathf.Clamp((int)(moveCurrency.Balance + moveAmount), 0,
                SavingSystemManager.Instance.GetMaxMove());

            moveCurrency.Balance = moveAmount;
            SavingSystemManager.Instance.OnSetCloudCurrency(m_MoveId, (int)moveCurrency.Balance);
            SavingSystemManager.Instance.SaveLocalBalances(BalanceForSaving());
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

        public void ResetCurrencies(List<PlayerBalance> currencies)
        {
            m_Currencies = currencies;
            SavingSystemManager.Instance.SaveLocalBalances(BalanceForSaving());
            MainUI.Instance.OnUpdateCurrencies.Invoke();
        }

        // Update local currencies to have it match with cloud data when the command is still not flushed up
        public void IncrementCurrency(string currencyId, int currencyAmount)
        {
            var currency = m_Currencies.Find(t => t.CurrencyId == currencyId);
            currency.Balance += currencyAmount;
            MainUI.Instance.OnUpdateCurrencies.Invoke();
            
            // Update step from environmentManager
            if (currencyId.Equals(m_MoveId) && currencyAmount>0)
                GameFlowManager.Instance.GetEnvManager().UpdateRemainStep((int)currency.Balance);

            //Save currency
            SavingSystemManager.Instance.SaveLocalBalances(BalanceForSaving());
        }

        public void DeductCurrency(string currencyId, int currencyAmount)
        {
            var currency = m_Currencies.Find(t => t.CurrencyId == currencyId);
            if (currency != null)
                currency.Balance -= currencyAmount;
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
            RefreshLocalBalances();
            return m_LocalBalances;
        }

        private void RefreshLocalBalances()
        {
            if (m_LocalBalances == null)
                m_LocalBalances = new LocalBalancesData();

            m_LocalBalances.LocalBalances.Clear();

            foreach (var currency in m_Currencies)
                m_LocalBalances.LocalBalances.Add(new LocalBalance(currency.CurrencyId, (int)currency.Balance));
        }
    }
}