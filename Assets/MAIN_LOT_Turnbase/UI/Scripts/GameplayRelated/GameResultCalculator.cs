using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    [RequireComponent(typeof(CollectedLoot))]
    public class GameResultCalculator : MonoBehaviour
    {
        [SerializeField] protected GameObject _winPanel;
        [SerializeField] protected GameObject _losePanel;
        [SerializeField] protected GameObject _stackHolder;
        [SerializeField] protected TextMeshProUGUI _winScoreText;
        [SerializeField] protected TextMeshProUGUI _loseScoreText;
        [SerializeField] protected TextMeshProUGUI _stackText;
        [SerializeField] protected List<StarHolder> _starHolders;
        [SerializeField] protected List<BattleRewardItem> _rewardItemUI;

        private CollectedLoot m_CollectedLoot;
        private int _startGameEnemyCount;

        protected virtual void Start()
        {
            MainUI.Instance.OnEnableInteract.AddListener(BattleStatsCache);
            GameFlowManager.Instance.OnGameOver.AddListener(ShowGameOverPanel);
            GameFlowManager.Instance.GetEnvManager().OnChangeFaction.AddListener(CalculateWinStars);

            m_CollectedLoot = GetComponent<CollectedLoot>();
        }

        private void CalculateWinStars()
        {
            if (GameFlowManager.Instance.GetEnvManager().GetCurrFaction() == FactionType.Enemy)
                UpdateStarGuide();
        }

        private void UpdateStarGuide()
        {
            // Main hall demolished condition
            var mainHallDemolished = SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall();
            MainUI.Instance.OnStarGuide.Invoke(0, "Destroy MAIN HALL", mainHallDemolished);

            // Win rate condition
            var enemyBuildingCount = SavingSystemManager.Instance.GetEnvironmentData().CountEnemyBuilding(FactionType.Enemy);
            if (enemyBuildingCount == 0)
                GameFlowManager.Instance.OnGameOver.Invoke(2);

            var winRate = (_startGameEnemyCount - enemyBuildingCount) * 1f / _startGameEnemyCount;
            MainUI.Instance.OnStarGuide.Invoke(1, $"Destroy 50% buildings", winRate > 0.5f);
            MainUI.Instance.OnStarGuide.Invoke(2, $"Destroy ALL buildings", Math.Abs(winRate - 1f) < Mathf.Epsilon);
        }

        private void BattleStatsCache()
        {
            _startGameEnemyCount = SavingSystemManager.Instance.GetEnvironmentData().CountEnemyBuilding(FactionType.Enemy);
            UpdateStarGuide();
        }

        protected virtual async void ShowGameOverPanel(int delayInterval)
        {
            //TODO: calculate first, show UI after
            
            await Task.Delay(delayInterval);
            
            Debug.Log("Show game over panel");
            var enemyBuildingCount = SavingSystemManager.Instance.GetEnvironmentData().CountEnemyBuilding(FactionType.Enemy);
            int winStar = 0;
            int score = 0;

            // +1star for demolishing enemy's mainHall
            if (SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall())
                winStar++;

            var winRate = (_startGameEnemyCount - enemyBuildingCount) * 1f / _startGameEnemyCount;

            // +1star for devastating 50% enemy's tribe
            if (winRate > 0.5f)
                winStar++;

            // +1star for devastating 100% enemy's tribe
            if (Math.Abs(winRate - 1f) < Mathf.Epsilon)
                winStar++;

            Debug.Log($"Win rate: {winRate}, Demolishing mainHall: {SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall()}, star count: {winStar}");

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

                // Add collected lot from CollectedLoot
                var collectedLoot = m_CollectedLoot.GetCollectedLoot();
                foreach (var item in collectedLoot)
                {
                    rewardDictionary.TryAdd(item.Key, 0);
                    rewardDictionary[item.Key] += item.Value;
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

                    SavingSystemManager.Instance.GetEnvDataForSave().GatherCreature(creatureLoot);
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
                _winPanel.SetActive(true);
                _winScoreText.text = score.ToString();
            }
            else
            {
                score = -1 * Mathf.RoundToInt(Random.Range(winRate, 1f) * 10);
                _stackHolder.SetActive(false);
                _losePanel.SetActive(true);
                _loseScoreText.text = score.ToString();
            }

            // Save battle statistic & Record score
            SavingSystemManager.Instance.SaveBattleResult(winStar, score, winRate);
        }
    }
}