using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingReplayController : BuildingController
    {
        public override void Update()
        {
        }

        public void BuildingFire(RecordAction action)
        {
            var currentBuilding = m_buildings.Find(t => Vector3.Distance(t.GetPosition(), action.AtPos) < 0.1f);
            if (currentBuilding != null)
                currentBuilding.AskForAttack();
        }
    }
}