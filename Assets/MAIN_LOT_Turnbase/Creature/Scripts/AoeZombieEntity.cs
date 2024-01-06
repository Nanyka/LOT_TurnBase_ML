namespace JumpeeIsland
{
    public class AoeZombieEntity : CharacterEntity
    {
        public override void Init(CreatureData creatureData)
        {
            base.Init(creatureData);
            GetComponent<IAttackRegister>().Init();
        }

        public void StartHarvest()
        {
            m_AnimateComp.SetAnimation(AnimateType.Harvest);
        }
    }
}