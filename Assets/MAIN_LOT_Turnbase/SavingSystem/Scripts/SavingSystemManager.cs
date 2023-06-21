using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.Events;

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
        // invoke at CreatureEntity
        [NonSerialized] public UnityEvent OnSavePlayerEnvData = new(); 
        // invoke at EnvironmentManager
        [NonSerialized] public UnityEvent<CommandName> OnContributeCommand = new(); 
        // send to EnvironmentLoader, invoke at ResourceInGame, BuildingIngame, CreatureInGame;
        [NonSerialized] public UnityEvent<IRemoveEntity> OnRemoveEntityData = new(); 

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
            // Authenticate on UGS and get envData
            await m_CloudConnector.Init();

            // Load gameState from local to check if the previous session is disconnected
            await LoadGameState();
            
            // mark as starting point of loading phase.
            // If this process is not complete, the environment will not be save at OnDisable()
            SetInLoadingState(true);

            // Load environment and calculate any time-based resource increment
            await LoadEnvironment();
            m_EnvLoader.Init();

            // Load currency after commit MOVE created during skip period
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
            var cloudEnvData = await m_CloudConnector.OnLoadEnvData();
            if (cloudEnvData == null || cloudEnvData.mapSize == 0)
            {
                Debug.Log("No cloudEnvDat was found.");
                _gameStateData.IsDisconnectedLastSession = true;
            }
            else
                m_EnvLoader.SetData(cloudEnvData);

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
                await m_CloudConnector.OnResetBasicInventory(m_BasicInventory.ToList());
                m_CloudConnector.PlayerRecordScore(0);
            }
        }

        public void OnSpawnResource(InventoryType inventoryType, Vector3 position)
        {
            var inventoryItem = m_InventoryLoader.GetInventoriesByType(inventoryType);
            if (inventoryItem == null)
                return;

            var newResource = new ResourceData()
            {
                EntityName = inventoryItem.inventoryName,
                Position = position,
                CurrentLevel = 0
            };
            m_EnvLoader.SpawnResource(newResource);
        }

        public async void OnPlaceABuilding(JIInventoryItem inventoryItem, Vector3 position)
        {
            if (await ConductVirtualPurchase(inventoryItem.virtualPurchaseId) == false) return;

            // ...and get the building in place
            var newBuilding = new BuildingData
            {
                EntityName = inventoryItem.inventoryName,
                Position = position,
                CurrentLevel = 0
            };
            m_EnvLoader.PlaceABuilding(newBuilding);
        }

        public async void OnTrainACreature(JIInventoryItem inventoryItem, Vector3 position, bool isWaitForPurchase)
        {
            if (isWaitForPurchase)
            {
                if (await ConductVirtualPurchase(inventoryItem.virtualPurchaseId) == false) return;
            }

            var newCreature = new CreatureData()
            {
                EntityName = inventoryItem.inventoryName, 
                Position = position,
                CurrentLevel = 0
            };
            m_EnvLoader.TrainACreature(newCreature);
        }

        public void SpawnConsumableEntity(string itemId, Vector3 position, FactionType factionType)
        {
            var item = GetInventoryItemByName(itemId);
            
            if (factionType == FactionType.Neutral)
            {
                var newEntity = new CreatureData()
                {
                    EntityName = item.inventoryName,
                    Position = position,
                    CurrentLevel = 0
                };
                m_EnvLoader.SpawnAnEnemy(newEntity);
            }
        }

        private async Task<bool> ConductVirtualPurchase(string virtualPurchaseId)
        {
            var purchaseHandler = await m_CloudConnector.OnMakeAPurchase(virtualPurchaseId);
            if (purchaseHandler == null)
            {
                Debug.Log($"Show \"Lack of {virtualPurchaseId}\" panel");
                return false;
            }

            // Pay for constructing the building...
            var constructingCost = m_CloudConnector.GetVirtualPurchaseCost(virtualPurchaseId);
            Debug.Log(
                $"Conduct virtualPurchase {virtualPurchaseId} that cost {constructingCost[0].id} an amount {constructingCost[0].amount}");
            foreach (var cost in constructingCost)
            {
                m_CurrencyLoader.IncrementCurrency(cost.id, cost.amount * -1);
            }

            return true;
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
            int storageSpace = m_EnvLoader.GetStorageSpace(rewardID);
            if (storageSpace < rewardAmount)
            {
                Debug.Log("[TODO] Show something to announce \"Lack of storage\"");
                m_CurrencyLoader.IncrementCurrency(rewardID, storageSpace);
            }
            else
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

        public JIInventoryItem GetInventoryItemByName(string entityName)
        {
            return m_CloudConnector.GetInventoryByNameOrId(entityName);
        }

        #endregion

        #region VIRTUAL PURCHASE

        public VirtualPurchaseDefinition GetPurchaseDefinition(string id)
        {
            return m_CloudConnector.GetPurchaseDefinition(id);
        }

        #endregion

        #region COMMAND

        private async void StackUpCommand(CommandName commandName)
        {
            m_CloudConnector.OnCommandStackUp(commandName);
            await m_CloudConnector.OnSaveEnvData();
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
                if (commands.commandList.Count > 0)
                {
                    m_CloudConnector.SubmitCommands(commands);
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

        public void StoreCurrencyAtBuildings(string commandId, Vector3 fromPos)
        {
            var rewards = m_CloudConnector.GetRewardByCommandId(commandId);
            foreach (var reward in rewards)
            {
                switch (reward.service)
                {
                    case "currency":
                        m_EnvLoader.StoreRewardToBuildings(reward.id, reward.amount);
                        break;
                }
            }
        }

        #endregion
    }
}