using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using Unity.Services.Leaderboards.Models;
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
        [NonSerialized] public UnityEvent<CommandName> OnContributeCommand = new(); // invoke at EnvironmentManager
        [NonSerialized] public UnityEvent<Entity> OnContributeFromEntity = new(); // invoke at ResourceInGame

        [NonSerialized] public UnityEvent<IRemoveEntity>
            OnRemoveEntityData =
                new(); // send to EnvironmentLoader, invoke at ResourceInGame, BuildingIngame, CreatureInGame;

        [NonSerialized]
        public UnityEvent OnUpdateLocalMove = new(); // send to EnvironmentManager, invoke at CommandCache

        [SerializeField] protected JICloudConnector m_CloudConnector;
        [SerializeField] private string[] m_BasicInventory;

        private EnvironmentLoader m_EnvLoader;
        private CurrencyLoader m_CurrencyLoader;
        private InventoryLoader m_InventoryLoader;

        private GameStateData _gameStateData = new();
        private string _gamePath;
        private bool encrypt = true;
        private bool _isLastSessionDisconnect;

        protected override void Awake()
        {
            base.Awake();
            _gamePath = Application.persistentDataPath;
            m_EnvLoader = GetComponent<EnvironmentLoader>();
            m_CurrencyLoader = GetComponent<CurrencyLoader>();
            m_InventoryLoader = GetComponent<InventoryLoader>();

            OnSavePlayerEnvData.AddListener(SavePlayerEnv);
            OnContributeCommand.AddListener(StackUpCommand);
            OnContributeFromEntity.AddListener(StackUpFromEntity);
            GameFlowManager.Instance.OnLoadData.AddListener(StartUpLoadData);
        }

        private async void OnDisable()
        {
            if (!CheckLoadingPhaseFinished()) return;
            SavePlayerEnvAtEndGame();
            SaveCommandBatch(m_CloudConnector.GetCommands());
            await CheckInternetConnection();
        }

        private async Task CheckInternetConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Error. Check internet connection!");
                SaveDisconnectedState(true);
            }
        }

        private async void StartUpLoadData()
        {
            await LoadGameState();

            // mark as starting point of loading phase.
            // If this process is not complete, the environment will not be save at OnDisable()
            SetInLoadingState(true);

            await LoadEnvironment();
            m_EnvLoader.Init();

            await LoadCurrencies();
            m_CurrencyLoader.Init();

            await LoadCommands();

            SaveDisconnectedState(false); // set it as connected state when loaded all disconnected session's data
            SetInLoadingState(false); // Finish loading phase

            GameFlowManager.Instance.OnStartGame.Invoke(m_CurrencyLoader.GetMoveAmount());
        }

        public async void OnResetData()
        {
            m_EnvLoader.ResetData();
            await ResetData();
            m_EnvLoader.Init();
        }

        #region GAME STATE

        private void SetInLoadingState(bool isLoading)
        {
            _gameStateData.IsInLoadingPhase = isLoading;
            var gameStatePath = GetSavingPath(SavingPath.GameState);
            SaveManager.Instance.Save(_gameStateData, gameStatePath, GameStateWasSaved, encrypt);
        }

        private async Task LoadGameState()
        {
            var gameStatePath = GetSavingPath(SavingPath.GameState);
            SaveManager.Instance.Load<GameStateData>(gameStatePath, GameStateWasLoaded, encrypt);
        }

        private void GameStateWasLoaded(GameStateData gameState, SaveResult result, string message)
        {
            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No State data File Found -> Creating new data...");

            if (result == SaveResult.Success)
                _gameStateData = gameState;
        }

        private void SaveDisconnectedState(bool isDisconnected)
        {
            _gameStateData.IsDisconnectedLastSession = isDisconnected;
            var gameStatePath = GetSavingPath(SavingPath.GameState);
            SaveManager.Instance.Save(_gameStateData, gameStatePath, GameStateWasSaved, encrypt);
        }

        private void GameStateWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
                Debug.LogError($"Error saving data:\n{result}\n{message}");
        }

        public bool CheckLoadingPhaseFinished()
        {
            return !_gameStateData.IsInLoadingPhase;
        }

        #endregion

        #region ENVIRONMENT

        private void SavePlayerEnv()
        {
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Save(m_EnvLoader.GetData(), envPath, DataWasSaved, encrypt);
        }

        private void SavePlayerEnvAtEndGame()
        {
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Save(m_EnvLoader.GetDataForSave(), envPath, DataWasSaved, encrypt);
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
            await m_CloudConnector.Init();
            var cloudEnvData = await m_CloudConnector.OnLoadEnvData();
            if (cloudEnvData == null || cloudEnvData.mapSize == 0)
            {
                Debug.Log("This is the first time log in the game.");
            }
            else
                m_EnvLoader.SetData(await m_CloudConnector.OnLoadEnvData());

            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Load<EnvironmentData>(envPath, EnvWasLoaded, encrypt);
        }

        private async void EnvWasLoaded(EnvironmentData data, SaveResult result, string message)
        {
            Debug.Log($"Env Was Loaded:\n{result}\n{message}");

            if (result == SaveResult.EmptyData || result == SaveResult.Error)
            {
                Debug.LogError("No Env data File Found -> Creating new data...");
                await ResetData();
            }

            if (result == SaveResult.Success)
            {
                if (!_gameStateData.IsDisconnectedLastSession) return;
                Debug.Log("Restore command after a disconnected session");
                data.lastTimestamp = m_EnvLoader.GetData().lastTimestamp;
                m_EnvLoader.SetData(data);
            }
        }

        private async Task ResetData()
        {
            var cloudEnvData = await m_CloudConnector.OnResetEnvData();

            if (cloudEnvData != null)
            {
                m_EnvLoader.SetData(cloudEnvData);
                SavePlayerEnv();
                foreach (var inventoryId in m_BasicInventory)
                    m_CloudConnector.OnGrantInventory(inventoryId);
            }
        }

        public void OnSpawnResource(InventoryType inventoryType, Vector3 position)
        {
            var inventoryItem = m_InventoryLoader.GetInventoriesByType(inventoryType);
            if (inventoryItem == null)
                return;

            var newResource = new ResourceData()
            {
                EntityName = inventoryItem.inventoryName, SkinAddress = inventoryItem.skinAddress, Position = position
            };
            m_EnvLoader.SpawnResource(newResource);
        }

        public async void OnPlaceABuilding(JIInventoryItem inventoryItem, Vector3 position)
        {
            var purchaseHandler = await m_CloudConnector.OnMakeAPurchase(inventoryItem.virtualPurchaseId);
            if (purchaseHandler == null)
            {
                Debug.Log("Show \"Lack of currency\" panel");
                return;
            }

            // Pay for constructing the building...
            var constructingCost = m_CloudConnector.GetVirtualPurchaseCost(inventoryItem.virtualPurchaseId);
            foreach (var cost in constructingCost)
            {
                m_CurrencyLoader.IncrementCurrency(cost.id, cost.amount * -1);
            }

            // ...and get the building in place
            var newBuilding = new BuildingData
            {
                EntityName = inventoryItem.inventoryName, SkinAddress = inventoryItem.skinAddress, Position = position
            };
            m_EnvLoader.PlaceABuilding(newBuilding);
        }

        public async void OnTrainACreature(JIInventoryItem inventoryItem, Vector3 position, bool isEcoMode)
        {
            if (isEcoMode)
            {
                var purchaseHandler = await m_CloudConnector.OnMakeAPurchase(inventoryItem.virtualPurchaseId);
                if (purchaseHandler == null)
                {
                    Debug.Log("Show \"Lack of currency\" panel");
                    return;
                }
            }

            var newCreature = new CreatureData()
            {
                EntityName = inventoryItem.inventoryName, SkinAddress = inventoryItem.skinAddress, Position = position
            };
            m_EnvLoader.TrainACreature(newCreature);
        }

        #endregion

        #region ECONOMY

        private async Task LoadCurrencies()
        {
            m_CurrencyLoader.SetData(await m_CloudConnector.OnLoadCurrency());
            m_InventoryLoader.SetData(await m_CloudConnector.OnLoadInventory());
            m_CloudConnector.OnLoadVirtualPurchase();
        }

        public void IncrementLocalCurrency(string rewardID, int rewardAmount)
        {
            m_CurrencyLoader.IncrementCurrency(rewardID, rewardAmount);
        }

        public IEnumerable<PlayerBalance> GetCurrencies()
        {
            return m_CurrencyLoader.GetCurrencies();
        }

        public bool CheckEnoughCurrency(string currencyId, int currencyAmount)
        {
            return m_CurrencyLoader.CheckEnoughCurrency(currencyId, currencyAmount);
        }

        public void GrantCurrency(string currencyId, int amount)
        {
            m_CloudConnector.OnGrantCurrency(currencyId, amount);
            m_CurrencyLoader.IncrementCurrency(currencyId, amount);
        }

        #endregion

        #region INVENTORY

        public void OnAskForShowingBuildingMenu()
        {
            m_InventoryLoader.SendInventoriesToBuildingMenu();
        }

        public void OnAskForShowingCreatureMenu()
        {
            m_InventoryLoader.SendInventoriesToCreatureMenu();
        }

        public JIInventoryItem ConvertToInventoryItem(EntityData data)
        {
            return m_CloudConnector.ConvertToInventoryItem(data);
        }

        #endregion

        #region COMMAND

        private void StackUpCommand(CommandName commandName)
        {
            m_CloudConnector.OnCommandStackUp(commandName);
        }

        private void StackUpFromEntity(Entity fromEntity)
        {
            m_CloudConnector.OnCommandStackUp(fromEntity);
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
            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No Command data File Found -> Creating new data...");

            if (result == SaveResult.Success)
            {
                Debug.Log(
                    $"Commands Was Loaded:{result}\nNumber of commands: {commands.commandList.Count}\nMessage: {message}");

                // Just load command when the game disconnect in the latest session
                if (_gameStateData.IsDisconnectedLastSession)
                {
                    for (int i = 0; i < commands.commandList.Count; i++)
                        OnUpdateLocalMove.Invoke();
                    m_CloudConnector.SubmitCommands(commands.commandList);
                }
            }
        }

        #endregion

        #region LEADERBOARD

        public async Task<EnvironmentData> GetEnemyEnv()
        {
            return await m_CloudConnector.GetEnemyEnvironment();
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
            return m_EnvLoader.GetData();
        }

        public EnvironmentData GetEnvDataForSave()
        {
            return m_EnvLoader.GetDataForSave();
        }

        public void StoreCurrencyAtBuildings(string currency, int amount, Vector3 fromPos)
        {
            m_EnvLoader.StoreRewardToBuildings(currency, amount, fromPos);
        }

        #endregion
    }
}