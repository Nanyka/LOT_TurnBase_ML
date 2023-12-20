using UnityEngine;

namespace JumpeeIsland
{
    public class AoeCreature1AttackComp : MonoBehaviour, IAttackComp
    {
        private IComboMonitor m_ComboMonitor;
        private IAttackRelated m_AttackRelated;
        private ISkillComp m_SkillComp;
        private ISkillCaster _mSkillCaster;
        private IComboReceiver _comboReceiver;
        private int _skillAvailable;

        private void Start()
        {
            m_ComboMonitor = GetComponent<IComboMonitor>();
            m_AttackRelated = GetComponent<IAttackRelated>();
            m_SkillComp = GetComponent<ISkillComp>();
            _mSkillCaster = GetComponent<ISkillCaster>();
            _comboReceiver = GetComponent<IComboReceiver>();
        }

        public void SuccessAttack(GameObject target)
        {
            if (target.TryGetComponent(out IHealthComp targetHp))
            {
                targetHp.TakeDamage(m_AttackRelated);
                m_ComboMonitor.PowerUp();
            }
            
            if (target.TryGetComponent(out IAttackRelated targetEntity))
            {
                if (_skillAvailable > 0)
                {
                    m_SkillComp.GetSkill(_skillAvailable)?.TakeEffectOn(_mSkillCaster, targetEntity);
                    _skillAvailable = 0;
                }
                else if (_comboReceiver.GetAttackIndex() > 0)
                {
                    _skillAvailable = _comboReceiver.GetAttackIndex();
                }
            }
        }
    }
}