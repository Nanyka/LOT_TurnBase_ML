using System;

namespace JumpeeIsland
{
    public class ManualRegisterEntity : CharacterEntity
    {
        public override void Init(CreatureData creatureData)
        {
            base.Init(creatureData);
            GetComponent<IAttackRegister>().Init();
        }
    }
}