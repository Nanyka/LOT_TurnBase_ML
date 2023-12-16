using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AoeBoss0AttackExecutor : MonoBehaviour, IAttackExecutor
    {
        [SerializeField] private float attackRange;
        [SerializeField] private ParticleSystem[] attackVfx;
        [SerializeField] private AnimAttackCollider attackCollider;
        [SerializeField] private Transform specialAttackPos;

        private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;
        private ISpecialSkillReceiver _specialSkill;
        private ISkillMonitor _skillMonitor;
        private LayerMask layerMask = 1 << 7;

        private void Start()
        {
            _attackComp = GetComponent<IAttackComp>();
            _attackRelated = GetComponent<IAttackRelated>();
            _specialSkill = GetComponent<ISpecialSkillReceiver>();
            _skillMonitor = GetComponent<ISkillMonitor>();

            SetAttackLayer();
        }

        private void SetAttackLayer()
        {
            attackCollider.Init(this);
            if (_attackRelated.GetFaction() == FactionType.Player)
                layerMask = 1 << 9 | 1 << 11;
            else
                layerMask = 1 << 9 | 1 << 7;
        }

        public void PlayAttackVfx()
        {
            attackVfx[_specialSkill.GetAttackIndex()].Play();

            if (_specialSkill.GetAttackIndex() > 0)
            {
                Collider[] hitColliders = new Collider[10];
                int numColliders =
                    Physics.OverlapSphereNonAlloc(specialAttackPos.position, attackRange, hitColliders, layerMask);

                if (numColliders > 0)
                {
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider == null)
                            continue;
                        ExecuteHitEffect(hitCollider.gameObject);
                    }
                }
            }

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