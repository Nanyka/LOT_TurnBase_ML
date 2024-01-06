using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ZombieTransform : GAction
    {
        [SerializeField] private AoeCharacterTransformComp _transformComp;
        
        public override bool PrePerform()
        {
            return true;
        }

        public override bool PostPerform()
        {
            _transformComp.ExecuteTransform();
            return true;
        }
    }
}