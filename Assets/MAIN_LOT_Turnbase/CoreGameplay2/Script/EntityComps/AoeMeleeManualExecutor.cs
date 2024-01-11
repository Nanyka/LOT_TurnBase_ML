using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeMeleeManualExecutor : MonoBehaviour, IAttackExecutor, IAttackRegister
    {
        [SerializeField] private ParticleSystem[] attackVfx;
        [SerializeField] private List<AnimAttackCollider> attackColliders;
        
        private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;
        private IComboReceiver _combo;
        private IComboMonitor _comboMonitor;

        public void Init()
        {
            _attackComp = GetComponent<IAttackComp>();
            _attackRelated = GetComponent<IAttackRelated>();
            _combo = GetComponent<IComboReceiver>();
            _comboMonitor = GetComponent<IComboMonitor>();
            
            foreach (var attackCollider in attackColliders)
                attackCollider.Init(this);
        }

        public void PlayAttackVfx()
        {
            attackVfx[_combo.GetAttackIndex()].Play();
            _comboMonitor.ResetPowerBar();
        }

        public FactionType GetFaction()
        {
            return _attackRelated.GetFaction();
        }
        
        public void ExecuteHitEffect(GameObject target)
        {
            _attackComp.SuccessAttack(target);
        }

        public void ExecuteHitEffect(Vector3 atPos)
        {
            Debug.Log("Use for single range attack");
        }

        public void ExecuteHitEffect(Vector3 atPos, int skillIndex)
        {
            Debug.Log("Use for multi range attacks");
        }
    }
}