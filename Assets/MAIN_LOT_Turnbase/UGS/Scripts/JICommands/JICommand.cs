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

        public abstract void ProcessCommandLocally(JIRemoteConfigManager remoteConfigManager);

        // Use for currencies that do not require storing at building
        internal void DistributeRewardsLocally(List<JIRemoteConfigManager.Reward> rewards)
        {
            foreach (var reward in rewards)
            {
                switch (reward.service)
                {
                    case "currency":
                        SavingSystemManager.Instance.IncrementLocalCurrency(reward.id, reward.amount);
                        break;
                }
            }
        }
    }
}
