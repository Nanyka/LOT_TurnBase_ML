using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [Serializable]
    public class BattleRecord
    {
        public bool IsRecorded;
        public int TestInt;

        public BattleRecord()
        {
            IsRecorded = false;
        }
    }
}
