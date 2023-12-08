using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class PowerComp : MonoBehaviour, ISkillMonitor
    {
        private ISpecialAttackReceiver _skillReceiver;
        private IEntityUIUpdate _entityUIUpdate;
        private int _attackIndex;
        private int maxPower = 4; // 3 normal attack allow 1 special attack
        private int _curPower;
        private bool _isAvailable;
        private bool _isPowerFull;

        private void Start()
        {
            _skillReceiver = GetComponent<ISpecialAttackReceiver>();
            _entityUIUpdate = GetComponent<IEntityUIUpdate>();
        }

        // Enable powerBar when specialAttack is unlocked
        // Fill up the powerBar
        // When it's full, change attack index from ISpecialAttackReceiver
        // Disable powerBar

        public void SetSpecialAttack(int attackIndex)
        {
            _attackIndex = attackIndex;
            Debug.Log("Enable power bar");
            _entityUIUpdate.ShowBars(false,true,true);
            _isAvailable = true;
            ResetPowerBar();
        }

        public void PowerUp()
        {
            if (_isAvailable == false || _isPowerFull)
                return;

            _curPower++;
            if (_curPower >= maxPower)
            {
                _isPowerFull = true;
                _skillReceiver.SetAttackIndex(_attackIndex);
                Debug.Log("Full power");
            }
        }

        public void ResetPowerBar()
        {
            if (_isAvailable && _isPowerFull)
            {
                _isPowerFull = false;
                _skillReceiver.SetAttackIndex(0);
                _curPower = 0;
                Debug.Log("Reset power bar");
            }
        }

        public void DisablePowerBar()
        {
            Debug.Log("Disable power bar");
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