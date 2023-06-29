using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Economy.Model;
using Unity.Services.Samples.IdleClickerGame;
using UnityEngine;
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
                var updatedState = await _cloudCodeManager.CallLoadUpdatedStateEndpoint();
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
                OnLoadInventory(),
                _remoteConfigManager.FetchConfigs()
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

        public void OnCommandStackUp(CommandName commandName)
        {
            var command = GetCommandByName(commandName);

            if (command != null)
            {
                command.Execute(_commandBatchManager, _remoteConfigManager);
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
                case CommandName.JI_FOOD_1:
                    command = new Food1();
                    break;
                case CommandName.JI_FOOD_5:
                    command = new Food5();
                    break;
                case CommandName.JI_FOOD_20:
                    command = new Food20();
                    break;
                case CommandName.JI_FOOD_50:
                    command = new Food50();
                    break;
                case CommandName.JI_FOOD_100:
                    command = new Food100();
                    break;
                case CommandName.JI_FOOD_200:
                    command = new Food200();
                    break;
                case CommandName.JI_FOOD_500:
                    command = new Food500();
                    break;
                case CommandName.JI_WOOD_1:
                    command = new Wood1();
                    break;
                case CommandName.JI_WOOD_5:
                    command = new Wood5();
                    break;
                case CommandName.JI_WOOD_20:
                    command = new Wood20();
                    break;
                case CommandName.JI_WOOD_50:
                    command = new Wood50();
                    break;
                case CommandName.JI_WOOD_100:
                    command = new Wood100();
                    break;
                case CommandName.JI_WOOD_200:
                    command = new Wood200();
                    break;
                case CommandName.JI_WOOD_500:
                    command = new Wood500();
                    break;
                case CommandName.JI_COIN_1:
                    command = new Coin1();
                    break;
                case CommandName.JI_COIN_5:
                    command = new Coin5();
                    break;
                case CommandName.JI_COIN_20:
                    command = new Coin20();
                    break;
                case CommandName.JI_COIN_50:
                    command = new Coin50();
                    break;
                case CommandName.JI_COIN_100:
                    command = new Coin100();
                    break;
                case CommandName.JI_COIN_200:
                    command = new Coin200();
                    break;
                case CommandName.JI_COIN_500:
                    command = new Coin500();
                    break;
                case CommandName.JI_GOLD_1:
                    command = new Gold1();
                    break;
                case CommandName.JI_GOLD_5:
                    command = new Gold5();
                    break;
                case CommandName.JI_GOLD_20:
                    command = new Gold20();
                    break;
                case CommandName.JI_GOLD_50:
                    command = new Gold50();
                    break;
                case CommandName.JI_GOLD_100:
                    command = new Gold100();
                    break;
                case CommandName.JI_GOLD_200:
                    command = new Gold200();
                    break;
                case CommandName.JI_GOLD_500:
                    command = new Gold500();
                    break;
                case CommandName.JI_GEM_1:
                    command = new Gem1();
                    break;
                case CommandName.JI_GEM_5:
                    command = new Gem5();
                    break;
                case CommandName.JI_GEM_20:
                    command = new Gem20();
                    break;
                case CommandName.JI_GEM_50:
                    command = new Gem50();
                    break;
                case CommandName.JI_GEM_100:
                    command = new Gem100();
                    break;
                case CommandName.JI_GEM_200:
                    command = new Gem200();
                    break;
                case CommandName.JI_GEM_500:
                    command = new Gem500();
                    break;
            }

            return command;
        }

        public CommandsCache GetCommands()
        {
            return _commandBatchManager.GetCommandsForSaving();
        }

        public async Task SubmitCommands(CommandsCache commandCache)
        {
            await _commandBatchManager.SubmitListCommands(commandCache, _cloudCodeManager, _remoteConfigManager);
        }

        public void OnGrantCurrency(string currencyId, int amount)
        {
            _economyManager.OnGrantCurrency(currencyId, amount);
        }

        public async void DeductCurrency(string currencyId, int amount)
        {
            await _economyManager.DeductCurrency(currencyId, amount);
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
        
        public JIInventoryItem GetInventoryByNameOrId(string inventoryInfo)
        {
            var inventoryDefinitions = _economyManager.GetInventoryDefinitions();
            
            foreach (var itemDefinition in inventoryDefinitions)
                if (itemDefinition.Name.Equals(inventoryInfo) || itemDefinition.Id.Equals(inventoryInfo))
                    return itemDefinition.CustomDataDeserializable.GetAs<JIInventoryItem>();

            return null;
        }

        public async void OnGrantInventory(string inventoryId)
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
        
        public VirtualPurchaseDefinition GetPurchaseDefinition(string id)
        {
            return _economyManager.GetPurchaseDefinition(id);
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
            return await _cloudCodeManager.CallLoadEnemyEnvironment(getPlayerRange[Random.Range(0,getPlayerRange.Count)].PlayerId);
        }

        public void PlayerRecordScore(int playerScore)
        {
            _leaderboardManager.AddScore(playerScore);
        }

        #endregion

        #region GAME PROCESS

        public async Task<GameProcessData> OnLoadGameProcess()
        {
            try
            {
                var gameProcess = await _cloudCodeManager.CallLoadGameProcess();
                if (this == null)
                    return null;

                return gameProcess;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            return null;
        }
        
        public async Task OnSaveGameProcess(GameProcessData currentProcess)
        {
            await _cloudCodeManager.CallSaveGameProcess(currentProcess);
        }

        #endregion
    }
}
