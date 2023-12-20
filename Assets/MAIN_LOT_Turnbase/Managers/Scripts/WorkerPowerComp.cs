using UnityEngine;

namespace JumpeeIsland
{
    public class WorkerPowerComp : MonoBehaviour, IComboMonitor
    {
        [SerializeField] private int _attackIndex;

        public void SetSpecialAttack(int attackIndex)
        {
            _attackIndex = attackIndex;
        }

        public void PowerUp()
        {
            // Debug.Log("Worker power up");
        }

        public void ResetPowerBar()
        {
            // Debug.Log("Worker reset powerBar");
        }

        public void DisablePowerBar()
        {
            throw new System.NotImplementedException();
        }
    }
}