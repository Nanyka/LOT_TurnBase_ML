using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AoeMeleeAttackExecutor : MonoBehaviour, IAttackExecutor
    {
        [SerializeField] private ParticleSystem[] attackVfx;
        [SerializeField] private List<AnimAttackCollider> attackColliders;
        
        private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;
        private ISpecialSkillReceiver _specialSkill;
        private ISkillMonitor _skillMonitor;

        private void Start()
        {
            _attackComp = GetComponent<IAttackComp>();
            _attackRelated = GetComponent<IAttackRelated>();
            _specialSkill = GetComponent<ISpecialSkillReceiver>();
            _skillMonitor = GetComponent<ISkillMonitor>();
            
            foreach (var attackCollider in attackColliders)
                attackCollider.Init(this);
        }

        public void PlayAttackVfx()
        {
            attackVfx[_specialSkill.GetAttackIndex()].Play();
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
