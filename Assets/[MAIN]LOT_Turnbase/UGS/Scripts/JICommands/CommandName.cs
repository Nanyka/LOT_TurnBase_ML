namespace JumpeeIsland
{
    // Must also create new JICommand and update at JIRemoteConfigManager, JICloudConnector, CloudCode.JumpeeIsland_ProcessBatch()
    public enum CommandName
    {
        NONE,
        JI_SPEND_MOVE,
        JI_NEUTRAL_WOOD_1_0,
        JI_NEUTRAL_FOOD_1_0
    }
}