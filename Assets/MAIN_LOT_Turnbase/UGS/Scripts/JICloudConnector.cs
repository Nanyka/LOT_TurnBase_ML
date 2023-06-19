using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Economy.Model;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Samples.IdleClickerGame;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class JICloudConnector : MonoBehaviour
    {
        [SerializeField] protected JIEconomyManager _economyManager;
        [SerializeField] private JICloudCodeManager _cloudCodeManager;
        [SerializeField] private JICommandBatchSystem _commandBatchManager;
        [SerializeField] private JIRemoteConfigManager _remoteConfigManager;
        [SerializeField] private JILeaderboardManager _leaderboardManager;

        public async Task Init()
        {
            try
            {
                var options = new InitializationOptions();
                options.SetEnvironmentName("dev");
                await UnityServices.InitializeAsync(options);

                // Check that scene has not been unloaded while processing async wait to prevent throw.
                if (this == null)
                    return;

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    if (this == null)
                        return;
                }

                Debug.Log($"Player id:{AuthenticationService.Instance.PlayerId}");
                
                await _economyManager.RefreshEconomyConfiguration();
                if (this == null)
                    return;

                await FetchUpdatedServicesData();
                if (this == null) return;

                Debug.Log("Set UI interactable");

                Debug.Log("Initialization and signin complete.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        async Task<EnvironmentData> GetUpdatedState()
        {
            try
            {
                var updatedState = await _cloudCodeManager.CallGetUpdatedStateEndpoint();
                if (this == null)
                    return null;

                return updatedState;
            }
            catch (CloudCodeResultUnavailableException)
            {
                // Exception already handled by CloudCodeManager
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            return null;
        }
        
        async Task FetchUpdatedServicesData()
        {
            await Task.WhenAll(
                _economyManager.RefreshCurrencyBalances(),
                _remoteConfigManager.FetchConfigs()
                // CloudSaveManager.instance.LoadAndCacheData()
            );
        }

        #region ENVIRONMENT DATA

        public async Task OnSaveEnvData()
        {
            await _cloudCodeManager.CallSaveEnvData(SavingSystemManager.Instance.GetEnvDataForSave());
        }

        public async Task<EnvironmentData> OnLoadEnvData()
        {
            try
            {
                var returnEnvData = await GetUpdatedState();
                return returnEnvData;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }
        
        public async Task<EnvironmentData> OnResetEnvData()
        {
            try
            {
                var returnEnvData = await _cloudCodeManager.CallResetStateEndpoint();
                return returnEnvData;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        #endregion
        
        #region CURRENCY DATA

        public async Task<List<PlayerBalance>> OnLoadCurrency()
        {
            try
            {
                var currencyList = await _economyManager.RefreshCurrencyBalances();
                return currencyList;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }
        
        // private async Task HandleCommandBatch()
        // {
        //     try
        //     {
        //         await _commandBatchManager.FlushBatch(_cloudCodeManager);
        //         if (this == null) return;
        //
        //         await FetchUpdatedServicesData();
        //         if (this == null) return;
        //
        //         Debug.Log("Flush all command to cloud");
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.Log("There was a problem communicating with the server.");
        //         Debug.LogException(e);
        //     }
        // }
        
        public void OnCommandStackUp(CommandName commandName, bool isEconomyDirectInteract)
        {
            var command = GetCommandByName(commandName);

            if (command != null)
            {
                command.Execute(_commandBatchManager, _remoteConfigManager);
                if (isEconomyDirectInteract)
                    command.ProcessCommandLocally(_remoteConfigManager);
            }
        }
        
        private JICommand GetCommandByName(CommandName commandName)
        {
            JICommand command = null;
            switch (commandName)
            {
                case CommandName.JI_SPEND_MOVE:
                    command = new SpendMove();
                    break;
                case CommandName.JI_NEUTRAL_WOOD_1_0:
                    command = new NeutralWood010();
                    break;
                case CommandName.JI_NEUTRAL_FOOD_1_0:
                    command = new NeutralFood010();
                    break;
            }

            return command;
        }

        public CommandsCache GetCommands()
        {
            return _commandBatchManager.GetCommandsForSaving();
        }

        public async void SubmitCommands(CommandsCache commandCache)
        {
            await _commandBatchManager.SubmitListCommands(commandCache, _cloudCodeManager, _remoteConfigManager);
        }

        public void OnGrantCurrency(string currencyId, int amount)
        {
            _economyManager.OnGrantCurrency(currencyId, amount);
        }

        #endregion

        #region INVENTORY DATA

        public async Task OnResetBasicInventory(List<string> basicInventory)
        {
            await _economyManager.ResetInventory();
            
            foreach (var inventoryId in basicInventory)
                OnGrantInventory(inventoryId);
        }
        
        public async Task<List<PlayersInventoryItem>> OnLoadInventory()
        {
            try
            {
                var inventories = await _economyManager.RefreshInventory();
                return inventories;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
        
        public JIInventoryItem ConvertToInventoryItem(EntityData entityData)
        {
            var inventoryDefinitions = _economyManager.GetInventoryDefinitions();
            foreach (var itemDefinition in inventoryDefinitions)
            {
                if (itemDefinition.Name.Equals(entityData.EntityName))
                    return itemDefinition.CustomDataDeserializable.GetAs<JIInventoryItem>();
            }

            return null;
        }

        private async void OnGrantInventory(string inventoryId)
        {
            await _economyManager.OnGrantInventory(inventoryId);
        }

        #endregion

        #region VIRTUAL PURCHASE DATA

        public void OnLoadVirtualPurchase()
        {
            _economyManager.InitializeVirtualPurchaseLookup();
        }

        public async Task<MakeVirtualPurchaseResult> OnMakeAPurchase(string virtualPurchaseId)
        {
            return await _economyManager.MakeVirtualPurchaseAsync(virtualPurchaseId);
        }
        
        public List<JIItemAndAmountSpec> GetVirtualPurchaseCost(string virtualPurchaseId)
        {
            return _economyManager.GetVirtualPurchaseCost(virtualPurchaseId);
        }

        #endregion

        #region REMOTE CONFIG

        public List<JIRemoteConfigManager.Reward> GetRewardByCommandId(string commandId)
        {
            return _remoteConfigManager.commandRewards[commandId];
        }

        #endregion

        #region LEADERBOARD

        public async Task<EnvironmentData> GetEnemyEnvironment()
        {
            var getPlayerRange = await _leaderboardManager.GetPlayerRange();
            return await _cloudCodeManager.CallGetEnemyEnvironment(getPlayerRange[Random.Range(0,getPlayerRange.Count)].PlayerId);
        }

        #endregion
    }
}
