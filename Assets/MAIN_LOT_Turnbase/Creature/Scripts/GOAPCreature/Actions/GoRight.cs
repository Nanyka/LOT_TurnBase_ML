using GOAP;

namespace JumpeeIsland
{
    public class GoRight: GAction
    {
        public override bool PrePerform()
        {
            return true;
        }

        public override bool PostPerform()
        {
            var agent = (GOAPCreatureBrain) m_GAgent;
            agent.ResponseToCreature(2);
            return true;
        }
    }
}