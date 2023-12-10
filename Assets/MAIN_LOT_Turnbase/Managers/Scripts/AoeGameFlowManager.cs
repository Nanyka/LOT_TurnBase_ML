using UnityEngine;

namespace JumpeeIsland
{
    public class AoeGameFlowManager : GameFlowManager
    {
        public override EnvironmentManager GetEnvManager()
        {
            Debug.Log("Get environment");
            return null;
        }
    }
}