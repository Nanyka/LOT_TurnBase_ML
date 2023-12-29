using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingDataChecker : MonoBehaviour, ITimeMachineChecker
    {
        [SerializeField] private BuildingType _buildingType;
        [SerializeField] private int _amount;
        
        public bool ConditionCheck()
        {
            var envData = SavingSystemManager.Instance.GetEnvironmentData();
            var factoryAmount = envData.BuildingData.Count(t =>
                t.FactionType == FactionType.Player && t.BuildingType == _buildingType);

            return factoryAmount >= _amount;
        }
    }
}