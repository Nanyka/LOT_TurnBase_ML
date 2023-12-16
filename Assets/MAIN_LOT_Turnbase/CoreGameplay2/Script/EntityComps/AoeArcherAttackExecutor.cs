using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeArcherAttackExecutor : MonoBehaviour, IAttackExecutor
    {
        [SerializeField] private FireComp m_FireComp;
        [SerializeField] private AnimAttackCollider[] attackColliders;
        
        private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;
        private ISkillMonitor _skillMonitor;
        private ISpecialSkillReceiver _skillReceiver;
        private IAnimateComp _animateComp;
        private Vector3 _attackPos;

        private void Start()
        {
            _attackComp = GetComponent<IAttackComp>();
            _attackRelated = GetComponent<IAttackRelated>();
            _skillMonitor = GetComponent<ISkillMonitor>();
            _skillReceiver = GetComponent<ISpecialSkillReceiver>();
            _animateComp = GetComponent<IAnimateComp>();
            
            foreach (var attackCollider in attackColliders)
                attackCollider.Init(this);
        }

        public void PlayAttackVfx()
        {
            m_FireComp.PlayCurveFX(_attackPos);
            _skillMonitor.ResetPowerBar();
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
            _animateComp.TriggerAttackAnim(_skillReceiver.GetAttackIndex());
        }

        public void ExecuteHitEffect(Vector3 atPos, int skillIndex)
        {
            Debug.Log("Use for multi range attacks");
        }
    }
}