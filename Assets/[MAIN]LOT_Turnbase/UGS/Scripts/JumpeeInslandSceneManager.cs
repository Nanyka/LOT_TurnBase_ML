using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Samples.IdleClickerGame;
using UnityEngine;

namespace JumpeeIsland
{
    public class JumpeeInslandSceneManager : MonoBehaviour
    {
        [SerializeField] private JIEconomyManager _economyManager;
        [SerializeField] private JICloudCodeManager _cloudCodeManager;
        [SerializeField] private JISimulatedCurrencyManager _simulatedCurrencyManager;
        
        const string m_MoveCurrency = "MOVE";
        
        async void Start()
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

                // await GetUpdatedState();
                // if (this == null)
                //     return;

                // await _economyManager.RefreshCurrencyBalances();
                // if (this == null)
                //     return;

                ShowStateAndStartSimulating();

                Debug.Log("Set UI interactable");

                Debug.Log("Initialization and signin complete.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        async Task GetUpdatedState()
        {
            try
            {
                var updatedState = await _cloudCodeManager.CallGetUpdatedStateEndpoint();
                if (this == null)
                    return;

                UpdateState(updatedState);
                Debug.Log($"Starting State: {updatedState}");
            }
            catch (CloudCodeResultUnavailableException)
            {
                // Exception already handled by CloudCodeManager
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        void UpdateState(EnvironmentData updatedState)
        {
            Debug.Log($"Get cloudSave data with timestamp: {updatedState.timestamp}");
            
            // _economyManager.SetCurrencyBalance(m_MoveCurrency, updatedState.currencyBalance);
            // _simulatedCurrencyManager.UpdateServerTimestampOffset(updatedState.timestamp);

            // m_AllWells[0] = SetWellLevels(updatedState.wells_level1, 1);
            // m_AllWells[1] = SetWellLevels(updatedState.wells_level2, 2);
            // m_AllWells[2] = SetWellLevels(updatedState.wells_level3, 3);
            // m_AllWells[3] = SetWellLevels(updatedState.wells_level4, 4);
            //
            // m_Obstacles = updatedState.obstacles;
        }
        
        void ShowStateAndStartSimulating()
        {
            Debug.Log("Show state on game");
        }

        #region TESTING

        public async void OnSaveEnvData()
        {
            try
            {
                await _cloudCodeManager.CallSaveEnvData(SavingSystemManager.Instance.LoadEnvironment());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public async void OnLoadEnvData()
        {
            try
            {
                var returnEnvData = await _cloudCodeManager.CallGetUpdatedStateEndpoint();
                UpdateState(returnEnvData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion
    }
}
