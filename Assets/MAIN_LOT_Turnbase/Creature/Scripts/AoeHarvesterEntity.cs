using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeHarvesterEntity : CharacterEntity
    {
        public override void Init(CreatureData creatureData)
        {
            base.Init(creatureData);
            StartCoroutine(WaitToModify());
        }

        private IEnumerator WaitToModify()
        {
            yield return new WaitForSeconds(1f);
            // Modify movingSpeed
            m_AnimateComp.SetFloatValue("MovingSpeed", m_CurrentStat.MovingSpeed);
        }
        
        public void SetCarryingState(bool isCarrying)
        {
            m_AnimateComp.SetBoolValue("Carrying" ,isCarrying);
        }
    }
}