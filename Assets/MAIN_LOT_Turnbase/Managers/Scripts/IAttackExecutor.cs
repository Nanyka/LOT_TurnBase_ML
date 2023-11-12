using UnityEngine;

namespace JumpeeIsland
{
    public interface IAttackExecutor
    {
        public FactionType GetFaction();
        public void ExecuteHitEffect(int posIndex);
        public void ExecuteHitEffect(Vector3 atPos);
        public void ExecuteHitEffect(Vector3 atPos, int skillIndex);
        public void ExecuteHitEffect(GameObject target);
    }
}