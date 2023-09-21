using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AttackVisual : MonoBehaviour
    {
        [SerializeField] private Transform m_AttackContainer;
        [SerializeField] private ParticleSystem[] m_AttackVfx;

        private CreatureEntity m_Creature;

        private void Start()
        {
            m_Creature = GetParent(transform);
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

        public void ExecuteHitEffect(int posIndex)
        {
            // TODO attack is handled by animation
            m_Creature.Attack(posIndex);
        }

        public void ExecuteJumpEffect()
        {
            GameFlowManager.Instance.AskGlobalVfx(GlobalVfxType.JUMP, transform.position);
        }

        public void ExecuteAttackEffect(int index)
        {
            // m_AttackContainer.position = m_Creature.GetData().Position;
            m_AttackVfx[index].Play();
        }

        public void ExecutePreAttackEffect()
        {
            m_Creature.PreAttackEffect();
        }
    }
}