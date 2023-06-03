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
        Currencies,
        Commands
    }

    [RequireComponent(typeof(EnvironmentLoader))]
    [RequireComponent(typeof(CurrencyLoader))]
    public class SavingSystemManager : Singleton<SavingSystemManager>
    {
        // Save Player environment data whenever a creature move
        [NonSerialized] public UnityEvent OnSavePlayerEnvData = new(); // invoke at CreatureEntity
        [NonSerialized] public UnityEvent OnUseOneMove = new(); // invoke at EnvironmentManager

        [NonSerialized]
        public UnityEvent OnRestoreCommands = new(); // send to EnvironmentManager, invoke at CommandCache

        [SerializeField] private JICloudConnector cloudConnector;
        private EnvironmentLoader _envLoader;
        private CurrencyLoader _currencyLoader;

        private string _gamePath;
        private bool encrypt = true;
        private bool _isLastSessionDisconnect;

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
            Debug.Log($"Time from start game {Mathf.RoundToInt(Time.realtimeSinceStartup * 1000000)}");
            _envLoader.GetData().timestamp +=
                Mathf.RoundToInt(Mathf.Clamp(Time.realtimeSinceStartup - 10, 0, Time.realtimeSinceStartup - 10) *
                                 1000000); // minus 10 second to ensure local always faster than cloud in timestamp 
            SavePlayerEnv();
            SaveCommandBatch(cloudConnector.GetCommands());
        }

        private async void StartUpLoadData()
        {
            await LoadEnvironment();
            _envLoader.Init();

            await LoadCurrencies();
            _currencyLoader.Init();

            await LoadCommands();

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
                // Check if the last session is not Internet connection
                if (_envLoader.GetData().lastTimestamp < data.timestamp)
                    _isLastSessionDisconnect = true;

                if (_isLastSessionDisconnect)
                {
                    data.lastTimestamp = _envLoader.GetData().lastTimestamp;
                    _envLoader.SetData(data);
                }
            }
        }

        #endregion

        #region CURRENCIES

        private async Task LoadCurrencies()
        {
            _currencyLoader.SetData(await cloudConnector.OnLoadCurrency());
        }

        private void SpendOneMove()
        {
            cloudConnector.OnSpendOneMove();
        }

        #endregion

        #region COMMAND

        private void SaveCommandBatch(CommandsCache commandsCache)
        {
            var commandPath = GetSavingPath(SavingPath.Commands);
            SaveManager.Instance.Save(commandsCache, commandPath, CommandBatchWasSaved, encrypt);
        }

        private void CommandBatchWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
            {
                Debug.LogError($"Error saving data:\n{result}\n{message}");
            }
        }

        private async Task LoadCommands()
        {
            var commandPath = GetSavingPath(SavingPath.Commands);
            SaveManager.Instance.Load<CommandsCache>(commandPath, CommandWasLoaded, encrypt);
        }

        private void CommandWasLoaded(CommandsCache commands, SaveResult result, string message)
        {
            Debug.Log(
                $"Data Was Loaded:{result}\nNumber of commands: {commands.commandList.Count}\nMessage: {message}");

            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No Data File Found -> Creating new data...");

            if (result == SaveResult.Success)
            {
                Debug.Log("Restore command after a disconnected session");
                // Just load command when the game disconnect in the latest session
                if (_isLastSessionDisconnect)
                    commands.ExecuteJICommands();
            }
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