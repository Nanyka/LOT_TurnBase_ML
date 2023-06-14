using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class GOAPCreatureSensor : MonoBehaviour
    {
        public void DetectEnvironment(WorldStates beliefs)
        {
            beliefs.ClearStates();
            
            Debug.Log("GOAP creature sensor is working...");
        }
    }
}