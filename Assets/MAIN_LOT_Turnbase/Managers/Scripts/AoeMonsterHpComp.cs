using UnityEngine;

namespace JumpeeIsland
{
    public class AoeMonsterHpComp : AoeTroopHpComp
    {
        [SerializeField] private CurrencyUnit _reward;

        protected override void Die(IAttackRelated killedByFaction)
        {
            base.Die(killedByFaction);

            if (killedByFaction == null)
                return;

            Debug.Log($"{m_Data.FactionType} kill by {killedByFaction.GetFaction()}");
            if (killedByFaction.GetFaction() == m_Data.FactionType)
                return;

            SavingSystemManager.Instance.IncreaseLocalCurrency(_reward.currencyId,_reward.amount);
            MainUI.Instance.OnShowCurrencyVfx.Invoke(_reward.currencyId,_reward.amount,transform.position);
        }
    }
}