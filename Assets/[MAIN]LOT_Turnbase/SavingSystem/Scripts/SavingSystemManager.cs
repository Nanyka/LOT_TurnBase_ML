using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

namespace JumpeeIsland
{
    public enum SavingPath
    {
        PlayerEnvData,
        CommandBatchCurrencies
    }

    [RequireComponent(typeof(EnvironmentLoader))]
    [RequireComponent(typeof(CurrencyLoader))]
    public class SavingSystemManager : Singleton<SavingSystemManager>
    {
        // Save Player environment data whenever a creature move
        [NonSerialized] public UnityEvent OnSavePlayerEnvData = new(); // invoke at CreatureEntity
        [NonSerialized] public UnityEvent OnUseOneMove = new(); // invoke at EnvironmentManager

        [SerializeField] private JICloudConnector cloudConnector;
        private EnvironmentLoader _envLoader;
        private CurrencyLoader _currencyLoader;

        private string _gamePath;
        private bool encrypt = true;

        protected override void Awake()
        {
            base.Awake();
            _gamePath = Application.persistentDataPath;
            _envLoader = GetComponent<EnvironmentLoader>();
            _currencyLoader = GetComponent<CurrencyLoader>();
            OnSavePlayerEnvData.AddListener(SavePlayerEnv);
            OnUseOneMove.AddListener(SpendOneMove);
            StartUpProcessor.Instance.OnLoadData.AddListener(StartUpLoadData);
            StartUpProcessor.Instance.OnResetData.AddListener(ResetData);
        }

        private void OnDisable()
        {
            Debug.Log($"Time from start game {Mathf.RoundToInt(Time.realtimeSinceStartup*1000000)}");
            _envLoader.GetData().timestamp += Mathf.RoundToInt(Time.realtimeSinceStartup * 1000000);
            SavePlayerEnv();
            // SaveCommandBatch(cloudConnector.GetCommandCache());
        }

        private async void StartUpLoadData()
        {
            await LoadEnvironment();
            _envLoader.Init();

            await LoadCurrencies();
            _currencyLoader.Init();
            
            StartUpProcessor.Instance.OnStartGame.Invoke(_currencyLoader.GetMoveAmount());
        }

        private void ResetData()
        {
            _envLoader.ResetData();
            StartUpLoadData();
        }

        #region ENVIRONMENT

        private void SavePlayerEnv()
        {
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Save(_envLoader.GetData(), envPath, DataWasSaved, encrypt);
        }

        private void DataWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
            {
                Debug.LogError($"Error saving data:\n{result}\n{message}");
            }
        }

        private async Task LoadEnvironment()
        {
            // Authenticate on UGS and get envData
            await cloudConnector.Init();
            _envLoader.SetData(await cloudConnector.OnLoadEnvData());
            
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Load<EnvironmentData>(envPath, EnvWasLoaded, encrypt);
        }

        private void EnvWasLoaded(EnvironmentData data, SaveResult result, string message)
        {
            Debug.Log($"Data Was Loaded:\n{result}\n{message}");

            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No Data File Found -> Creating new data...");

            if (result == SaveResult.Success)
            {
                // Save current timestamp as a cache data to update data.timestamp if it is assigned as envData
                var dummyLastTimestamp = _envLoader.GetData().lastTimestamp;
                // Check if the last session is not Internet connection
                if (_envLoader.GetData().lastTimestamp < data.timestamp)
                {
                    data.lastTimestamp = dummyLastTimestamp;
                    _envLoader.SetData(data);
                }
            }
        }

        #endregion

        #region CURRENCIES
        
        private void SaveCommandBatch(CommandsCache commandsCache)
        {
            commandsCache.timestamp = _envLoader.GetData().timestamp;
            var envPath = GetSavingPath(SavingPath.CommandBatchCurrencies);
            SaveManager.Instance.Save(commandsCache, envPath, CommandBatchWasSaved, encrypt);
        }

        private void CommandBatchWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
            {
                Debug.LogError($"Error saving data:\n{result}\n{message}");
            }
        }

        private async Task LoadCurrencies()
        {
            // Authenticate on UGS and get envData
            _currencyLoader.SetData(await cloudConnector.OnLoadCurrency());
            
            // var commandPath = GetSavingPath(SavingPath.CommandBatchCurrencies);
            // SaveManager.Instance.Load<CommandsCache>(commandPath, CommandWasLoaded, encrypt);
        }

        // private void CommandWasLoaded(CommandsCache commands, SaveResult result, string message)
        // {
        //     Debug.Log($"Data Was Loaded:\n{result}\n{message}");
        //
        //     if (result == SaveResult.EmptyData || result == SaveResult.Error)
        //         Debug.LogError("No Data File Found -> Creating new data...");
        //
        //     if (result == SaveResult.Success)
        //     {
        //         // Compare timestamp. Select the latest data
        //         cloudConnector.RestoreCommands(_envLoader.GetData().lastTimestamp > commands.timestamp ? null : commands);
        //     }
        // }

        private void SpendOneMove()
        {
            cloudConnector.OnSpendOneMove();
        }

        #endregion

        #region GET & SET
        
        private string GetSavingPath(SavingPath tailPath)
        {
            var envPath = _gamePath + "/" + tailPath;
            return envPath;
        }

        public EnvironmentData GetEnvironmentData()
        {
            return _envLoader.GetData();
        }

        #endregion
    }
}