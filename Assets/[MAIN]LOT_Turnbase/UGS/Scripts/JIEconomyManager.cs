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
        readonly Dictionary<string, long> m_CurrencyBalance = new();

        public async Task RefreshEconomyConfiguration()
        {
            // Calling SyncConfigurationAsync(), will update the cached configuration list (the lists of Currency,
            // Inventory Item, and Purchase definitions) with any definitions that have been published or changed by
            // Economy or overriden by Game Overrides since the last time the player's configuration was cached. It also
            // ensures that other services like Cloud Code are working with the same configuration that has been cached.
            await EconomyService.Instance.Configuration.SyncConfigurationAsync();
        }

        // public void SetCurrencyBalance(string currencyId, long balance)
        // {
        //     m_CurrencyBalance[currencyId] = balance;
        //     Debug.Log("Update currency on UI");
        // }

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
    }
}