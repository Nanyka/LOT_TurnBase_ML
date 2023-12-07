using System;
using System.Collections.Generic;
using Sirenix.Utilities;
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

        public async void ClickYes()
        {
            if (SavingSystemManager.Instance.CheckEnoughCurrency(_buildingEntity.GetUpgradeCurrency().ToString(),
                    _buildingEntity.GetUpgradePrice()))
            {
                if (_buildingEntity.GetBuildingType() == BuildingType.MAINHALL)
                {
                    try
                    {
                        if (GameFlowManager.Instance.GameMode != GameMode.ECONOMY) return;

                        var upcomingTier = SavingSystemManager.Instance.GetUpcomingTier();

                        foreach (var item in upcomingTier.TierItems)
                        {
                            if (item.inventoryId.IsNullOrWhitespace() == false)
                            {
                                var grantReturn = await SavingSystemManager.Instance.GrantInventory(item.inventoryId);
                                if (grantReturn != null)
                                {
                                    await SavingSystemManager.Instance.RefreshEconomy();
                                    SavingSystemManager.Instance.DeductCurrencyFromBuildings(_buildingEntity.GetUpgradeCurrency().ToString(),
                                        _buildingEntity.GetUpgradePrice());
                                    _buildingEntity.BuildingUpdate();
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                else
                {
                    SavingSystemManager.Instance.DeductCurrencyFromBuildings(_buildingEntity.GetUpgradeCurrency().ToString(), _buildingEntity.GetUpgradePrice());
                    _buildingEntity.BuildingUpdate();
                }
            }
            else
            {
                Debug.Log($"SHOW panel: Not enough {_buildingEntity.GetUpgradeCurrency().ToString()} to upgrade {_buildingEntity.GetData().EntityName}");
            }
        }

        public GameObject GetGameObject()
        {
            return _buildingEntity.gameObject;
        }
    }

    public class InteractBuildingMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _interactBuildingMenu;
        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] private MainHallUpgradeInfo _mainHallUpgrade;
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
            if (_currentConfirm.GetGameObject().TryGetComponent(out BuildingEntity buildingEntity))
            {
                var fastUpgrade = new FastUpgrade(buildingEntity);
                _currentConfirm = fastUpgrade;

                // Display the statistic for the upcoming level of the constructions.
                // If it is MainHall, reveal the types of buildings that can be unlocked
                MainUI.Instance.OnHideAllMenu.Invoke();
                if (buildingEntity.GetBuildingType() == BuildingType.MAINHALL)
                {
                    _mainHallUpgrade.ShowUpgradeInfo(buildingEntity.CalculateUpgradePrice());
                }
                else
                {
                    _confirmMessage.text = "Upgrade the building";
                    ShowConfirmPanel(buildingEntity.CalculateUpgradePrice());
                }
            }
        }

        public void OnClickSell()
        {
            _confirmMessage.text = "Sell it at this price?";
            if (_currentConfirm.GetGameObject().TryGetComponent(out BuildingEntity buildingEntity))
            {
                MainUI.Instance.OnHideAllMenu.Invoke();
                ShowConfirmPanel(buildingEntity.CalculateSellingPrice());
            }
        }

        public void OnMakeTheDeal()
        {
            _currentConfirm.ClickYes();
        }

        private void ShowConfirmPanel(int amount)
        {
            if (_currentConfirm.GetGameObject().TryGetComponent(out BuildingEntity buildingEntity))
            {
                _interactBuildingMenu.SetActive(false);
                _confirmPanel.SetActive(true);
                _currencyGroup.VisualCurrency(buildingEntity.GetUpgradeCurrency().ToString(), amount);
                MainUI.Instance.OnShowAnUI.Invoke();
            }
        }
    }
}