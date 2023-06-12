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
        public Dictionary<string, (List<JIItemAndAmountSpec> costs, List<JIItemAndAmountSpec> rewards)>
            virtualPurchaseTransactions { get; private set; }

        public List<CurrencyDefinition> currencyDefinitions { get; private set; }
        public List<InventoryItemDefinition> inventoryItemDefinitions { get; private set; }

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

        // This method is used to help test this Use Case sample by giving some currency to permit
        // transactions to be completed.
        private async Task GrantDebugCurrency(string currencyId)
        {
            try
            {
                await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(currencyId, _debugCurrencyAmount);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public async void OnGrantCurrency(string currencyId)
        {
            await GrantDebugInventory(currencyId);
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
                inventoryResult = await GetEconomyPlayerInventory();
            }
            catch (EconomyRateLimitedException e)
            {
                inventoryResult = await JIUtils.RetryEconomyFunction(GetEconomyPlayerInventory, e.RetryAfter);
            }
            catch (Exception e)
            {
                Debug.Log("Problem getting Economy inventory items:");
                Debug.LogException(e);
            }

            if (this == null || inventoryResult == null)
                return null;
            
            // if (inventoryResult.PlayersInventoryItems[0].GetItemDefinition().CustomDataDeserializable.GetAs<Dictionary<string, string>>() is
            //         { } customData && customData.TryGetValue("skinAddress", out var skinAddress))
            // {
            //     Debug.Log($"Get inventory addressable asset: {skinAddress}");
            // }

            return inventoryResult.PlayersInventoryItems;
        }

        private Task<GetInventoryResult> GetEconomyPlayerInventory()
        {
            var options = new GetInventoryOptions {ItemsPerFetch = 100};
            return EconomyService.Instance.PlayerInventory.GetInventoryAsync(options);
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

        public async void OnGrantInventory(string inventoryId)
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

            foreach (var transaction in virtualPurchaseTransactions)
            {
                Debug.Log($"Virtual purchase Id: {transaction.Key}" +
                          $"Reward: {transaction.Value.rewards[0].id}, Amount: {transaction.Value.rewards[0].amount}" +
                          $"Cost: {transaction.Value.costs[0].id}, Amount: {transaction.Value.costs[0].amount}");
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

        #endregion
    }
}