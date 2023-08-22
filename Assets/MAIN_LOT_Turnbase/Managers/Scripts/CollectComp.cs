using System;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class CollectComp : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _collectingVfx;
        [SerializeField] private bool _isGlobalVfx;
        [SerializeField] private GlobalVfxType _globalVfxType;
        [SerializeField] private bool _isForBattleMode;
        
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

                if (_isGlobalVfx)
                    GameFlowManager.Instance.AskGlobalVfx(_globalVfxType,transform.position);
            }
        }

        private void Die(Entity killedByFaction)
        {
            _dieEvent.Invoke(killedByFaction);
        }
    }
}