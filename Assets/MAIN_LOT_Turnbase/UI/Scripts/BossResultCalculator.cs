using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class BossResultCalculator : MonoBehaviour
    {
        [SerializeField] private int m_BossIndex;
        [SerializeField] private GameObject _killedPossPanel;
        [SerializeField] private GameObject _winStagePanel;
        [SerializeField] private GameObject _losePanel;
        [SerializeField] private List<StarHolder> _starHolders;
        [SerializeField] private List<BattleRewardItem> _rewardItems;

        private CountDownSteps _countDownSteps;
        private int _starCount;
        private bool _isEndGame;

        private void Start()
        {
            GameFlowManager.Instance.OnGameOver.AddListener(ShowWinStagePanel);
            GameFlowManager.Instance.OnKilledBoss.AddListener(ShowKilledBossPanel);
            GameFlowManager.Instance.GetEnvManager().OnChangeFaction.AddListener(CalculateWinStars);

            _countDownSteps = GetComponent<CountDownSteps>();

            InitStarGuide();
        }

        private void InitStarGuide()
        {
            var quest = GameFlowManager.Instance.GetQuest();
            MainUI.Instance.OnStarGuide.Invoke(0, $"Complete in {quest.maxMovingTurn} steps",false);
            MainUI.Instance.OnStarGuide.Invoke(1, $"Complete in {quest.excellentRank[0]} steps",false);
            MainUI.Instance.OnStarGuide.Invoke(2, $"Complete in {quest.excellentRank[1]} steps",false);
        }

        private void CalculateWinStars()
        {
            if (GameFlowManager.Instance.GetEnvManager().GetCurrFaction() == FactionType.Enemy)
                UpdateStarGuide();
        }

        private void UpdateStarGuide()
        {
            var quest = GameFlowManager.Instance.GetQuest();
            if (quest.isFinalBoss == false)
            {
                MainUI.Instance.OnStarGuide.Invoke(0, $"Complete in {quest.maxMovingTurn} steps",
                    (quest.maxMovingTurn - _countDownSteps.GetRemainSteps()) >= quest.maxMovingTurn);
                MainUI.Instance.OnStarGuide.Invoke(1, $"Complete in {quest.excellentRank[0]} steps",
                    (quest.maxMovingTurn - _countDownSteps.GetRemainSteps()) > quest.excellentRank[0]);
                MainUI.Instance.OnStarGuide.Invoke(2, $"Complete in {quest.excellentRank[1]} steps",
                    (quest.maxMovingTurn - _countDownSteps.GetRemainSteps()) > quest.excellentRank[1]);
            }
        }

        private async void ShowWinStagePanel(int delayInvterval)
        {
            if (_isEndGame) return;
            _isEndGame = true;

            // just show Complete stage panel when it is not final boss stage
            var quest = GameFlowManager.Instance.GetQuest();
            if (quest.isFinalBoss)
                return;

            await WaitToShowWinStage(delayInvterval);
        }

        private async Task WaitToShowWinStage(int delayInvterval)
        {
            await Task.Delay(delayInvterval);

            var quest = GameFlowManager.Instance.GetQuest();
            if (quest.winCondition.CheckPass())
            {
                // Calculate stars
                var remainStep = GetComponent<CountDownSteps>().GetRemainSteps();
                if (remainStep >= 0)
                    _starCount++;
                if (remainStep > quest.excellentRank[0])
                    _starCount++;
                if (remainStep > quest.excellentRank[1])
                    _starCount++;

                for (int i = 0; i < _starCount; i++)
                    _starHolders[i].EnableStar();

                // Calculate reward
                int rewardIndex = 0;
                foreach (var reward in quest.rewards)
                {
                    var iconAddress = SavingSystemManager.Instance.GetCurrencySprite(reward.id);
                    _rewardItems[rewardIndex].ShowReward(iconAddress, reward.amount.ToString(), false);
                    SavingSystemManager.Instance.StoreCurrencyByEnvData(reward.id, reward.amount,
                        SavingSystemManager.Instance.GetEnvDataForSave());
                }

                // Save stage completion
                SavingSystemManager.Instance.SaveQuestData(m_BossIndex,
                    QuestFlowManager.Instance.GetQuestData().CurrentQuestAddress, _starCount);

                _winStagePanel.SetActive(true);
            }
            else
                _losePanel.SetActive(true);
        }

        private void ShowKilledBossPanel()
        {
            Debug.Log("Killed boss");
            var creatures = SavingSystemManager.Instance.GetEnvironmentData().EnemyData;

            int totalAmount = 0;
            foreach (var creature in creatures)
                if (creature.CreatureType == CreatureType.BOSS)
                    totalAmount++;

            if (totalAmount > 0)
                _losePanel.SetActive(true);
            else
            {
                _killedPossPanel.SetActive(true);
                if (SavingSystemManager.Instance.GetGameProcess().bossUnlock <= m_BossIndex)
                    SavingSystemManager.Instance.SaveBossUnlock(m_BossIndex + 1);
            }

            SavingSystemManager.Instance.SaveBossBattle();
        }
    }
}