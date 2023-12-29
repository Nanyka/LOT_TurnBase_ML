using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CreatureDataChecker : MonoBehaviour, ITimeMachineChecker
    {
        [SerializeField] private CreatureType _creatureType;
        [SerializeField] private int _amount;
        
        public bool ConditionCheck()
        {
            var envData = SavingSystemManager.Instance.GetEnvironmentData();
            var creatureAmount =
                envData.PlayerData.Count(t => t.FactionType == FactionType.Player && t.CreatureType == _creatureType);

            return creatureAmount >= _amount;
        }
    }
}