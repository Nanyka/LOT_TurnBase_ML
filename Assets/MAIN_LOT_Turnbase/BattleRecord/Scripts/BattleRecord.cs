using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [Serializable]
    public class BattleRecord
    {
        public bool isRecorded;
        public int score;
        public int winStar;
        public int winStack;
        public float winRate;
        public List<CurrencyUnit> rewards;
        public EnvironmentData environmentData;
        public List<RecordAction> actions = new();

        public BattleRecord()
        {
            isRecorded = false;
        }
    }

    [Serializable]
    public class RecordAction
    {
        public Vector3 AtPos;
        public int Action;
        // public float AtSecond;
        public EntityType EntityType;
    }

    [Serializable]
    public class CurrencyUnit
    {
        public string currencyId;
        public int amount;
    }
}
