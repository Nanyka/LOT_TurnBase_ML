using UnityEngine;

namespace JumpeeIsland
{
    public interface IGetCreatureInfo
    {
        public (string name, int health, int damage, int power) InfoToShow();
        public (Vector3 midPos, Vector3 direction, int jumpStep, FactionType faction) GetCurrentState();
        public EnvironmentManager GetEnvironment();
    }
}