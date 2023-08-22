using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class GameResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _gameoverPanel;
        [SerializeField] private GameObject _stackHolder;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _stackText;
        [SerializeField] private List<StarHolder> _starHolders;
        [SerializeField] private List<BattleRewardItem> _rewardItemUI;

        private int _startGameEnemyCount;

        private void Start()
        {
            MainUI.Instance.OnEnableInteract.AddListener(BattleStatsCache);
            GameFlowManager.Instance.OnGameOver.AddListener(ShowGameOverPanel);
        }

        private void BattleStatsCache()
        {
            _startGameEnemyCount =
                SavingSystemManager.Instance.GetEnvironmentData().CountEnemyBuilding(FactionType.Enemy);
        }

        private async void ShowGameOverPanel()
        {
            var enemyBuildingCount =
                SavingSystemManager.Instance.GetEnvironmentData().CountEnemyBuilding(FactionType.Enemy);
            int winStar = 0;
            int score = 0;

            // +1star for demolishing enemy's mainHall
            if (SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall())
                winStar++;

            var winRate = (enemyBuildingCount - _startGameEnemyCount) * 1f / _startGameEnemyCount;

            // +1star for devastating 50% enemy's tribe
            if (winRate > 0.5f)
                winStar++;

            // +1star for devastating 100% enemy's tribe
            if (Math.Abs(winRate - 1f) < Mathf.Epsilon)
                winStar++;

            Debug.Log(
                $"Win rate: {winRate}, Demolishing mainHall: {SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall()}, star count: {winStar}");

            if (winStar > 0)
            {
                // Record battle loot right after get amount of stars

                var battleLoot = await SavingSystemManager.Instance.GetBattleLootByStar(winStar);

                // Get reward data to show off on resultPanel
                var rewardDictionary = new Dictionary<string, int>();
                var amountOfCommands = Random.Range(2, 4);
                for (int i = 0; i < amountOfCommands; i++)
                {
                    var rewards =
                        SavingSystemManager.Instance.GetRewardByCommand(
                            battleLoot.CurrencyLoots[Random.Range(0, battleLoot.CurrencyLoots.Count)]);
                    foreach (var reward in rewards)
                    {
                        if (rewardDictionary.ContainsKey(reward.id) == false)
                            rewardDictionary.Add(reward.id, reward.amount);
                        else
                            rewardDictionary[reward.id] += reward.amount;
                    }
                }

                int currentRewardItemUI = 0;
                foreach (var rewardItem in rewardDictionary)
                {
                    // Show rewardItemUI and get Stats from Translator
                    var rewardItemUI = _rewardItemUI[currentRewardItemUI];
                    rewardItemUI.ShowReward(SavingSystemManager.Instance.GetCurrencySprite(rewardItem.Key),
                        rewardItem.Value.ToString(),
                        rewardItem.Key == CurrencyType.GEM.ToString() ||
                        rewardItem.Key == CurrencyType.GOLD.ToString());
                    rewardItemUI.gameObject.SetActive(true);
                    currentRewardItemUI++;

                    // Store currencies to buildings of player's EnvData
                    SavingSystemManager.Instance.StoreCurrencyByEnvData(rewardItem.Key, rewardItem.Value,
                        SavingSystemManager.Instance.GetEnvDataForSave());
                }

                // 10% player have a chance to gather a creature
                if (battleLoot.CreatureLoot.Count > 0 && Random.Range(0, 100) < 100)
                {
                    var creatureLoot = battleLoot.CreatureLoot[Random.Range(0, battleLoot.CreatureLoot.Count)];
                    var creatureData = SavingSystemManager.Instance.GetInventoryItemByName(creatureLoot);

                    // Show rewardItemUI and get Stats from Translator
                    var rewardItemUI = _rewardItemUI[currentRewardItemUI];
                    rewardItemUI.ShowReward(creatureData.spriteAddress, "1", true);
                    rewardItemUI.gameObject.SetActive(true);

                    SavingSystemManager.Instance.GetEnvironmentData().GatherCreature(creatureLoot);
                }

                for (int i = 0; i < winStar; i++)
                    _starHolders[i].EnableStar();

                var gameProcess = SavingSystemManager.Instance.GetGameProcess();
                if (gameProcess.winStack > 0)
                {
                    gameProcess.winStack++;
                    _stackText.text = gameProcess.winStack.ToString();
                    _stackHolder.SetActive(true);
                }
                else
                    _stackHolder.SetActive(false);

                score = Mathf.RoundToInt(Random.Range(0, gameProcess.winStack > 0 ? 1 : 0 + winStar) * 10);
                _gameoverPanel.SetActive(true);
            }
            else
            {
                score = -1 * Mathf.RoundToInt(Random.Range(winRate, 1f) * 10);
                _stackHolder.SetActive(false);
                _gameoverPanel.SetActive(true);
            }

            _scoreText.text = score.ToString();

            // Save battle statistic & Record score
            SavingSystemManager.Instance.SaveBattleResult(winStar, score);
        }
    }
}