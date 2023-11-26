using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AutomationModeCreatureLoader : CreatureLoader
    {
        [SerializeField] private PlayerNpcLoader _playerNpcLoader;

        protected override void Init()
        {
            var playerFaction = (PlayerFactionController)_factionController;
            if (playerFaction._isAutomation)
            {
                // Just load King for PlayerFactionController
                foreach (var creatureData in _creatureDatas)
                    if (creatureData.EntityName.Equals("King"))
                        TrainANewCreature(creatureData);

                // ... and load the rest of troop on NpcFactionController
                _playerNpcLoader.Init(_creatureDatas);
            }
            else
            {
                foreach (var creatureData in _creatureDatas)
                    TrainANewCreature(creatureData);
            }

            _factionController.Init();
        }

        public override GameObject PlaceNewObject<T>(T data)
        {
            var creatureData = (CreatureData)Convert.ChangeType(data, typeof(CreatureData));
            var playerFaction = (PlayerFactionController)_factionController;
            if (playerFaction._isAutomation)
            {
                if (creatureData.EntityName.Equals("King"))
                    return TrainANewCreature(creatureData);
                _playerNpcLoader.PlaceNewObject(data);
            }
            else
                return TrainANewCreature(creatureData);

            return null;
        }
    }
}