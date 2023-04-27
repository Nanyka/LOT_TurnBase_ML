using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class StatsTranslator : Singleton<StatsTranslator>
    {
        [SerializeField] private List<ResourceStats> _foodStats;

        #region ResourceStats Translate

        public ResourceStats GetResourceStats(CurrencyType type, int level)
        {
            switch (type)
            {
                case CurrencyType.Food:
                    return _foodStats[level];
            }

            return null;
        }

        #endregion
    }
}
