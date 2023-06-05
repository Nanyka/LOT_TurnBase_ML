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
        Commands,
        GameState
    }

    [RequireComponent(typeof(EnvironmentLoader))]
    [RequireComponent(typeof(CurrencyLoader))]
    public class SavingSystemManager : Singleton<SavingSystemManager>
    {
        // Save Player environment data whenever a creature move
        [NonSerialized] public UnityEvent OnSavePlayerEnvData = new(); // invoke at CreatureEntity
        [NonSerialized] public UnityEvent OnUseOneMove = new(); // invoke at EnvironmentManager
        [NonSerialized] public UnityEvent OnRestoreCommands = new(); // send to EnvironmentManager, invoke at CommandCache

        [SerializeField] private JICloudConnector cloudConnector;
        private EnvironmentLoader _envLoader;
        private CurrencyLoader _currencyLoader;
        
        private GameStateData _gameStateData = new();
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

        private async void OnDisable()
        {
            SavePlayerEnv();
            SaveCommandBatch(cloudConnector.GetCommands());
            await CheckInternetConnection();
        }

        private async Task CheckInternetConnection()
        {
            if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Error. Check internet connection!");
                await SaveDisconnectedState();
            }
        }

        private async void StartUpLoadData()
        {
            await LoadGameState();
            
            await LoadEnvironment();
            _envLoader.Init();

            await LoadCurrencies();
            _currencyLoader.Init();

            await LoadCommands();

            await FinishStartUp();
        }

        private async Task FinishStartUp()
        {
            StartUpProcessor.Instance.OnStartGame.Invoke(_currencyLoader.GetMoveAmount());
        }

        private void ResetData()
        {
            _envLoader.ResetData();
            StartUpLoadData();
        }

        #region GAME STATE

        private async Task SaveDisconnectedState()
        {
            _gameStateData.IsDisconnected = true;
            var gameStatePath = GetSavingPath(SavingPath.GameState);
            SaveManager.Instance.Save(_gameStateData, gameStatePath, GameStateWasSaved, encrypt);
        }

        private void GameStateWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
                Debug.LogError($"Error saving data:\n{result}\n{message}");
        }

        private async Task LoadGameState()
        {
            var gameStatePath = GetSavingPath(SavingPath.GameState);
            SaveManager.Instance.Load<GameStateData>(gameStatePath, GameStateWasLoaded, encrypt);
        }

        private void GameStateWasLoaded(GameStateData gameState, SaveResult result, string message)
        {
            Debug.Log($"GameState Was Loaded:\n{result}, last session disconnected is {gameState.IsDisconnected}\n{message}");

            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No Data File Found -> Creating new data...");

            if (result == SaveResult.Success)
                _gameStateData = gameState;
        }

        #endregion

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
            Debug.Log($"Env Was Loaded:\n{result}\n{message}");

            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No Data File Found -> Creating new data...");

            if (result == SaveResult.Success)
            {
                if (!_gameStateData.IsDisconnected) return;
                Debug.Log("Restore command after a disconnected session");
                data.lastTimestamp = _envLoader.GetData().lastTimestamp;
                _envLoader.SetData(data);
            }
        }

        #endregion

        #region CURRENCIES

        private async Task LoadCurrencies()
        {
            _currencyLoader.SetData(await cloudConnector.OnLoadCurrency());
        }

        #endregion

        #region COMMAND

        private void SpendOneMove()
        {
            cloudConnector.OnSpendOneMove();
        }
        
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
            Debug.Log($"Commands Was Loaded:{result}\nNumber of commands: {commands.commandList.Count}\nMessage: {message}");

            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No Data File Found -> Creating new data...");

            if (result == SaveResult.Success)
            {
                // Just load command when the game disconnect in the latest session
                if (_gameStateData.IsDisconnected)
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