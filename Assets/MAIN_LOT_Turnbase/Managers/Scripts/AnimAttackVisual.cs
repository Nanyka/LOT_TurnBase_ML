using UnityEngine;

namespace JumpeeIsland
{
    public class AnimAttackVisual : MonoBehaviour, IAttackExecutor
    {
        [SerializeField] private ParticleSystem m_AttackVfx;
        [SerializeField] private AttackCollider[] m_AttackColliders;
        
        private Entity m_Entity;
        
        private void Start()
        {
            m_Entity = GetParent(transform);
            
            foreach (var attackCollider in m_AttackColliders)
                attackCollider.Init(this);
        }

        private Entity GetParent(Transform upperLevel)
        {
            if (upperLevel.TryGetComponent(out Entity creatureInGame))
                return creatureInGame;

            if (upperLevel.parent == null)
                return null;

            upperLevel = upperLevel.parent;
            return GetParent(upperLevel);
        }
        
        public FactionType GetFaction()
        {
            return m_Entity.GetFaction();
        }

        public void ExecuteAttackEffect()
        {
            m_AttackVfx.Play();
        }

        public void ExecuteHitEffect(int posIndex)
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteHitEffect(Vector3 atPos)
        {
            var collectableEntity = m_Entity as CollectableEntity;
            if (collectableEntity != null) collectableEntity.Attack(atPos);
        }

        public void ExecuteHitEffect(Vector3 atPos, int skillIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}