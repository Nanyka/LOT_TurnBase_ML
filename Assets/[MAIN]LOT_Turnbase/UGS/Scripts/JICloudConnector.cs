using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Economy.Model;
using Unity.Services.Samples.IdleClickerGame;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class JICloudConnector : MonoBehaviour
    {
        [SerializeField] private JIEconomyManager _economyManager;
        [SerializeField] private JICloudCodeManager _cloudCodeManager;
        [SerializeField] private JISimulatedCurrencyManager _simulatedCurrencyManager;
        [SerializeField] private JICommandBatchSystem _commandBatchManager;
        [SerializeField] private JIRemoteConfigManager _remoteConfigManager;
        
        private async void OnDisable()
        {
            await Task.WhenAll(
                _cloudCodeManager.CallSaveEnvData(SavingSystemManager.Instance.GetEnvironmentData()),
                HandleCommandBatch()
            ) ;
        }

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
        
        public async void OnResetEnvData()
        {
            try
            {
                await _cloudCodeManager.CallResetStateEndpoint();
                StartUpProcessor.Instance.OnResetData.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
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
        
        private async Task HandleCommandBatch()
        {
            try
            {
                await _commandBatchManager.FlushBatch(_cloudCodeManager);
                if (this == null) return;

                await FetchUpdatedServicesData();
                if (this == null) return;

                Debug.Log("Flush all command to cloud");
            }
            catch (Exception e)
            {
                Debug.Log("There was a problem communicating with the server.");
                Debug.LogException(e);
            }
        }
        
        public void OnCommandStackUp(CommandName commandName)
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

            if (command != null) command.Execute(_commandBatchManager, _remoteConfigManager);
        }

        public CommandsCache GetCommands()
        {
            return _commandBatchManager.GetCommandsForSaving();
        }

        public async void SubmitCommands(List<CommandName> commandNames)
        {
            await _commandBatchManager.SubmitListCommands(commandNames, _cloudCodeManager);
        }

        #endregion
    }
}
