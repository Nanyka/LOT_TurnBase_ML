using UnityEngine;

namespace JumpeeIsland
{
    public interface IGetCreatureInfo
    {
        public (Vector3 midPos, Vector3 direction, int jumpStep, FactionType faction) GetCurrentState();
        public EnvironmentManager GetEnvironment();
    }

    public interface IShowInfo
    {
        public (Entity entity,int jump) ShowInfo();
    }
}