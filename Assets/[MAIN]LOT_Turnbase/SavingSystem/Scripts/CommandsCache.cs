using System.Collections.Generic;

namespace JumpeeIsland
{
    [System.Serializable]
    public class CommandsCache
    {
        public List<JICommand> commandBatch = new();
        public List<string> commandList = new();

        // Just translate when move through CloudManager
        public void CreateCommandList()
        {
            foreach (var command in commandBatch)
                commandList.Add(command.GetKey());

            commandBatch = new();
        }

        // Just translate when move through CloudManager
        public void ExecuteJICommands()
        {
            foreach (var commandString in commandList)
            {
                switch (commandString)
                {
                    case "JI_SPEND_MOVE":
                        SavingSystemManager.Instance.OnRestoreCommands.Invoke();
                        break;
                }
            }

            commandList = new();
        }
    }
}