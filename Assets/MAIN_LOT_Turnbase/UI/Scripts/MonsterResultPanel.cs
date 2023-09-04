using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class MonsterResultPanel : MonoBehaviour
    {
        [SerializeField] private int m_BossIndex;
        [SerializeField] private GameObject _killedPossPanel;
        [SerializeField] private GameObject _winStagePanel;
        [SerializeField] private GameObject _losePanel;
        [SerializeField] private List<BattleRewardItem> _rewardItems;

        private void Start()
        {
            GameFlowManager.Instance.OnGameOver.AddListener(ShowWinStagePanel);
            GameFlowManager.Instance.OnKilledBoss.AddListener(ShowKilledBossPanel);
        }

        private async void ShowWinStagePanel()
        {
            await WaitToShowWinStage();
        }

        private async Task WaitToShowWinStage()
        {
            await Task.Delay(2000);
            
            Debug.Log("Win this stage");
            
            var quest = GameFlowManager.Instance.GetQuest();
            if (quest.winCondition.CheckPass())
            {
                int rewardIndex = 0;
                foreach (var reward in quest.rewards)
                {
                    var iconAddress = SavingSystemManager.Instance.GetCurrencySprite(reward.id);
                    _rewardItems[rewardIndex].ShowReward(iconAddress,reward.amount.ToString(),false);
                    SavingSystemManager.Instance.StoreCurrencyByEnvData(reward.id, reward.amount,
                        SavingSystemManager.Instance.GetEnvDataForSave());
                }
                _winStagePanel.SetActive(true);
            }
            else
                _losePanel.SetActive(true);
        }

        private void ShowKilledBossPanel()
        {
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
                if (SavingSystemManager.Instance.GetGameProcess().bossUnlock < m_BossIndex)
                    SavingSystemManager.Instance.SaveBossUnlock(m_BossIndex);
            }
            SavingSystemManager.Instance.SaveBossBattle();
        }
    }
}