using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class CreatureInfoUI : MonoBehaviour
    {
        [SerializeField] private GameObject infoMenu;
        [SerializeField] private Image entityIcon;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Slider strengthSlider;
        [SerializeField] private Slider defendSlider;
        [SerializeField] private SkillIcon[] skills;

        private Entity _selectedEntity;

        private void Start()
        {
            MainUI.Instance.OnShowInfo.AddListener(ShowUnitInfo);
            MainUI.Instance.OnHideAllMenu.AddListener(HideInfoMenu);
        }

        private void HideInfoMenu()
        {
            infoMenu.SetActive(false);
        }

        private void ShowUnitInfo(IShowInfo infoGetter)
        {
            MainUI.Instance.OnHideAllMenu.Invoke();

            var info = infoGetter.ShowInfo();
            _selectedEntity = info.entity;

            if (info.entity.TryGetComponent(out CreatureEntity creatureEntity))
            {
                var creatureItem =
                    SavingSystemManager.Instance.GetInventoryItemByName(creatureEntity.GetData().EntityName);
                entityIcon.sprite = AddressableManager.Instance.GetAddressableSprite(creatureItem.spriteAddress);
                hpSlider.value = creatureEntity.GetData().CurrentHp;
                strengthSlider.value = creatureEntity.GetStats().Strengh;
                defendSlider.value = creatureEntity.GetStats().Armor;
                foreach (var skill in skills)
                    skill.Deactivate();
                if (info.jump > 0)
                    skills[info.jump - 1].Active();

                infoMenu.SetActive(true);
            }

            if (info.entity.TryGetComponent(out BuildingEntity buildingEntity))
            {
                var buildingItem =
                    SavingSystemManager.Instance.GetInventoryItemByName(buildingEntity.GetData().EntityName);
                entityIcon.sprite = AddressableManager.Instance.GetAddressableSprite(buildingItem.spriteAddress);
                hpSlider.value = buildingEntity.GetData().CurrentHp;
                strengthSlider.value = buildingEntity.GetStats().AttackDamage;
                defendSlider.value = buildingEntity.GetStats().Shield;
                foreach (var skill in skills)
                    skill.Deactivate();

                infoMenu.SetActive(true);

                var buildingInGame = buildingEntity.GetComponent<BuildingInGame>();
                MainUI.Instance.OnInteractBuildingMenu.Invoke(buildingInGame);
            }

            MainUI.Instance.OnShowAnUI.Invoke();
        }

        public void OnClickCharacterIcon()
        {
            if (_selectedEntity == null || _selectedEntity.GetData().FactionType != FactionType.Player)
                return;
            
            if (_selectedEntity.TryGetComponent(out CreatureEntity creatureEntity))
                MainUI.Instance.OnShowCreatureDetails.Invoke(creatureEntity);
        }

        public Entity GetSelectedEntity()
        {
            return _selectedEntity;
        }
    }
}