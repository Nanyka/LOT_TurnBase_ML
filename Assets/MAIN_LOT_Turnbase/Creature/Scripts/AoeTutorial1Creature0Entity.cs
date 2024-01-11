using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTutorial1Creature0Entity : CharacterEntity
    {
        public override void Init(CreatureData creatureData)
        {
            base.Init(creatureData);
            GetComponent<IAttackRegister>().Init();

            // if (m_Brain.TryGetComponent(out PlayerAwareMonster sensor))
            //     StartCoroutine(FirstDetecting(sensor));
        }

        // private IEnumerator FirstDetecting(PlayerAwareMonster sensor)
        // {
        //     yield return new WaitForSeconds(3f);
        //     sensor.AwareMonsterAttack();
        // }
    }
}