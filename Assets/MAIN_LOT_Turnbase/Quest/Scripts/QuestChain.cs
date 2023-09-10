using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [Serializable]
    public class QuestChain
    {
        public List<QuestUnit> QuestUnits;
    }

    [Serializable]
    public class QuestUnit
    {
        public string QuestAddress;
        public int StarAmount;

        public QuestUnit(string questAddress)
        {
            QuestAddress = questAddress;
        }
    }
}