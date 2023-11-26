using UnityEngine;

namespace JumpeeIsland
{
    public class AoeCreatureLoader : CreatureLoader
    {
        protected override void Init()
        {
            _factionController.Init();
        }
        
        protected override GameObject TrainANewCreature(CreatureData creatureData)
        {
            var creatureObj = _creaturePool.GetObject(creatureData.EntityName);
            if (creatureObj == null)
                return null;

            creatureData.FactionType = _factionController.GetFaction(); // assign Faction
            GameFlowManager.Instance.OnDomainRegister.Invoke(creatureObj, _factionController.GetFaction());
            
            if (creatureObj.TryGetComponent(out CharacterEntity creatureInGame))
            {
                creatureInGame.gameObject.SetActive(true);
                creatureInGame.Init(creatureData);
            }

            return creatureObj;
        }
    }
}