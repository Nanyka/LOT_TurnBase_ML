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
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Slider strengthSlider;
        [SerializeField] private Slider defendSlider;
        [SerializeField] private SkillIcon[] skills;

        private void Start()
        {
            MainUI.Instance.OnShowInfo.AddListener(ShowUnitInfo);
        }

        private void ShowUnitInfo(IShowInfo infoGetter)
        {
            var info = infoGetter.ShowInfo();

            if (info.entity.TryGetComponent(out CreatureEntity creatureEntity))
            {
                hpSlider.value = creatureEntity.GetStats().HealthPoint;
                strengthSlider.value = creatureEntity.GetStats().Strengh;
                defendSlider.value = creatureEntity.GetStats().Armor;
                foreach (var skill in skills)
                    skill.Deactivate();
                if (info.jump > 0)
                    skills[info.jump - 1].Active();
            }

            // _unitInfoText.text = infoGetter.ShowInfo();
        }
    }
}