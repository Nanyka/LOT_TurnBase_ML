using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class PopulationSelfUpdate : MonoBehaviour
    {
        [SerializeField] private CurrencyButton populationButton;
        [SerializeField] private string iconAddress;

        private void Start()
        {
            MainUI.Instance.OnUpdateCurrencies.AddListener(UpdatePopulation);
        }

        private void UpdatePopulation()
        {
            populationButton.UpdateCurrency(
                SavingSystemManager.Instance.GetEnvironmentData().GetTownhouseSpace().ToString(), iconAddress);
        }
    }
}