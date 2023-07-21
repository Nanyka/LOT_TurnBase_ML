using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class EffectComp : MonoBehaviour
    {
        private Entity m_Entity;
        private bool _isStrengthBoost = false;

        public void Init(Entity entity)
        {
            m_Entity = entity;
            GameFlowManager.Instance.GetEnvManager().OnChangeFaction.AddListener(EffectCountDown);

            if (m_Entity.GetType() == typeof(CreatureEntity))
            {
                var creatureData = (CreatureData)m_Entity.GetData();
                if (creatureData.StregthBoostRemain > 0)
                    _isStrengthBoost = true;
            }
        }

        private void EffectCountDown()
        {
            if (GameFlowManager.Instance.GetEnvManager().GetCurrFaction() != m_Entity.GetFaction())
                return;

            if (_isStrengthBoost)
            {
                if (m_Entity.GetType() == typeof(CreatureEntity))
                {
                    var creatureData = (CreatureData)m_Entity.GetData();
                    creatureData.StregthBoostRemain--;
                    if (creatureData.StregthBoostRemain <= 0)
                    {
                        creatureData.StregthBoostRemain = 0;
                        if (m_Entity is IStatsProvider<UnitStats> statsProvider)
                            creatureData.CurrentDamage = statsProvider.GetStats().Strengh;
                        _isStrengthBoost = false;
                    }
                }
            }
        }

        // TODO entity take effect from attack
        public bool StrengthBoost()
        {
            if (_isStrengthBoost)
                return false;
            _isStrengthBoost = true;
            return true;
        }
    }
}