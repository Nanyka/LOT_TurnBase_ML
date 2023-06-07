using System.Collections;
using System.Collections.Generic;
using Unity.Services.Samples.CommandBatching;
using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public abstract class JICommand
    {
        // public const string key = "";

        public abstract void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager);
        public abstract CommandName GetKey();

        internal void DistributeRewardsLocally(List<JIRemoteConfigManager.Reward> rewards)
        {
            foreach (var reward in rewards)
            {
                switch (reward.service)
                {
                    case "currency":
                        SavingSystemManager.Instance.IncrementLocalCurrency(reward.id, reward.amount);
                        break;
        
                    // case "cloudSave":
                    //     switch (reward.id)
                    //     {
                    //         case CloudSaveManager.xpKey:
                    //             GameStateManager.instance.xp += reward.amount;
                    //             break;
                    //
                    //         case CloudSaveManager.goalsAchievedKey:
                    //             GameStateManager.instance.goalsAchieved += reward.amount;
                    //             break;
                    //
                    //         default:
                    //             Debug.Log($"No local disbursement action exists for the reward {reward.id}");
                    //             break;
                    //     }
                    //
                    //     break;
                }
            }
        }
    }
}
