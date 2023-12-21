namespace JumpeeIsland
{
    public class AoeHarvesterEntity : CharacterEntity
    {
        public void SetCarryingState(bool isCarrying)
        {
            m_AnimateComp.SetBoolValue("Carrying" ,isCarrying);
        }
    }
}