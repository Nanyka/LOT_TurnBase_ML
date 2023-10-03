using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AttackCollider : MonoBehaviour
    {
        [SerializeField] private int skillIndex;

        private AttackVisual _attackVisual;
        private ParticleSystem m_AttackVfx;

        public void Init(AttackVisual attackVisual)
        {
            _attackVisual = attackVisual;
            m_AttackVfx = GetComponent<ParticleSystem>();
            var collision = m_AttackVfx.collision;

            if (_attackVisual.GetFaction() == FactionType.Player)
                collision.collidesWith = 1 << 9 | 1 << 11;
            else
                collision.collidesWith = 1 << 9 | 1 << 7;
        }

        private void OnParticleCollision(GameObject other)
        {
            if (skillIndex > 0)
                _attackVisual.ExecuteHitEffect(other.transform.position, skillIndex);
            else
                _attackVisual.ExecuteHitEffect(other.transform.position);
        }
    }
}