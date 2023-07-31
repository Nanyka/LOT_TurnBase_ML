using UnityEngine;

namespace JumpeeIsland
{
    public interface ICreatureMove
    {
        public void CreatureStartMove(Vector3 currentPos, int direction);
        public void CreatureEndMove();
    }
}