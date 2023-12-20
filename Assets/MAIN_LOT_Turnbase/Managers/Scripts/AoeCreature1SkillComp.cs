using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeCreature1SkillComp : MonoBehaviour, ISkillComp
    {
        [SerializeField] private int _duration;
        [SerializeField] private Material _effectMaterial;

        private ISkillEffect[] m_Skills;

        public void Init(string creatureName, int level)
        {
            var inventory = SavingSystemManager.Instance.GetInventoryItemByName(creatureName);
            m_Skills = new ISkillEffect[inventory.skillsAddress.Count];

            for (int i = 0; i < inventory.skillsAddress.Count; i++)
            {
                switch (inventory.skillsAddress[i])
                {
                    case SkillEffectType.Frozen:
                        m_Skills[i] = new Frozen(_duration, _effectMaterial);
                        break;
                }
            }
        }

        public ISkillEffect GetSkill(int skillIndex)
        {
            return m_Skills[skillIndex];
        }
    }

    public interface ISkillComp
    {
        public void Init(string creatureName, int level);
        public ISkillEffect GetSkill(int skillIndex);
    }
}