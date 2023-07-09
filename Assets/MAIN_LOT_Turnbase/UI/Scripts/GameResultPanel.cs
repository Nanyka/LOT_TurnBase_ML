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
        [SerializeField] private List<BattleRewardItem> _rewardItemUI;

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

        private async void ShowGameOverPanel()
        {
            var enemyCount = GameFlowManager.Instance.GetEnvManager().CountFaction(FactionType.Enemy);
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
                        rewardItem.Key == CurrencyType.GEM.ToString() || rewardItem.Key == CurrencyType.GOLD.ToString());
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
                    rewardItemUI.ShowReward(creatureData.spriteAddress, "1",true);
                    rewardItemUI.gameObject.SetActive(true);
                    
                    SavingSystemManager.Instance.GetEnvironmentData().GatherCreature(creatureLoot);
                }

                _gameoverText.text = $"Player WIN {winStar} STARs.\n Win rate {winRate * 100}%";
                _gameoverPanel.SetActive(true);
            }
            else
            {
                _gameoverText.text = $"Player LOSS!!!";
                _gameoverPanel.SetActive(true);
            }
        }
    }
}