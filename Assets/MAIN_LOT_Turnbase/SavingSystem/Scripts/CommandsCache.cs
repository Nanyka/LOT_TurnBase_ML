using System.Collections.Generic;

namespace JumpeeIsland
{
    [System.Serializable]
    public class CommandsCache
    {
        public List<JICommand> commandBatch = new();
        public List<CommandName> commandList = new();

        // Just translate when move through CloudManager
        public void CreateCommandList()
        {
            foreach (var command in commandBatch)
                commandList.Add(command.GetKey());

            commandBatch = new();
        }
    }
}