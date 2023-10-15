using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

namespace JumpeeIsland
{
    public class JICustomEventSender : MonoBehaviour
    {
        public void SendBossQuestEvent(int playerScore, int bossId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "playerScore", playerScore },
                { "bossId", bossId }
            };

            AnalyticsService.Instance.CustomData("startedBoss1Quest", parameters);
        }

        public void SendTutorialTrackEvent(string stepId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "tutorialStepId", stepId }
            };

            AnalyticsService.Instance.CustomData("tutorialStepCompleted", parameters);
        }
    }
}