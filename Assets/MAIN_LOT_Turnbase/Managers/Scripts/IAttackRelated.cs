using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public interface IAttackRelated
    {
        public void GainGoldValue();
        public FactionType GetFaction();
        public int GetAttackDamage();
        public IEnumerable<Skill_SO> GetSkills();
        public IEffectComp GetEffectComp();
        public void AccumulateKills();
    }

    public interface ISkillCaster
    {
        public void UpdateTransform(Vector3 pos, Vector3 dir);
        public FactionType GetFaction();
    }
}