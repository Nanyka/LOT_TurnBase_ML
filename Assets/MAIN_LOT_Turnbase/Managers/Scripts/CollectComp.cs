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

        private UnityEvent<IAttackRelated> _dieEvent;

        public void Init(UnityEvent<IAttackRelated> dieEvent)
        {
            _dieEvent = dieEvent;
            // _dieEvent.AddListener(DieVisual);
            MainUI.Instance.OnEnableInteract.AddListener(EnableTrigger);
        }

        private void EnableTrigger()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isForBattleMode && GameFlowManager.Instance.GameMode == GameMode.ECONOMY)
                return;

            if (other.TryGetComponent(out CreatureEntity creatureEntity))
                Die(creatureEntity);
        }

        private void Die(IAttackRelated killedByFaction)
        {
            DieVisual();
            _dieEvent.Invoke(killedByFaction);
        }

        private void DieVisual()
        {
            _collectingVfx.Play();
            if (_isGlobalVfx)
                GameFlowManager.Instance.AskGlobalVfx(_globalVfxType, transform.position);
        }
    }
}