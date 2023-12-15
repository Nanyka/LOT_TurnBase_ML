using System;

namespace JumpeeIsland
{
    public class MonsterPowerComp : PowerComp
    {
        private void OnEnable()
        {
            SetSpecialAttack(1);
        }

        private void OnDisable()
        {
            DisablePowerBar();
        }
    }
}