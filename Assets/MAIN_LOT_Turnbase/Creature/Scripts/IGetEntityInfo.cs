using UnityEngine;

namespace JumpeeIsland
{
    public interface IGetEntityInfo
    {
        public (Vector3 midPos, Vector3 direction, int jumpStep, FactionType faction) GetCurrentState();
    }

    public interface IShowInfo
    {
        public (Entity entity,int jump) ShowInfo();
    }

    public interface ICreatureType
    {
        
    }
}