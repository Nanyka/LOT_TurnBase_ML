using System;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class CollectComp : MonoBehaviour
    {
        [SerializeField] private bool _isForBattleMode;
        [SerializeField] private ParticleSystem _collectingVfx;
        
        private UnityEvent<Entity> _dieEvent;
        
        public void Init(UnityEvent<Entity> dieEvent)
        {
            _dieEvent = dieEvent;
            MainUI.Instance.OnEnableInteract.AddListener(EnableTrigger);
        }

        private void EnableTrigger()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isForBattleMode && GameFlowManager.Instance.IsEcoMode)
                return;
            
            if (other.TryGetComponent(out CreatureEntity creatureEntity))
            {
                Die(creatureEntity);
                _collectingVfx.Play();
            }
        }

        private void Die(Entity killedByFaction)
        {
            _dieEvent.Invoke(killedByFaction);
        }
    }
}