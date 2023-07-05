using GOAP;

namespace JumpeeIsland
{
    public class GoBackward: GAction
    {
        public override bool PrePerform()
        {
            return true;
        }

        public override bool PostPerform()
        {
            var agent = (GOAPCreatureBrain) m_GAgent;
            agent.ResponseToCreature(3);
            return true;
        }
    }
}