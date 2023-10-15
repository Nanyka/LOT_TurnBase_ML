using System;
using System.Collections.Generic;

namespace JumpeeIsland
{    
    [Serializable]
    public class QuestData
    {
        public List<QuestChain> QuestChains;
        public string CurrentQuestAddress;
    }
}