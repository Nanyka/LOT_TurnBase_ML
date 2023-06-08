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
        
        public abstract void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager, Vector3 fromPos);
        
        public abstract CommandName GetKey();

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
        
        // Use for currencies that require storing at building
        internal void DistributeRewardsLocally(List<JIRemoteConfigManager.Reward> rewards, Vector3 fromPos)
        {
            foreach (var reward in rewards)
            {
                switch (reward.service)
                {
                    case "currency":
                        SavingSystemManager.Instance.StoreCurrencyAtBuildings(reward.id, reward.amount, fromPos);
                        break;
                }
            }
        }
    }
}
