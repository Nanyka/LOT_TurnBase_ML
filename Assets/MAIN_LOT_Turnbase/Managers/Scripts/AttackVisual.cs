using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AttackVisual : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_HitVfx;
        [SerializeField] private Transform m_VfxContainer;

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
            var attackRange = m_Creature.GetAttackRange();
            if (posIndex >= attackRange.Count())
                return;
            
            var hitPoint = attackRange.ElementAt(posIndex);

            var environment = GameFlowManager.Instance.GetEnvManager();
            if (environment.CheckEnemy(hitPoint, m_Creature.GetFaction()) ||
                environment.CheckAlly(hitPoint, FactionType.Neutral))
            {
                m_VfxContainer.position = hitPoint;
                m_HitVfx.Play();
            }
        }

        public void ExecuteJumpEffect()
        {
            GameFlowManager.Instance.AskGlobalVfx(GlobalVfxType.JUMP, transform.position);
        }
    }
}