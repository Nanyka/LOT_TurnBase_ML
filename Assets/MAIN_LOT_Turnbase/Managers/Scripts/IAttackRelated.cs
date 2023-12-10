using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public interface IAttackRelated
    {
        public void GainGoldValue();
        public FactionType GetFaction();
        public int GetAttackDamage();
        // public void TakeDamage(int damage, IAttackRelated fromEntity);
        public IEnumerable<Skill_SO> GetSkills();
        public EffectComp GetEffectComp();
        public void AccumulateKills();
    }

    public interface ISkillRelated
    {
        public void UpdateTransform(Vector3 pos, Vector3 dir);
        public FactionType GetFaction();
    }
}