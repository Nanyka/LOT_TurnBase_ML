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

        public async void OnSaveEnvData()
        {
            try
            {
                await _cloudCodeManager.CallSaveEnvData(SavingSystemManager.Instance.GetEnvironmentData());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
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

                Debug.Log("Refresh currencyUI");
            }
            catch (Exception e)
            {
                Debug.Log("There was a problem communicating with the server.");
                Debug.LogException(e);
            }
        }
        
        public void OnSpendOneMove()
        {
            var command = new SpendMove();
            command.Execute(_commandBatchManager,_remoteConfigManager);
            Debug.Log($"Number command: {_commandBatchManager.CountCommand()}");
        }

        // public CommandsCache GetCommandCache()
        // {
        //     return _commandBatchManager.GetCommandCache();
        // }
        //
        // public void RestoreCommands(CommandsCache resetCommand)
        // {
        //     _commandBatchManager.SetCommandCache(resetCommand);
        // }

        #endregion
    }
}
