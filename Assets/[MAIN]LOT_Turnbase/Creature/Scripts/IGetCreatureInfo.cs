using UnityEngine;

namespace LOT_Turnbase
{
    public interface IGetCreatureInfo
    {
        public (string name, int health, int damage, int power) GetCreatureInfo();
        public (Vector3 midPos, Vector3 direction, int jumpStep, FactionType faction) GetCurrentState();
        public EnvironmentManager GetEnvironment();
    }
}