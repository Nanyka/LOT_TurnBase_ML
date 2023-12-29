using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class CurrencyDataChecker : MonoBehaviour, ITimeMachineChecker
    {
        [SerializeField] private CurrencyType _currencyType;
        [SerializeField] private int _amount;
        
        public bool ConditionCheck()
        {
            if (_currencyType == CurrencyType.TROOP)
                return SavingSystemManager.Instance.GetStorageController().GetStorages()
                    .Sum(t => t.GetSpawnableAmount()) >= _amount;
                    
            return SavingSystemManager.Instance.CheckEnoughCurrency(_currencyType.ToString(), _amount);
        }
    }
}