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
        const string m_MoveCurrency = "MOVE";

        public void Init()
        {
            Debug.Log("Load currencies...");
            MainUI.Instance.OnRemainStep.Invoke(m_Currencies.Find(t => t.CurrencyId == m_MoveCurrency).Balance);
        }

        public List<PlayerBalance> GetData()
        {
            return m_Currencies;
        }

        public void SetData(List<PlayerBalance> currencies)
        {
            m_Currencies = currencies;
        }
    }
}