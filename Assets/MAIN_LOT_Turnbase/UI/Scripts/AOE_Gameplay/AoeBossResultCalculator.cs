using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class AoeBossResultCalculator : MonoBehaviour
    {
        [SerializeField] private int m_BossIndex;

        [SerializeField]
        private int _unlockedMap; // the player will be moved into this unlocked map after defeated the boss

        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;

        private int _primaryCoinAmount;

        protected virtual void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(RecordPrimaryState);
            MainUI.Instance.OnUpdateCurrencies.AddListener(UpdateProfit);
            MainUI.Instance.OnUpdateResult.AddListener(UpdateWinCondition);
        }

        private void RecordPrimaryState()
        {
            var coinCurrency = SavingSystemManager.Instance.GetCurrencyById("COIN");
            _primaryCoinAmount = coinCurrency;
            MainUI.Instance.OnBossMapProfitUpdate.Invoke((coinCurrency - _primaryCoinAmount) * 1f / _primaryCoinAmount);
        }

        private void UpdateProfit()
        {
            if (_primaryCoinAmount == 0)
                return;

            var coinCurrency = SavingSystemManager.Instance.GetCurrencyById("COIN");
            MainUI.Instance.OnBossMapProfitUpdate.Invoke((coinCurrency - _primaryCoinAmount) * 1f / _primaryCoinAmount);
        }

        private async void UpdateWinCondition()
        {
            await WaitToCalculate();

            // if (SavingSystemManager.Instance.GetEnvironmentData().EnemyData.Count == 0)
            // {
            //     // Defeat the boss to win the game
            //     Debug.Log($"Result: {SavingSystemManager.Instance.GetEnvironmentData().IsDefeatedBoss()}");
            //     if (SavingSystemManager.Instance.GetEnvironmentData().IsDefeatedBoss())
            //         await ShowKillBossPanel(2000);
            //     else
            //         await ShowLosePanel(2000);
            // }

            // TODO: Check to return the ecoMode when the player have no enough money to follow the battle
        }

        private async Task WaitToCalculate()
        {
            await Task.Delay(2000);

            if (SavingSystemManager.Instance.GetEnvironmentData().IsDefeatedBoss())
                ShowKillBossPanel();
            else
                ShowLosePanel();
        }

        private void ShowKillBossPanel()
        {
            _winPanel.SetActive(true);
            if (SavingSystemManager.Instance.GetGameProcess().bossUnlock <= m_BossIndex)
                SavingSystemManager.Instance.SaveBossUnlock(m_BossIndex + 1);
            SavingSystemManager.Instance.GetEnvDataForSave().mapSize = _unlockedMap; //TODO: Remove this and set the finish button to the current Map
            SavingSystemManager.Instance.SaveBossBattle();
        }

        private void ShowLosePanel()
        {
            _losePanel.SetActive(true);
        }
    }
}