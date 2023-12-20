using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeArcherAttackExecutor : MonoBehaviour, IAttackExecutor
    {
        
        [SerializeField] private AnimAttackCollider[] attackColliders;
        
        private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;
        private IComboMonitor _comboMonitor;
        private IComboReceiver _comboReceiver;
        private IAnimateComp _animateComp;
        private Vector3 _attackPos;
        private IFireComp _fireComp;

        private void Start()
        {
            _attackComp = GetComponent<IAttackComp>();
            _attackRelated = GetComponent<IAttackRelated>();
            _comboMonitor = GetComponent<IComboMonitor>();
            _comboReceiver = GetComponent<IComboReceiver>();
            _animateComp = GetComponent<IAnimateComp>();
            _fireComp = GetComponent<IFireComp>();
            
            foreach (var attackCollider in attackColliders)
                attackCollider.Init(this);
        }

        public void PlayAttackVfx()
        {
            _fireComp.PlayCurveFX(_attackPos);
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

        // Use to set firing target
        public void ExecuteHitEffect(Vector3 atPos)
        {
            _attackPos = atPos;
            _animateComp.TriggerAttackAnim(_comboReceiver.GetAttackIndex());
        }

        public void ExecuteHitEffect(Vector3 atPos, int skillIndex)
        {
            Debug.Log("Use for multi range attacks");
        }
    }
}