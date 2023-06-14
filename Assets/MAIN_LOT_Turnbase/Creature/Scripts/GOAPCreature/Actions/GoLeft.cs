using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class GoLeft: GAction
    {
        public override bool PrePerform()
        {
            return true;
        }

        public override bool PostPerform()
        {
            var agent = (GOAPCreatureBrain) m_GAgent;
            agent.ResponseToCreature(1);
            return true;
        }
    }
}