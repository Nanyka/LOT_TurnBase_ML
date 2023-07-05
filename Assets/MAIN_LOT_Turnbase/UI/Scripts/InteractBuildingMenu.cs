using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class FastUpgrade : IConfirmFunction
    {
        private BuildingEntity _buildingEntity;

        public FastUpgrade(BuildingEntity buildingEntity)
        {
            _buildingEntity = buildingEntity;
        }
        
        public void ClickYes()
        {
            var upgradeExp = _buildingEntity.CalculateUpgradePrice();
            if (SavingSystemManager.Instance.CheckEnoughCurrency("GEM", upgradeExp))
            {
                SavingSystemManager.Instance.DeductCurrency("GEM",upgradeExp);
                _buildingEntity.CollectExp(upgradeExp);
            }
            else
            {
                Debug.Log($"SHOW panel: Not enough GEM for upgrading {_buildingEntity.GetData().EntityName}");
            }
        }

        public Entity GetEntity()
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class InteractBuildingMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _interactBuildingMenu;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] private TextMeshProUGUI _coinText;

        private IConfirmFunction _currentConfirm;
        
        private void Start()
        {
            MainUI.Instance.OnInteractBuildingMenu.AddListener(ShowSellBuildingMenu);
        }

        private void ShowSellBuildingMenu(IConfirmFunction confirmFunction)
        {
            _currentConfirm = confirmFunction;
            _interactBuildingMenu.SetActive(true);
        }

        public void OnClickFastUpgrade()
        {
            var buildingEntity = (BuildingEntity)_currentConfirm.GetEntity();
            var fastUpgrade = new FastUpgrade(buildingEntity);
            _currentConfirm = fastUpgrade;
            ShowConfirmPanel("Cost: "+buildingEntity.CalculateUpgradePrice());
        }

        public void OnClickSell()
        {
            var buildingEntity = (BuildingEntity)_currentConfirm.GetEntity();
            ShowConfirmPanel("Return: "+buildingEntity.CalculateSellingPrice());
        }

        public void OnMakeTheDeal()
        {
            _currentConfirm.ClickYes();
        }

        private void ShowConfirmPanel(string coinText)
        {
            _interactBuildingMenu.SetActive(false);
            _confirmPanel.SetActive(true);
            _coinText.text = coinText;
        }
    }
}