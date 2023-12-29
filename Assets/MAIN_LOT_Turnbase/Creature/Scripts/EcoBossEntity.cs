using System;

namespace JumpeeIsland
{
    public class EcoBossEntity : CharacterEntity
    {
        private void Start()
        {
            // m_Brain.OnDisable();
        }

        public override void Init(CreatureData creatureData)
        {
            base.Init(creatureData);
            GetComponent<AoeEcoBoss0AttackExecutor>().Init();
        }
    }
}