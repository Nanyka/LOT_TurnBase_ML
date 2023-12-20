using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class MonsterPowerComp : PowerComp
    {
        [SerializeField] private int m_ComboIndex;
        
        private void OnEnable()
        {
            SetSpecialAttack(m_ComboIndex);
        }

        private void OnDisable()
        {
            DisablePowerBar();
        }
    }
}