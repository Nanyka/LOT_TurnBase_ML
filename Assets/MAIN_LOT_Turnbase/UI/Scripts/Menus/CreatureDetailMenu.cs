using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class CreatureDetailMenu : MonoBehaviour
    {
        [SerializeField] private GameObject creatureDetailMenu;
        [SerializeField] private GameObject confirmPanel;
        [SerializeField] private Image creatureIcon;
        [SerializeField] private TextMeshProUGUI creatureName;
        [SerializeField] private StatSlider strengthSlider;
        [SerializeField] private StatSlider defenseSlider;
        [SerializeField] private StatSlider hpSlider;
        [SerializeField] private CurrencyGroup upgradeCost;
        [SerializeField] private SkillIcon[] skills;

        private CreatureEntity _currentCreature;

        private void Start()
        {
            MainUI.Instance.OnShowCreatureDetails.AddListener(ShowSelectedCreature);
        }

        private void ShowSelectedCreature(CreatureEntity creatureEntity)
        {
            _currentCreature = creatureEntity;
            
            creatureIcon.sprite = AddressableManager.Instance.GetAddressableSprite(SavingSystemManager.Instance
                .GetInventoryItemByName(creatureEntity.GetData().EntityName).spriteAddress);
            creatureName.text = creatureEntity.GetData().EntityName;
            var creatureStats = creatureEntity.GetStats();
            strengthSlider.SetSlider(creatureStats.Strengh);
            defenseSlider.SetSlider(creatureStats.Armor);
            hpSlider.SetSlider(creatureStats.HealthPoint);
            for (int i = 0; i < skills.Length; i++)
                skills[i].ShowSkill(i > creatureEntity.GetData().CurrentLevel);
            
            creatureDetailMenu.SetActive(true);
        }

        public void ShowUpgradePanel()
        {
            upgradeCost.VisualCurrency(_currentCreature.GetUpgradeCost().ToString());
            confirmPanel.SetActive(true);
        }

        public void UpgradeCreature()
        {
            var creatureLevel = SavingSystemManager.Instance.GetInventoryLevel(_currentCreature.GetData().EntityName);
            Debug.Log($"Upgrade {_currentCreature.GetData().EntityName} to level {creatureLevel+1}");
            SavingSystemManager.Instance.UpgradeInventory(_currentCreature.GetData().EntityName, creatureLevel+1);
        }
    }
}