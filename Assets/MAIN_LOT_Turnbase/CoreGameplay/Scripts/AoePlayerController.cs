using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoePlayerController : MonoBehaviour, IFactionController
    {
        private FactionType m_Faction = FactionType.Player;

        private List<CharacterEntity> _characterEntities;

        public void Init()
        {
            Debug.Log("Player faction initiate");
        }

        public void AddCreatureToFaction(ICreatureType creature)
        {
            Debug.Log($"Type of the creature: {creature.GetType()}");
            var characterEntity = creature as CharacterEntity;
            if (characterEntity != null)
                _characterEntities.Add(characterEntity);
        }

        public MovementInspector GetMovementInspector()
        {
            throw new NotImplementedException();
        }

        public void KickOffNewTurn()
        {
            throw new NotImplementedException();
        }

        public void WaitForCreature()
        {
            throw new NotImplementedException();
        }

        public FactionType GetFaction()
        {
            return m_Faction;
        }

        public EnvironmentManager GetEnvironment()
        {
            throw new NotImplementedException();
        }

        public void RemoveAgent(CreatureInGame creatureInGame)
        {
            Debug.Log("Player faction remove");
        }

        public void ResetData()
        {
            Debug.Log("Player faction reset");
        }
    }
}