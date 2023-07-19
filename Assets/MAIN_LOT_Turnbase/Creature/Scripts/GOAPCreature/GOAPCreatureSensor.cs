using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public abstract class GOAPCreatureSensor : MonoBehaviour
    {
        public abstract void Init(CreatureData creatureData);
        
        public abstract void DetectEnvironment(WorldStates beliefs);
    }
}