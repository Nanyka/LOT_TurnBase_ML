using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class SkillComp : MonoBehaviour
    {
        [SerializeField] private List<Skill_SO> m_SkillSOs = new();

        public void Init(string creatureName)
        {
            if (m_SkillSOs.Count > 0)
                return;

            var enemyInventory = SavingSystemManager.Instance.GetInventoryItemByName(creatureName);
            if (enemyInventory.skillsAddress == null)
                return;
            
            foreach (var skillAddress in enemyInventory.skillsAddress)
                m_SkillSOs.Add((Skill_SO)AddressableManager.Instance.GetAddressableSO(skillAddress));
        }

        public IEnumerable<Vector3> AttackPoints(Vector3 targetPos, Vector3 direction, int jumpStep)
        {
            if (m_SkillSOs.Count() < jumpStep || m_SkillSOs[jumpStep - 1] == null)
                return null;
            return m_SkillSOs[jumpStep - 1].CalculateSkillRange(targetPos, direction);
        }

        public float GetSkillMagnitude(int jumpStep)
        {
            return m_SkillSOs[jumpStep - 1].GetDuration();
        }

        public IEnumerable<Skill_SO> GetSkills()
        {
            return m_SkillSOs;
        }

        public Skill_SO GetSkillByIndex(int index)
        {
            return m_SkillSOs[index];
        }

        public int GetSkillAmount()
        {
            return m_SkillSOs.Count();
        }

        public string GetAttackAnimation(int atIndex)
        {
            return m_SkillSOs[atIndex].GetAnimation();
        }
    }
}