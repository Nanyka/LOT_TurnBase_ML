using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class SpawnJumpBoost : GAction
    {
        [SerializeField] private string _jumpBoostName;

        public override bool PrePerform()
        {
            var availableTile = GameFlowManager.Instance.GetEnvManager().GetPotentialTile();
            if (availableTile != Vector3.negativeInfinity)
                SavingSystemManager.Instance.OnSpawnCollectable(_jumpBoostName, availableTile, 0);
            
            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}