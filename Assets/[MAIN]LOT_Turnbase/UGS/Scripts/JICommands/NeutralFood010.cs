using UnityEngine;

namespace JumpeeIsland
{
    public class NeutralFood010 : JICommand
    {
        public CommandName key = CommandName.JI_NEUTRAL_FOOD_1_0;

        public override void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager)
        {
            throw new System.NotImplementedException();
        }

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
            // GameStateManager.instance.SetIsOpenChestValidMove(true);
        }
    }
}