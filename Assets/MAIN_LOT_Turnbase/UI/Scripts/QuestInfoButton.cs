using UnityEngine;

namespace JumpeeIsland
{
    public class QuestInfoButton : MonoBehaviour
    {
        public void OnClickButton()
        {
            var quest = GameFlowManager.Instance.GetQuest();
            if (quest.isFinalBoss)
            {
                MainUI.Instance.OnConversationUI.Invoke($"Defeat the boss in {quest.maxMovingTurn} steps to UNLOCK NEW CHARACTER", true);
            }
            else
            {
                var threeStarSteps = quest.maxMovingTurn - quest.excellentRank[1];
                MainUI.Instance.OnConversationUI.Invoke($"Complete in {threeStarSteps} step to get 3 stars", true);
            }
        }
    }
}