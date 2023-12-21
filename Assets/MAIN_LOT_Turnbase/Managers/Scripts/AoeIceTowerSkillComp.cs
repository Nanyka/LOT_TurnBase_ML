using UnityEngine;

namespace JumpeeIsland
{
    public class AoeIceTowerSkillComp : MonoBehaviour, ISkillComp
    {
        [SerializeField] private int _duration;
        [SerializeField] private Material _effectMaterial;
        
        private ISkillEffect m_Skill;
        
        public void Init(string creatureName, int level)
        {
            m_Skill = new Frozen(_duration, _effectMaterial);
        }

        public ISkillEffect GetSkill(int skillIndex)
        {
            return m_Skill;
        }
    }
}