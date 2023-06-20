using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public abstract class GOAPCreatureSensor : MonoBehaviour
    {
        public abstract void DetectEnvironment(WorldStates beliefs);
    }
}