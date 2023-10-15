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
            if (availableTile.x.CompareTo(float.NegativeInfinity) == 1)
                SavingSystemManager.Instance.OnSpawnCollectable(_resourceId,availableTile,0);
        
            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}