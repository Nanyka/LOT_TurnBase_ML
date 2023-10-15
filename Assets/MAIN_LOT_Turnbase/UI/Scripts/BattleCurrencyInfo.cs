using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class BattleCurrencyInfo : MonoBehaviour
    {
        [SerializeField] private List<CurrencyButton> _currencyButtons;

        private void Start()
        {
            MainUI.Instance.OnUpdateCurrencies.AddListener(Show);
        }

        private void Show()
        {
            Debug.Log("Update currency info");
            
            // Show enemy economy
            var woodAmount = 0;
            foreach (var buildingData in SavingSystemManager.Instance.GetEnvironmentData().BuildingData)
            {
                if (buildingData.StorageCurrency == CurrencyType.WOOD)
                    woodAmount += buildingData.CurrentStorage;
            }
            var woodbutton = _currencyButtons.Find(t => t.m_Currency.Equals("WOOD"));
            woodbutton.UpdateCurrency(woodAmount.ToString(), SavingSystemManager.Instance.GetCurrencySprite("WOOD"));

            var foodAmount = 0;
            foreach (var buildingData in SavingSystemManager.Instance.GetEnvironmentData().BuildingData)
            {
                if (buildingData.StorageCurrency == CurrencyType.FOOD)
                    foodAmount += buildingData.CurrentStorage;
            }
            var foodbutton = _currencyButtons.Find(t => t.m_Currency.Equals("FOOD"));
            foodbutton.UpdateCurrency(foodAmount.ToString(), SavingSystemManager.Instance.GetCurrencySprite("FOOD"));
            
            var coinAmount = 0;
            foreach (var buildingData in SavingSystemManager.Instance.GetEnvironmentData().BuildingData)
            {
                if (buildingData.StorageCurrency == CurrencyType.COIN)
                    coinAmount += buildingData.CurrentStorage;
            }
            var coinbutton = _currencyButtons.Find(t => t.m_Currency.Equals("COIN"));
            coinbutton.UpdateCurrency(coinAmount.ToString(), SavingSystemManager.Instance.GetCurrencySprite("COIN"));
        }
    }
}