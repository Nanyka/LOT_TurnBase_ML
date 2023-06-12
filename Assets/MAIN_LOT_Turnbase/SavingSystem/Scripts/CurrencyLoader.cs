using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Economy.Model;
using UnityEngine;

namespace JumpeeIsland
{
    public class CurrencyLoader : MonoBehaviour
    {
        private List<PlayerBalance> m_Currencies;
        private string m_MoveId = "MOVE";

        public void Init()
        {
            Debug.Log("Load currencies...");
        }

        private long GetCurrency(string currencyId)
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
        public void IncrementCurrency(string rewardID, int rewardAmount)
        {
            m_Currencies.Find(t => t.CurrencyId == rewardID).Balance += rewardAmount;
            MainUI.Instance.OnUpdateCurrencies.Invoke();
        }
    }
}