using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class EcoBossInfoComp : MonoBehaviour
    {
        private HealthComp m_HealthComp;

        private void OnEnable()
        {
            m_HealthComp = GetComponent<HealthComp>();
            m_HealthComp.TakeDamageEvent.AddListener(UpdateEcoBossInfo);
        }

        private void Start()
        {
            MainUI.Instance.TurnOnEcoBossInfo(SavingSystemManager.Instance
                .GetInventoryItemByName(GetComponent<CreatureEntity>().GetData().EntityName).spriteAddress);
        }

        private void OnDisable()
        {
            m_HealthComp.TakeDamageEvent.RemoveListener(UpdateEcoBossInfo);
        }

        private void UpdateEcoBossInfo(float healthPortion)
        {
            MainUI.Instance.OnEcoBossInfo.Invoke(healthPortion);
        }
    }
}