using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class ReplayResultCalculator : GameResultCalculator
    {
        protected override void Start()
        {
            GameFlowManager.Instance.OnGameOver.AddListener(ShowGameOverPanel);
        }
        
        protected override async void ShowGameOverPanel(int delayInterval)
        {
            await Task.Delay(delayInterval);

            var battleRecord = FindObjectOfType<ReplaySavingManager>().GetBattleRecord();

            var score = battleRecord.score;
            var winStar = battleRecord.winStar;
            var winStack = battleRecord.winStack;
            var winRate = battleRecord.winRate;
            var rewardDictionary = battleRecord.rewards;

            Debug.Log($"Win rate: {winRate}, Demolishing mainHall: {SavingSystemManager.Instance.GetEnvironmentData().IsDemolishMainHall()}, star count: {winStar}");

            if (winStar > 0)
            {
                // Record battle loot right after get amount of stars

                var battleLoot = await SavingSystemManager.Instance.GetBattleLootByStar(winStar);
                
                int currentRewardItemUI = 0;
                foreach (var rewardItem in rewardDictionary)
                {
                    // Show rewardItemUI and get Stats from Translator
                    var rewardItemUI = _rewardItemUI[currentRewardItemUI];
                    rewardItemUI.ShowReward(SavingSystemManager.Instance.GetCurrencySprite(rewardItem.currencyId),
                        rewardItem.amount.ToString(),
                        rewardItem.currencyId == CurrencyType.GEM.ToString() ||
                        rewardItem.currencyId == CurrencyType.GOLD.ToString());
                    rewardItemUI.gameObject.SetActive(true);
                    currentRewardItemUI++;
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
                }

                for (int i = 0; i < winStar; i++)
                    _starHolders[i].EnableStar();

                if (winStack > 0)
                {
                    _stackText.text = winStack.ToString();
                    _stackHolder.SetActive(true);
                }
                else
                    _stackHolder.SetActive(false);

                _winPanel.SetActive(true);
                _winScoreText.text = score.ToString();
            }
            else
            {
                _stackHolder.SetActive(false);
                _losePanel.SetActive(true);
                _loseScoreText.text = score.ToString();
            }
        }
    }
}