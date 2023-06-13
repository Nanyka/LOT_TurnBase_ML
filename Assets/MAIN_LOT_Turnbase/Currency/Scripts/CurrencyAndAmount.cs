using System;
using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;

namespace JumpeeIsland
{
    [Serializable]
    public class CurrencyAndAmount
    {
        public CurrencyType currencyType;
        public int Amount;

        public CurrencyAndAmount(CurrencyType type, int amount)
        {
            currencyType = type;
            Amount = amount;
        }
    }
}
