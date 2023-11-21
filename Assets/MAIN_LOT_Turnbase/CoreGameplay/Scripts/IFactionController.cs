using UnityEngine;

namespace JumpeeIsland
{
    public interface IFactionController
    {
        public void Init();
        public void AddCreatureToFaction(ICreatureType creatureInGame);
        public MovementInspector GetMovementInspector();
        public void KickOffNewTurn();
        public void WaitForCreature();
        public FactionType GetFaction();
        public EnvironmentManager GetEnvironment();
        public void RemoveAgent(CreatureInGame creatureInGame);
        public void ResetData();
    }
}