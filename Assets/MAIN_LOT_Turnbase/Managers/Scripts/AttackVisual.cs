using Sirenix.OdinInspector;
using UnityEngine;

namespace JumpeeIsland
{
    public class AttackVisual : MonoBehaviour,IAttackExecutor
    {
        [SerializeField] private bool _isRangeAttack;

        [ShowIf("@_isRangeAttack == true")] [SerializeField]
        private FireComp m_FireComp;

        [SerializeField] private bool _isSelfSetPath;
        [SerializeField] private ParticleSystem[] m_AttackVfx;
        [SerializeField] private AttackCollider[] m_AttackColliders;

        private CreatureEntity m_Creature;
        private bool _originalRangeAttack;

        private void Start()
        {
            m_Creature = GetParent(transform);
            if (m_Creature == null)
                return;

            _originalRangeAttack = _isRangeAttack;

            foreach (var attackCollider in m_AttackColliders)
                attackCollider.Init(this);
        }

        private CreatureEntity GetParent(Transform upperLevel)
        {
            if (upperLevel.TryGetComponent(out CreatureEntity creatureInGame))
                return creatureInGame;

            if (upperLevel.parent == null)
                return null;

            upperLevel = upperLevel.parent;
            return GetParent(upperLevel);
        }

        // Use to set _isRangeAttack by animation
        public void SetRangeAttack(int range)
        {
            _isRangeAttack = range > 0;
        }

        public void ExecuteHitEffect(int posIndex)
        {
            m_Creature.Attack(posIndex);
        }

        public void ExecuteHitEffect(Vector3 atPos)
        {
            m_Creature.Attack(atPos);
        }

        public void ExecuteHitEffect(Vector3 atPos, int skillIndex)
        {
            m_Creature.Attack(atPos, skillIndex);
        }

        public void ExecuteHitEffect(GameObject target)
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteJumpEffect()
        {
            GameFlowManager.Instance.AskGlobalVfx(GlobalVfxType.JUMP, transform.position);
        }

        // There are some kind of attack:
        // 1- Based on attack path calculated from skill
        // 2- Use FireComp to execute accurate attacks and cast damage on target using AttackCollider
        //    With regard to _isSelfSetPath case, attack path is created by fireComp, so, not require RotateTowardTarget()
        // 3- Execute attackVfx and take damage on objects collided with the particle system via AttackCollider
        // [REMEMBER] ExecuteAttackEffect use 1-based index
        
        public void ExecuteAttackEffect(int index)
        {
            if (_isRangeAttack)
            {
                m_FireComp.SetBulletFX(m_AttackVfx[index-1]);
                m_FireComp.PlayCurveFX(
                    _isSelfSetPath ? m_Creature.CalculateAttackRange(index) : m_Creature.GetAttackRange(), this);
            }
            else
                m_AttackVfx[index-1].Play();
        }

        public void ExecutePreAttackEffect()
        {
            m_Creature.PreAttackEffect();
        }

        public void RotateTowardTarget()
        {
            if (_isRangeAttack)
                m_Creature.RotateTowardTarget();
            else
                m_Creature.RotateTowardTarget(transform);
        }

        public void RotateTowardTarget(Vector3 target)
        {
            m_Creature.RotateTowardTarget(target);
        }

        public void EndAnimation()
        {
            _isRangeAttack = _originalRangeAttack;
            m_Creature.GetAnimateComp().EndMove();
        }

        #region UTILITIES

        public FactionType GetFaction()
        {
            return m_Creature.GetFaction();
        }

        #endregion
    }
}