using UnityEngine;

namespace JumpeeIsland
{
    public class AoeManualZombieEntity : CharacterEntity
    {
        [SerializeField] private bool _isNoDamage;
        
        public override void Init(CreatureData creatureData)
        {
            base.Init(creatureData);
            GetComponent<IAttackRegister>().Init();
            if (_isNoDamage) m_CreatureData.CurrentDamage = 0;
        }

        public void StartHarvest()
        {
            m_AnimateComp.SetAnimation(AnimateType.Harvest);
        }
    }
}