using System.Collections;
using System.Collections.Generic;
using GOAP;
using JumpeeIsland;
using UnityEngine;

namespace JumpeeIsland
{
    public class SpawnTree : GAction
    {
        public override bool PrePerform()
        {
            var availableTile = GameFlowManager.Instance.GetEnvManager().GetAvailableTile();
            if (availableTile != Vector3.negativeInfinity)
                SavingSystemManager.Instance.OnSpawnResource(InventoryType.Resource,availableTile);
        
            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}
