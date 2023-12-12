using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AnimAttackCollider : MonoBehaviour
    {
        [SerializeField] private LayerMask collideLayer;
        
        private IAttackExecutor _attackExecutor;
        private ParticleSystem m_AttackVfx;

        public void Init(IAttackExecutor attackVisual)
        {
            _attackExecutor = attackVisual;
            m_AttackVfx = GetComponent<ParticleSystem>();
            var collision = m_AttackVfx.collision;

            if (collideLayer.value != 0)
            {
                collision.collidesWith = collideLayer;
                return;
            }

            if (_attackExecutor.GetFaction() == FactionType.Player)
                collision.collidesWith = 1 << 9 | 1 << 11;
            else
                collision.collidesWith = 1 << 9 | 1 << 7;
        }

        private void OnParticleCollision(GameObject other)
        {
            _attackExecutor.ExecuteHitEffect(other);
        }
    }
}