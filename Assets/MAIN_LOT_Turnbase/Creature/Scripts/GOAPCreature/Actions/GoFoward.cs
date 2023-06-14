using GOAP;

namespace JumpeeIsland
{
    public class GoFoward: GAction
    {
        public override bool PrePerform()
        {
            return true;
        }

        public override bool PostPerform()
        {
            var agent = (GOAPCreatureBrain) m_GAgent;
            agent.ResponseToCreature(4);
            return true;
        }
    }
}