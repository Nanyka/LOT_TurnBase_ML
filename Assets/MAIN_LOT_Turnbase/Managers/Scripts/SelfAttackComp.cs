using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class SelfAttackComp : MonoBehaviour
    {
        [SerializeField] private SkillEffectType _skillEffectType;
        [SerializeField] private Material _effectMaterial;
        [SerializeField] private int _duration;
        [Tooltip("It might be strength multiplier or available range of attack")]
        [SerializeField] private int _magnitude;
        
        private SkillEffect _skillEffect;

        private void Start()
        {
            switch (_skillEffectType)
            {
                case SkillEffectType.StrengthBoost:
                    _skillEffect = new StrengthBooster(_duration,_magnitude);
                    break;
                case SkillEffectType.Teleport:
                    _skillEffect = new Teleport();
                    break;
                case SkillEffectType.Frozen:
                    _skillEffect = new Frozen(_duration, _effectMaterial);
                    break;
            }
        }

        public void Attack(Vector3 attackPoint, IAttackRelated mEntity, ISkillRelated mSkill)
        {
            var mEnvironment = GameFlowManager.Instance.GetEnvManager();
            var attackFaction = mEnvironment.CheckFaction(attackPoint);
            var target = mEnvironment.GetObjectByPosition(attackPoint, attackFaction);
            if (target == null)
                return;

            if (target.TryGetComponent(out IAttackRelated targetEntity))
            {
                if (_skillEffect != null)
                    _skillEffect.TakeEffectOn(mSkill, targetEntity);

                if (targetEntity.GetFaction() != mEntity.GetFaction())
                {
                    // Debug.Log($"Take damage on {targetEntity.name} an amount: {mEntity.GetAttackDamage()}");
                    targetEntity.TakeDamage(mEntity.GetAttackDamage(), mEntity);
                    mEntity.GainGoldValue();
                }
            }
        }
    }
}