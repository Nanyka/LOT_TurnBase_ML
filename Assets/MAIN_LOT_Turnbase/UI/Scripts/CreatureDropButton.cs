using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class CreatureDropButton : CreatureBuyButton
    {
        [SerializeField] private Slider _amountSlider;
        [SerializeField] private TextMeshProUGUI _amountText;

        public override void TurnOn(JIInventoryItem creatureItem, CreatureMenu creatureMenu)
        {
            base.TurnOn(creatureItem, creatureMenu);

            var itemAmount = creatureMenu.GetAmountById(creatureItem.inventoryName);
            _amountSlider.maxValue = itemAmount;
            RefreshSlider();
        }

        private void RefreshSlider()
        {
            _amountSlider.value = m_CreatureMenu.GetAmountById(m_CreatureItem.inventoryName);
            _amountText.text = $"{_amountSlider.value}/{_amountSlider.maxValue}";
        }

        public override void ClickYes()
        {
            SavingSystemManager.Instance.OnTrainACreature(m_CreatureItem,_spawnPosition, false);
            
            // Remove item from CreatureMenu
            m_CreatureMenu.DecreaseAmount(m_CreatureItem.inventoryName);
            RefreshSlider();
            m_CreatureMenu.CheckEmptyMenu();
        }
    }
}