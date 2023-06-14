using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class GoAway: GAction
    {
        public override bool PrePerform()
        {
            Debug.Log("Start go away");
            return true;
        }

        public override bool PostPerform()
        {
            Debug.Log("End go away");
            
            var agent = (GOAPCreatureBrain) m_GAgent;
            agent.ResponseToCreature(1);
            return true;
        }
    }
}