using System;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class EcoBossInfoUI : MonoBehaviour
    {
        [SerializeField] private GameObject infoMenu;
        [SerializeField] private Image entityIcon;
        [SerializeField] private Slider hpSlider;

        private void Start()
        {
            MainUI.Instance.OnBossMapProfitUpdate.AddListener(UpdateEcoBossState);
        }
        
        private void UpdateEcoBossState(float healthPortion)
        {
            if (healthPortion <= 0f)
            {
                infoMenu.SetActive(false);
                return;
            }
            
            hpSlider.value = healthPortion;
        }

        public void TurnOn(string spriteAddress)
        {
            entityIcon.sprite = AddressableManager.Instance.GetAddressableSprite(spriteAddress);
            infoMenu.SetActive(true);
        }
    }
}