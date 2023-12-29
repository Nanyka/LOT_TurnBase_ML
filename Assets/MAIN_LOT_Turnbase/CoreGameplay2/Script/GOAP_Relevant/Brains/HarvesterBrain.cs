using System.Collections;
using System.Collections.Generic;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class HarvesterBrain : EnemyBrainComp
    {
        public override void OnEnable()
        {
            base.OnEnable();
            AddInitStates();
        }

        private void AddInitStates()
        {
            Beliefs.ModifyState("Empty",1);
        }
    }
}
