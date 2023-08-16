using System;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class CollectComp : MonoBehaviour
    {
        [SerializeField] private bool _isForBattleMode;
        
        private UnityEvent<Entity> _dieEvent;
        
        public void Init(UnityEvent<Entity> dieEvent)
        {
            _dieEvent = dieEvent;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isForBattleMode && GameFlowManager.Instance.IsEcoMode)
                return;
            
            if (other.TryGetComponent(out CreatureEntity creatureEntity))
            {
                Die(creatureEntity);
            }
        }

        private void Die(Entity killedByFaction)
        {
            _dieEvent.Invoke(killedByFaction);
        }
    }
}