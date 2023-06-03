using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class SpendMove : JICommand
    {
        public new const string key = "JI_SPEND_MOVE";

        public override void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager)
        {
            commandBatchSystem.EnqueueCommand(this);
            ProcessCommandLocally(remoteConfigManager);
        }

        public override string GetKey()
        {
            return key;
        }

        void ProcessCommandLocally(JIRemoteConfigManager remoteConfigManager)
        {
            var rewards = remoteConfigManager.commandRewards[key];
            Debug.Log("Processing spend one MOVE");
            // GameStateManager.instance.SetIsOpenChestValidMove(true);
        }
    }
}