namespace JumpeeIsland
{
    public class Wood5 : JICommand
    {
        private CommandName key = CommandName.JI_WOOD_5;

        public override void Execute(JICommandBatchSystem commandBatchSystem, JIRemoteConfigManager remoteConfigManager)
        {
            commandBatchSystem.EnqueueCommand(this);
            ProcessCommandLocally(remoteConfigManager);
        }

        public override CommandName GetKey()
        {
            return key;
        }

        public override void ProcessCommandLocally(JIRemoteConfigManager remoteConfigManager)
        {
            var rewards = remoteConfigManager.commandRewards[GetKey().ToString()];
            DistributeRewardsLocally(rewards);
        }
    }
}