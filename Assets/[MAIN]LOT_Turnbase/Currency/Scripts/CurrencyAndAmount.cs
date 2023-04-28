using System;
using System.Collections;
using System.Collections.Generic;
using LOT_Turnbase;
using UnityEngine;

namespace LOT_Turnbase
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
