using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class AoeResultCalculator : MonoBehaviour
    {
        [SerializeField] protected GameObject _winPanel;
        [SerializeField] protected GameObject _losePanel;
        [SerializeField] protected GameObject _stackHolder;
        [SerializeField] protected TextMeshProUGUI _winScoreText;
        [SerializeField] protected TextMeshProUGUI _loseScoreText;
        [SerializeField] protected TextMeshProUGUI _stackText;
        [SerializeField] protected List<StarHolder> _starHolders;
        [SerializeField] protected List<BattleRewardItem> _rewardItemUI;

        private int _startGameEnemyCount;
        private int _primaryCoinAmount;
        private int _maxDeficit;
        private float _profit;

        protected virtual void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(BattleStatsCache);
            MainUI.Instance.OnUpdateCurrencies.AddListener(UpdateProfit);
            // MainUI.Instance.OnUpdateResult.AddListener(UpdateStarGuide);
            GameFlowManager.Instance.OnGameOver.AddListener(ShowGameOverPanel);
        }

        private void BattleStatsCache()
        {
            // _startGameEnemyCount = SavingSystemManager.Instance.GetEnvironmentData().CountEnemyBuilding(FactionType.Enemy);

            var coinCurrency = SavingSystemManager.Instance.GetCurrencyById("COIN");
            _primaryCoinAmount = coinCurrency;
            UpdateProfit();
        }

        private void UpdateProfit()
        {
            if (_primaryCoinAmount == 0)
                return;

            var coinCurrency = SavingSystemManager.Instance.GetCurrencyById("COIN");
            var coinChange = coinCurrency - _primaryCoinAmount;
            if (coinChange < _maxDeficit)
                _maxDeficit = coinChange;

            _profit = _maxDeficit == 0 ? 0 : coinChange * 1f / Mathf.Abs(_maxDeficit);

            MainUI.Instance.OnProfitUpdate.Invoke(_profit);
            UpdateStarGuide();
        }

        private void UpdateStarGuide()
        {
            // Main hall demolished condition
            var mainHallDemolished = SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall();
            MainUI.Instance.OnStarGuide.Invoke(0, "Destroy MAIN HALL", mainHallDemolished);

            // Profit condition
            MainUI.Instance.OnStarGuide.Invoke(1, $"Reach through break even point", _profit > 0f);
            MainUI.Instance.OnStarGuide.Invoke(2, $"Earning rate more than 100%", _profit > 1f);

            // Win condition
            var enemyBuildingCount =
                SavingSystemManager.Instance.GetEnvironmentData().CountEnemyBuilding(FactionType.Enemy);

            if (enemyBuildingCount == 0)
                GameFlowManager.Instance.OnGameOver.Invoke(2);

            // // Win rate condition
            // var enemyBuildingCount =
            //     SavingSystemManager.Instance.GetEnvironmentData().CountEnemyBuilding(FactionType.Enemy);
            //
            // if (enemyBuildingCount == 0)
            //     GameFlowManager.Instance.OnGameOver.Invoke(2);
            //
            // var winRate = (_startGameEnemyCount - enemyBuildingCount) * 1f / _startGameEnemyCount;
            // MainUI.Instance.OnStarGuide.Invoke(1, $"Destroy 50% buildings", winRate > 0.5f);
            // MainUI.Instance.OnStarGuide.Invoke(2, $"Destroy ALL buildings", Math.Abs(winRate - 1f) < Mathf.Epsilon);
        }

        private async void ShowGameOverPanel(int delayInterval)
        {
            //TODO: calculate first, show UI after

            await Task.Delay(delayInterval);

            Debug.Log("Show game over panel");
            var enemyBuildingCount =
                SavingSystemManager.Instance.GetEnvironmentData().CountEnemyBuilding(FactionType.Enemy);
            int winStar = 0;
            int score = 0;

            // +1star for demolishing enemy's mainHall
            if (SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall())
                winStar++;

            // +1star for a profitable raid
            if (_profit > 0f)
                winStar++;

            // +1star for unbelievable profit making
            if (_profit > 1f)
                winStar++;

            // // +1star for demolishing enemy's mainHall
            // if (SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall())
            //     winStar++;
            //
            // var winRate = (_startGameEnemyCount - enemyBuildingCount) * 1f / _startGameEnemyCount;
            //
            // // +1star for devastating 50% enemy's tribe
            // if (winRate > 0.5f)
            //     winStar++;
            //
            // // +1star for devastating 100% enemy's tribe
            // if (Math.Abs(winRate - 1f) < Mathf.Epsilon)
            //     winStar++;
            // Debug.Log($"Win rate: {winRate}, Demolishing mainHall: {SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall()}, star count: {winStar}");

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
                    // SavingSystemManager.Instance.StoreCurrencyByEnvData(rewardItem.Key, rewardItem.Value,
                    //     SavingSystemManager.Instance.GetEnvDataForSave());
                }

                // 10% player have a chance to gather a creature
                if (battleLoot.CreatureLoot.Count > 0 && Random.Range(0, 100) < 10)
                {
                    var creatureLoot = battleLoot.CreatureLoot[Random.Range(0, battleLoot.CreatureLoot.Count)];
                    var creatureData = SavingSystemManager.Instance.GetInventoryItemByName(creatureLoot);

                    // Show rewardItemUI and get Stats from Translator
                    var rewardItemUI = _rewardItemUI[currentRewardItemUI];
                    rewardItemUI.ShowReward(creatureData.spriteAddress, "1", true);
                    rewardItemUI.gameObject.SetActive(true);

                    // TODO: Stock the creature in a separated storage
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
                score = Mathf.RoundToInt(_profit * 100);
                _stackHolder.SetActive(false);
                _losePanel.SetActive(true);
                _loseScoreText.text = score.ToString();
            }

            // Save battle statistic & Record score
            SavingSystemManager.Instance.SaveBattleResult(SavingSystemManager.Instance.GetEnemyId(), winStar, score, _profit);
        }
    }
}