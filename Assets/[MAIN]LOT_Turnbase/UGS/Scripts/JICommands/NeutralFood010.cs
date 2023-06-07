using UnityEngine;

namespace JumpeeIsland
{
    public class NeutralFood010 : JICommand
    {
        public CommandName key = CommandName.JI_NEUTRAL_FOOD_1_0;

        public override void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager)
        {
            commandBatchSystem.EnqueueCommand(this);
            ProcessCommandLocally(remoteConfigManager);
        }

        public override CommandName GetKey()
        {
            return key;
        }

        void ProcessCommandLocally(JIRemoteConfigManager remoteConfigManager)
        {
            var rewards = remoteConfigManager.commandRewards[GetKey().ToString()];
            Debug.Log("Processing collect one neutralWood");
            DistributeRewardsLocally(rewards);
            // GameStateManager.instance.SetIsOpenChestValidMove(true);
        }
    }
}