using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class SpendMove : JICommand
    {
        public CommandName key = CommandName.JI_SPEND_MOVE;

        public override void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager)
        {
            commandBatchSystem.EnqueueCommand(this);
            ProcessCommandLocally(remoteConfigManager);
        }

        public override void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager, Vector3 fromPos)
        {
            throw new System.NotImplementedException();
        }

        public override CommandName GetKey()
        {
            return key;
        }

        void ProcessCommandLocally(JIRemoteConfigManager remoteConfigManager)
        {
            var rewards = remoteConfigManager.commandRewards[GetKey().ToString()];
            Debug.Log("Processing spend one MOVE");
            DistributeRewardsLocally(rewards);
            // GameStateManager.instance.SetIsOpenChestValidMove(true);
        }
    }
}