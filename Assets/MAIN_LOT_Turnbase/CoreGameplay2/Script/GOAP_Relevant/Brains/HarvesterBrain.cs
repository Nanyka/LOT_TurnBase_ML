using System.Collections;
using System.Collections.Generic;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class HarvesterBrain : EnemyBrainComp
    {
        public override void EnableBrain()
        {
            base.EnableBrain();
            AddInitStates();
        }

        private void AddInitStates()
        {
            Beliefs.AddState("Empty");
        }
    }
}
