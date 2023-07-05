using GOAP;

namespace JumpeeIsland
{
    public class Idle: GAction
    {
        public override bool PrePerform()
        {
            return true;
        }

        public override bool PostPerform()
        {
            var agent = (GOAPCreatureBrain) m_GAgent;
            agent.ResponseToCreature(0);
            return true;
        }
    }
}