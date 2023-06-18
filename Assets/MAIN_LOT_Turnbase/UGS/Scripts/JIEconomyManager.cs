using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;

namespace JumpeeIsland
{
    public class JIEconomyManager : MonoBehaviour
    {
        // Dictionary of all Virtual Purchase transactions ids to lists of costs & rewards.
        private Dictionary<string, (List<JIItemAndAmountSpec> costs, List<JIItemAndAmountSpec> rewards)>
            virtualPurchaseTransactions { get; set; }

        private List<CurrencyDefinition> currencyDefinitions { get; set; }
        private List<InventoryItemDefinition> inventoryItemDefinitions { get; set; }

        [SerializeField] private int _debugCurrencyAmount = 10;
        [SerializeField] private int _debugInventoryAmount = 1;

        private int k_EconomyPurchaseCostsNotMetStatusCode = 10504;
        private List<VirtualPurchaseDefinition> m_VirtualPurchaseDefinitions;

        public async Task RefreshEconomyConfiguration()
        {
            // Calling SyncConfigurationAsync(), will update the cached configuration list (the lists of Currency,
            // Inventory Item, and Purchase definitions) with any definitions that have been published or changed by
            // Economy or overriden by Game Overrides since the last time the player's configuration was cached. It also
            // ensures that other services like Cloud Code are working with the same configuration that has been cached.
            await EconomyService.Instance.Configuration.SyncConfigurationAsync();

            // Check that scene has not been unloaded while processing async wait to prevent throw.
            if (this == null)
                return;

            currencyDefinitions = EconomyService.Instance.Configuration.GetCurrencies();
            inventoryItemDefinitions = EconomyService.Instance.Configuration.GetInventoryItems();
            m_VirtualPurchaseDefinitions = EconomyService.Instance.Configuration.GetVirtualPurchases();
        }

        #region CURRENCY

        public async Task<List<PlayerBalance>> RefreshCurrencyBalances()
        {
            GetBalancesResult balanceResult = null;

            try
            {
                balanceResult = await GetEconomyBalances();
                return balanceResult?.Balances;
            }
            catch (EconomyRateLimitedException e)
            {
                balanceResult = await JIUtils.RetryEconomyFunction(GetEconomyBalances, e.RetryAfter);
            }
            catch (Exception e)
            {
                Debug.Log("Problem getting Economy currency balances:");
                Debug.LogException(e);
            }

            // Check that scene has not been unloaded while processing async wait to prevent throw.
            return null;
        }

        static Task<GetBalancesResult> GetEconomyBalances()
        {
            var options = new GetBalancesOptions {ItemsPerFetch = 100};
            return EconomyService.Instance.PlayerBalances.GetBalancesAsync(options);
        }

        // Add currency when sell a building
        private async Task GrantCurrency(string currencyId, int amount)
        {
            try
            {
                await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(currencyId, amount);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public async void OnGrantCurrency(string currencyId, int amount)
        {
            await GrantCurrency(currencyId, amount);
        }

        #endregion

        #region INVENTORY

        public async Task<List<PlayersInventoryItem>> RefreshInventory()
        {
            GetInventoryResult inventoryResult = null;

            // empty the inventory view first
            Debug.Log("Empty the inventory view first");

            try
            {
                inventoryResult = await GetPlayerInventory();
            }
            catch (EconomyRateLimitedException e)
            {
                inventoryResult = await JIUtils.RetryEconomyFunction(GetPlayerInventory, e.RetryAfter);
            }
            catch (Exception e)
            {
                Debug.Log("Problem getting Economy inventory items:");
                Debug.LogException(e);
            }

            if (this == null || inventoryResult == null)
                return null;

            return inventoryResult.PlayersInventoryItems;
        }

        private Task<GetInventoryResult> GetPlayerInventory()
        {
            var options = new GetInventoryOptions {ItemsPerFetch = 100};
            return EconomyService.Instance.PlayerInventory.GetInventoryAsync(options);
        }

        public List<InventoryItemDefinition> GetInventoryDefinitions()
        {
            return inventoryItemDefinitions;
        }

        // This method is used to help test this Use Case sample by giving some currency to permit
        // transactions to be completed.
        private async Task GrantDebugInventory(string currencyId)
        {
            try
            {
                await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync(currencyId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async Task OnGrantInventory(string inventoryId)
        {
            await GrantDebugInventory(inventoryId);
        }

        #endregion

        #region VIRTUAL PURCHASE

        public void InitializeVirtualPurchaseLookup()
        {
            if (m_VirtualPurchaseDefinitions == null)
            {
                return;
            }

            virtualPurchaseTransactions = new Dictionary<string,
                (List<JIItemAndAmountSpec> costs, List<JIItemAndAmountSpec> rewards)>();

            foreach (var virtualPurchaseDefinition in m_VirtualPurchaseDefinitions)
            {
                var costs = ParseEconomyItems(virtualPurchaseDefinition.Costs);
                var rewards = ParseEconomyItems(virtualPurchaseDefinition.Rewards);

                virtualPurchaseTransactions[virtualPurchaseDefinition.Id] = (costs, rewards);
            }
        }

        List<JIItemAndAmountSpec> ParseEconomyItems(List<PurchaseItemQuantity> itemQuantities)
        {
            var itemsAndAmountsSpec = new List<JIItemAndAmountSpec>();

            foreach (var itemQuantity in itemQuantities)
            {
                var id = itemQuantity.Item.GetReferencedConfigurationItem().Id;
                itemsAndAmountsSpec.Add(new JIItemAndAmountSpec(id, itemQuantity.Amount));
            }

            return itemsAndAmountsSpec;
        }

        public async Task<MakeVirtualPurchaseResult> MakeVirtualPurchaseAsync(string virtualPurchaseId)
        {
            try
            {
                var costs = virtualPurchaseTransactions[virtualPurchaseId].costs;
                foreach (var cost in costs)
                    if (SavingSystemManager.Instance.CheckEnoughCurrency(cost.id, cost.amount) == false)
                        return null;
                
                return await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(virtualPurchaseId);
            }
            catch (EconomyException e)
                when (e.ErrorCode == k_EconomyPurchaseCostsNotMetStatusCode)
            {
                // Rethrow purchase-cost-not-met exception to be handled by shops manager.
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return default;
            }
        }

        public List<JIItemAndAmountSpec> GetVirtualPurchaseCost(string virtualPurchaseId)
        {
            return virtualPurchaseTransactions[virtualPurchaseId].costs;
        }

        #endregion
    }
}