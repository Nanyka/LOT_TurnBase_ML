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
            if (SavingSystemManager.Instance.CheckEnoughCurrency("GEM", _buildingEntity.GetUpgradePrice()))
            {
                SavingSystemManager.Instance.DeductCurrencyFromBuildings("GEM",_buildingEntity.GetUpgradePrice());
                _buildingEntity.BuildingUpdate();
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
        [SerializeField] private CurrencyGroup _currencyGroup;
        [SerializeField] private TextMeshProUGUI _confirmMessage;

        private IConfirmFunction _currentConfirm;
        
        private void Start()
        {
            MainUI.Instance.OnInteractBuildingMenu.AddListener(ShowInteractBuildingMenu);
            MainUI.Instance.OnHideAllMenu.AddListener(HideInteractMenu);
        }

        private void HideInteractMenu()
        {
            _interactBuildingMenu.SetActive(false);
        }

        private void ShowInteractBuildingMenu(IConfirmFunction confirmFunction)
        {
            _currentConfirm = confirmFunction;
            _interactBuildingMenu.SetActive(true);
        }

        public void OnClickFastUpgrade()
        {
            var buildingEntity = (BuildingEntity)_currentConfirm.GetEntity();
            var fastUpgrade = new FastUpgrade(buildingEntity);
            _currentConfirm = fastUpgrade;
            _confirmMessage.text = "Upgrade the building";
            ShowConfirmPanel(buildingEntity.CalculateUpgradePrice());
        }

        public void OnClickSell()
        {
            _confirmMessage.text = "Sell it at this price?";
            var buildingEntity = (BuildingEntity)_currentConfirm.GetEntity();
            ShowConfirmPanel(buildingEntity.CalculateSellingPrice());
        }

        public void OnMakeTheDeal()
        {
            _currentConfirm.ClickYes();
        }

        private void ShowConfirmPanel(int amount)
        {
            _interactBuildingMenu.SetActive(false);
            _confirmPanel.SetActive(true);
            _currencyGroup.VisualCurrency("GEM" ,amount);
            MainUI.Instance.OnShowAnUI.Invoke();
        }
    }
}