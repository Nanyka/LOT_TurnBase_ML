using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class SpawnTree : GAction
    {
        [SerializeField] private string _resourceId;
        
        public override bool PrePerform()
        {
            var availableTile = GameFlowManager.Instance.GetEnvManager().GetPotentialTile();
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
