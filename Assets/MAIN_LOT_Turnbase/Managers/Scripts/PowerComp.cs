using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class PowerComp : MonoBehaviour, ISkillMonitor
    {
        private ISpecialSkillReceiver _skillComp;
        private IEntityUIUpdate _entityUIUpdate;
        private int _attackIndex;
        private readonly int _maxPower = 3; // 3 normal attack allow 1 special attack
        [SerializeField] private int _curPower;
        private bool _isAvailable;
        private bool _isPowerFull;

        private void Awake()
        {
            _skillComp = GetComponent<ISpecialSkillReceiver>();
            _entityUIUpdate = GetComponent<IEntityUIUpdate>();
        }

        // Enable powerBar when specialAttack is unlocked
        // Fill up the powerBar
        // When it's full, change attack index from ISpecialAttackReceiver
        // Disable powerBar

        public void SetSpecialAttack(int attackIndex)
        {
            // Debug.Log("Enable power bar");
            _attackIndex = attackIndex;
            _entityUIUpdate.ShowBars(false,true,true);
            _isAvailable = true;
            _isPowerFull = false;
            _skillComp.SetAttackIndex(0);
            _curPower = 0;
        }

        public void PowerUp()
        {
            if (_isAvailable == false || _isPowerFull)
                return;

            _curPower++;
            if (_curPower >= _maxPower)
            {
                _curPower = 0;
                _isPowerFull = true;
                _skillComp.SetAttackIndex(_attackIndex);
                // Debug.Log("Full power");
            }
        }

        public void ResetPowerBar()
        {
            if (_isAvailable && _isPowerFull)
            {
                _isPowerFull = false;
                _skillComp.SetAttackIndex(0);
                _curPower = 0;
                // Debug.Log("Reset power bar");
            }
        }

        public void DisablePowerBar()
        {
            // Debug.Log("Disable power bar");
            _isAvailable = false;
        }
    }
    
    
    public interface ISkillMonitor
    {
        public void SetSpecialAttack(int attackIndex);
        public void PowerUp();
        public void ResetPowerBar();
        public void DisablePowerBar();
    }
}