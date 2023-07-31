using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class SpawnGame : GAction
    {
        [SerializeField] private string _resourceId;
        
        public override bool PrePerform()
        {
            var availableTile = GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile();
            if (availableTile != Vector3.negativeInfinity)
                SavingSystemManager.Instance.OnSpawnResource(_resourceId,availableTile);
        
            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}