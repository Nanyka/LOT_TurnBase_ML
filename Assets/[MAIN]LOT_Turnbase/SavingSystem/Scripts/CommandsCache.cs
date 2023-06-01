using System.Collections.Generic;

namespace JumpeeIsland
{
    [System.Serializable]
    public class CommandsCache
    {
        public List<JICommand> commandBatch = new();
        public long timestamp;
    }
}