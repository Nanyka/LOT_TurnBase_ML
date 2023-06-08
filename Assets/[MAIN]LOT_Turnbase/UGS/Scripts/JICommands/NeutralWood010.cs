using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class NeutralWood010 : JICommand
    {
        public CommandName key = CommandName.JI_NEUTRAL_WOOD_1_0;

        public override void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager) { }

        public override void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager, Vector3 fromPos)
        {
            commandBatchSystem.EnqueueCommand(this);
            ProcessCommandLocally(remoteConfigManager, fromPos);
        }

        public override CommandName GetKey()
        {
            return key;
        }

        void ProcessCommandLocally(JIRemoteConfigManager remoteConfigManager, Vector3 fromPos)
        {
            var rewards = remoteConfigManager.commandRewards[GetKey().ToString()];
            Debug.Log("Processing collect one neutralWood");
            DistributeRewardsLocally(rewards,fromPos);
            
            // Check available stock from BuildingManager
            
            // Stock currencies from near to far
            
        }
    }
}
