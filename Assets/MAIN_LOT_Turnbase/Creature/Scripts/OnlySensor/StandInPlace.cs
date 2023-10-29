using UnityEngine;

namespace JumpeeIsland
{
    public class StandInPlace : ISensorExecute
    {
        public (int, int) DecideDirection(CreatureData m_CreatureData, Transform m_Transform, EnvironmentManager _envManager,
            CreatureEntity m_Entity, SkillComp skillComp)
        {
            return (0,1);
        }
    }
}