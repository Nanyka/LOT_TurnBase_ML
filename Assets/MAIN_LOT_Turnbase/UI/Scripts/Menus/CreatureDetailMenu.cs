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
        [SerializeField] private CurrencyGroup upgradeButton;
        [SerializeField] private TextMeshProUGUI nextLevelText;
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
            strengthSlider.SetSlider(creatureStats.Strength);
            defenseSlider.SetSlider(creatureStats.Armor);
            hpSlider.SetSlider(creatureStats.HealthPoint);
            for (int i = 0; i < skills.Length; i++)
                skills[i].ShowSkill(i > creatureEntity.GetData().CurrentLevel);
            upgradeButton.VisualCurrency("COIN",_currentCreature.GetUpgradeCost());
            nextLevelText.text = $"Lv{_currentCreature.GetData().CurrentLevel + 1} Upgrade";
            
            creatureDetailMenu.SetActive(true);
        }

        public void ShowUpgradePanel()
        {
            if (_currentCreature.CheckMaxLevel())
            {
                MainUI.Instance.OnConversationUI.Invoke("This troop reach max level", true);
                return;
            }

            upgradeCost.VisualCurrency("COIN",_currentCreature.GetUpgradeCost());
            confirmPanel.SetActive(true);
            MainUI.Instance.OnShowAnUI.Invoke();
        }

        public void UpgradeCreature()
        {
            var creatureLevel = SavingSystemManager.Instance.GetInventoryLevel(_currentCreature.GetData().EntityName);
            Debug.Log($"Upgrade {_currentCreature.GetData().EntityName} to level {creatureLevel+1}");
            SavingSystemManager.Instance.UpgradeInventory(_currentCreature.GetData().EntityName, creatureLevel+1);
        }
    }
}