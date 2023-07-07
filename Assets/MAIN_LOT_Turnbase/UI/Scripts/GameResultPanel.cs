using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class GameResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _gameoverPanel;
        [SerializeField] private TextMeshProUGUI _gameoverText;

        private int _startGameEnemyCount;

        private void Start()
        {
            MainUI.Instance.OnEnableInteract.AddListener(BattleStatsCache);
            MainUI.Instance.OnGameOver.AddListener(ShowGameOverPanel);
        }

        private void BattleStatsCache()
        {
            _startGameEnemyCount = GameFlowManager.Instance.GetEnvManager().CountFaction(FactionType.Enemy);
            Debug.Log($"Start amount of {FactionType.Enemy}: {_startGameEnemyCount}");
        }

        private void ShowGameOverPanel()
        {
            var playerCount = GameFlowManager.Instance.GetEnvManager().CountFaction(FactionType.Player);
            var enemyCount = GameFlowManager.Instance.GetEnvManager().CountFaction(FactionType.Enemy);
            var winFaction = playerCount > enemyCount ? FactionType.Player : FactionType.Enemy;
            
            //TODO: Calculate WIN stars and reward for winner
            int winStar = 0;

            // +1star for demolishing enemy's mainHall
            if (SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall())
                winStar++;

            var winRate = (enemyCount - _startGameEnemyCount) * 1f / _startGameEnemyCount;

            // +1star for devastating 50% enemy's tribe
            if (winRate > 0.5f)
                winStar++;

            // +1star for devastating 100% enemy's tribe
            if (winRate == 0f)
                winStar++;

            _gameoverText.text = $"Faction {winFaction} WIN {winStar} STARs.\n Win rate {winRate*100}%";
            _gameoverPanel.SetActive(true);
        }
    }
}