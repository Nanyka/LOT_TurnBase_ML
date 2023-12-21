using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    // public enum SavingPath
    // {
    //     PlayerEnvData,
    //     Currencies,
    //     Commands,
    //     GameState,
    //     QuestData
    // }

    [RequireComponent(typeof(ICurrencyLoader))]
    public class SavingSystemManager : Singleton<SavingSystemManager>
    {
        // invoke at TileManager
        [NonSerialized] public UnityEvent OnSavePlayerEnvData = new();

        // invoke at EnvironmentManager
        [NonSerialized] public UnityEvent<CommandName> OnContributeCommand = new();

        // send to EnvironmentLoader, invoke at ResourceInGame, BuildingInGame, CreatureInGame;
        [NonSerialized] public UnityEvent<IRemoveEntity> OnRemoveEntityData = new();

        // invoke at InventoryLoader
        [NonSerialized] public UnityEvent<List<JIInventoryItem>> OnSetUpBuildingMenus = new();

        // invoke at JICloudCodeManager
        [NonSerialized] public UnityEvent OnRefreshBalances = new();

        // invoke at CreatureDetailMenu
        [NonSerialized] public UnityEvent<string> OnCreatureUpgrade = new();

        // invoke at ???
        [NonSerialized] public UnityEvent OnSaveQuestData = new();

        // invoke at CreatureInGame, BuildingInGame
        [NonSerialized] public UnityEvent<RecordAction> OnRecordAction = new();
        [SerializeField] private string[] m_BasicInventory;

        protected JICloudConnector m_CloudConnector;
        protected IEnvironmentLoader m_EnvLoader;
        private IResourceStock m_StorageHandler;
        private IResearchTopicSupervisor m_ResearchSup;
        private IStoragesControl m_StorageController;
        private ICurrencyLoader m_CurrencyLoader;
        private IInventoryDeliver m_InventoryLoader;

        protected RuntimeMetadata m_RuntimeMetadata = new();
        private GameProcessData m_GameProcess = new();
        private QuestData m_QuestData;
        private string _gamePath;
        private bool encrypt = true;
        private bool _isLastSessionDisconnect;

        protected override void Awake()
        {
            base.Awake();
            _gamePath = Application.persistentDataPath;
            
            m_EnvLoader = GetComponent<IEnvironmentLoader>();
            m_StorageHandler = GetComponent<IResourceStock>();
            m_ResearchSup = GetComponent<IResearchTopicSupervisor>();
            m_StorageController = GetComponent<IStoragesControl>();
            m_CurrencyLoader = GetComponent<ICurrencyLoader>();
            m_InventoryLoader = GetComponent<IInventoryDeliver>();

            OnSavePlayerEnvData.AddListener(SavePlayerEnv);
            OnContributeCommand.AddListener(StackUpCommand);
            OnRefreshBalances.AddListener(RefreshBalances);
            OnSaveQuestData.AddListener(SaveQuestData);
        }

        protected virtual void OnDisable()
        {
            SaveGameState();
        }

        private async void SaveGameState()
        {
            if (!CheckLoadingPhaseFinished() || GameFlowManager.Instance.GameMode == GameMode.AOE) return;
            if (m_EnvLoader.GetDataForSave().CheckStorable() == false)
                return;
            SavePlayerEnvAtEndGame();
            SaveCommandBatch(m_CloudConnector.GetCommands());
            await CheckInternetConnection();
        }

        private async Task CheckInternetConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Error. Check internet connection!");
                SaveMetadata();
            }
        }

        public virtual async void StartLoadData()
        {
            m_CloudConnector = JICloudConnector.Instance;

            // Authenticate on UGS and get envData
            await m_CloudConnector.Init();

            // Load gameState from local to check if the previous session is disconnected
            LoadPreviousMetadata();

            // mark as starting point of loading phase.
            // If this process is not complete, the environment will not be save at OnDisable()
            SetInLoading(true);

            // for Testing: Do not LoadCommands(), just use remoteConfig as currency JSON
            LoadLocalCurrencies();

            // Load currency after commit MOVE created during skip period
            LoadEconomy();
            // m_CurrencyLoader.Init();

            // Load environment and calculate any time-based resource increment
            await LoadEnvironment();

            // Load game process to refresh current tutorial
            LoadGameProcess();

            Debug.Log("Completed loading process");
            SaveMetadata(); // set it as connected state when loaded all disconnected session's data
            SetInLoading(false); // Finish loading phase

            GameFlowManager.Instance.OnDataLoaded.Invoke(m_CurrencyLoader.GetCurrency("MOVE"));
        }

        public async void OnResetData()
        {
            m_EnvLoader.ResetData();
            await ResetData();
            m_EnvLoader.Init();
        }

        #region GAME STATE

        private void SetInLoading(bool isLoading)
        {
            m_RuntimeMetadata.IsInLoadingPhase = isLoading;
            var gameStatePath = GetSavingPath(SavingPath.GameState);
            SaveManager.Instance.Save(m_RuntimeMetadata, gameStatePath, MetadataWasSaved, encrypt);
        }

        protected void LoadPreviousMetadata()
        {
            var gameStatePath = GetSavingPath(SavingPath.GameState);
            SaveManager.Instance.Load<RuntimeMetadata>(gameStatePath, MetadataWasLoaded, encrypt);
        }

        private void MetadataWasLoaded(RuntimeMetadata gameState, SaveResult result, string message)
        {
            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No State data File Found -> Creating new data...");

            if (result == SaveResult.Success)
                m_RuntimeMetadata = gameState;
        }

        private void SaveMetadata()
        {
            var gameStatePath = GetSavingPath(SavingPath.GameState);
            SaveManager.Instance.Save(m_RuntimeMetadata, gameStatePath, MetadataWasSaved, encrypt);
        }

        public void SaveMetadata(BattleRecord battleRecord)
        {
            Debug.Log("Save data for recordMode");
            m_RuntimeMetadata.BattleRecord = battleRecord;
            var gameStatePath = GetSavingPath(SavingPath.GameState);
            SaveManager.Instance.Save(m_RuntimeMetadata, gameStatePath, MetadataWasSaved, encrypt);
        }

        private void MetadataWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
                Debug.LogError($"Error saving data:\n{result}\n{message}");
        }

        private bool CheckLoadingPhaseFinished()
        {
            return !m_RuntimeMetadata.IsInLoadingPhase;
        }

        #endregion

        #region QUEST

        private void ResetQuestData()
        {
            m_QuestData = new QuestData();
            m_QuestData.QuestChains = new List<QuestChain>();
            SaveQuestData();
        }

        private void SaveQuestData()
        {
            var gameStatePath = GetSavingPath(SavingPath.QuestData);
            SaveManager.Instance.Save(m_QuestData, gameStatePath, QuestDataWasSaved, encrypt);
        }

        public void SaveQuestData(int bossIndex, string questAddress, int starAmount)
        {
            if (m_QuestData.QuestChains.Count <= bossIndex)
                m_QuestData.QuestChains.Add(new QuestChain());

            var questChain = m_QuestData.QuestChains[bossIndex];

            if (questChain.QuestUnits == null)
                questChain.QuestUnits = new List<QuestUnit>();
            var quest = questChain.QuestUnits.Find(t => t.QuestAddress.Equals(questAddress));
            if (quest == null)
            {
                quest = new QuestUnit(questAddress);
                questChain.QuestUnits.Add(quest);
            }

            quest.StarAmount = starAmount;

            var gameStatePath = GetSavingPath(SavingPath.QuestData);
            SaveManager.Instance.Save(m_QuestData, gameStatePath, QuestDataWasSaved, encrypt);
        }

        private void QuestDataWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
                Debug.LogError($"Error saving quest data:\n{result}\n{message}");
        }

        private void LoadQuestData()
        {
            var gameStatePath = GetSavingPath(SavingPath.QuestData);
            SaveManager.Instance.Load<QuestData>(gameStatePath, QuestDataWasLoaded, encrypt);
        }

        private void QuestDataWasLoaded(QuestData questData, SaveResult result, string message)
        {
            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No State data File Found -> Creating new data...");

            if (result == SaveResult.Success)
                m_QuestData = questData;
        }

        public QuestData GetQuestData()
        {
            return m_QuestData;
        }

        public void SetQuestData(QuestData questData)
        {
            m_QuestData = questData;
        }

        #endregion

        #region ENVIRONMENT

        private void SavePlayerEnv()
        {
            if (GameFlowManager.Instance.GameMode == GameMode.REPLAY)
                return;

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
            // Debug.Log("Save player environment locally");
            if (result == SaveResult.Error)
            {
                Debug.LogError($"Error saving data:\n{result}\n{message}");
            }
        }

        private Task LoadEnvironment()
        {
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Load<EnvironmentData>(envPath, EnvWasLoaded, encrypt);
            return Task.CompletedTask;
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
                // Fetch mainHallTier after receive envData
                // var mainHall = data.BuildingData.Find(t => t.BuildingType == BuildingType.MAINHALL);
                // await m_CloudConnector.FetchEnvRelevantData(mainHall.CurrentLevel);
                m_EnvLoader.SetData(data);
                m_EnvLoader.Init();
            }
        }

        private async Task ResetData()
        {
            var cloudEnvData = await m_CloudConnector.OnResetEnvData();

            if (cloudEnvData != null)
            {
                m_EnvLoader.SetData(cloudEnvData);
                SavePlayerEnv();
                m_CurrencyLoader.ResetCurrencies(await m_CloudConnector.OnLoadCurrency());
                ResetBasicInventory();
            }

            ResetQuestData();
        }

        public void OnSpawnResource(string resourceId, Vector3 position)
        {
            var inventoryItems = m_InventoryLoader.GetInventoriesByType(InventoryType.Resource);

            foreach (var item in inventoryItems)
            {
                if (item.inventoryName.Equals(resourceId))
                {
                    var newResource = new ResourceData()
                    {
                        EntityName = item.inventoryName,
                        Position = position,
                        CurrentLevel = 0
                    };
                    m_EnvLoader.SpawnResource(newResource);
                }
            }
        }

        public void OnSpawnCollectable(string collectableName, Vector3 position, int level)
        {
            var collectableData = new CollectableData()
            {
                EntityName = collectableName,
                Position = position,
                CurrentLevel = level
            };
            m_EnvLoader.SpawnCollectable(collectableData);
        }

        // Player pay some cost for constructing the building
        public async void OnPlaceABuilding(JIInventoryItem inventoryItem, Vector3 position, bool isCheckMax)
        {
            var buildingLimit = GetCurrentTier().TierItems.Find(t => t.itemName.Equals(inventoryItem.inventoryName)).amount;
            var buildingCount = GetEnvironmentData().BuildingData.Count(t => t.EntityName.Equals(inventoryItem.inventoryName));
            if (buildingCount >= buildingLimit)
            {
                MainUI.Instance.OnConversationUI.Invoke("Reach limited construction", true);
                return;
            }

            if (await OnConductVirtualPurchase(inventoryItem.virtualPurchaseId) == false) return;

            // ...and get the building in place
            var newBuilding = new BuildingData
            {
                EntityName = inventoryItem.inventoryName,
                Position = position,
                CurrentLevel = 0
            };
            m_EnvLoader.PlaceABuilding(newBuilding);
        }

        // Player pay something for construction locally
        public void OnPlaceABuilding(JIInventoryItem inventoryItem, Vector3 position, CurrencyUnit cost)
        {
            var buildingLimitation =
                GetCurrentTier().TierItems.Find(t => t.itemName.Equals(inventoryItem.inventoryName));

            if (buildingLimitation == null)
                return;

            var checkedAmount = buildingLimitation.amount;
            if (GetEnvironmentData().CheckBuildingLimitAmount(inventoryItem.inventoryName, checkedAmount))
            {
                MainUI.Instance.OnConversationUI.Invoke($"{inventoryItem.inventoryName} reach limited construction",
                    true);
                return;
            }

            if (m_CurrencyLoader.CheckEnoughCurrency(cost.currencyId, cost.amount) == false)
                return;

            // ...and get the building in place
            var newBuilding = new BuildingData
            {
                EntityName = inventoryItem.inventoryName,
                Position = position,
                CurrentLevel = 0
            };
            m_EnvLoader.PlaceABuilding(newBuilding);
        }

        // Spawn from a reward, player pay nothing for it
        public async Task OnPlaceABuilding(string buildingName, Vector3 position, bool isFromCollectable)
        {
            var inventoryItem = GetInventoryItemByName(buildingName);
            if (inventoryItem == null) return;

            if (isFromCollectable == false)
                if (await OnConductVirtualPurchase(inventoryItem.virtualPurchaseId) == false)
                    return;

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
            if (m_EnvLoader.GetData().CheckFullCapacity())
            {
                MainUI.Instance.OnConversationUI.Invoke("No any space for new member", true);
                return;
            }

            if (isWaitForPurchase)
            {
                if (await OnConductVirtualPurchase(inventoryItem.virtualPurchaseId) == false) return;
            }

            var newCreature = new CreatureData()
            {
                EntityName = inventoryItem.inventoryName,
                Position = position,
                CurrentLevel = 0
            };

            if (inventoryItem.EntityData != null)
            {
                newCreature = (CreatureData)inventoryItem.EntityData;
                newCreature.Position = position;
            }

            m_EnvLoader.TrainACreature(newCreature);
        }

        public async Task<GameObject> OnTrainACreature(string creatureName, Vector3 position, bool isWaitForPurchase)
        {
            var inventoryItem = GetInventoryItemByName(creatureName);

            if (inventoryItem == null)
            {
                Debug.Log($"No any inventory name {creatureName}");
                return null;
            }

            // if (m_EnvLoader.GetData().CheckFullCapacity())
            // {
            //     MainUI.Instance.OnConversationUI.Invoke("No any space for new member", true);
            //     return null;
            // }

            if (isWaitForPurchase)
            {
                if (await OnConductVirtualPurchase(inventoryItem.virtualPurchaseId) == false) return null;
            }

            var newCreature = new CreatureData()
            {
                EntityName = inventoryItem.inventoryName,
                Position = position,
                CurrentLevel = 0
            };

            if (inventoryItem.EntityData != null)
            {
                newCreature = (CreatureData)inventoryItem.EntityData;
                newCreature.Position = position;
            }

            return m_EnvLoader.TrainACreature(newCreature);
        }

        public GameObject OnTrainACreature(CreatureData creatureData)
        {
            if (creatureData.CreatureType == CreatureType.WORKER)
            {
                if (m_EnvLoader.GetData().CheckFullCapacity())
                {
                    MainUI.Instance.OnConversationUI.Invoke("No any space for new member", true);
                    return null;
                }
            }

            return m_EnvLoader.TrainACreature(creatureData);
        }

        public GameObject OnSpawnMovableEntity(string itemId, Vector3 position)
        {
            var item = GetInventoryItemByName(itemId);

            var newEntity = new CreatureData()
            {
                EntityName = item.inventoryName,
                Position = position,
                CurrentLevel = 0
            };
            return m_EnvLoader.SpawnAnEnemy(newEntity);
        }

        public async void OnSaveEnvById(string playerId)
        {
            await m_CloudConnector.OnSaveEnvById(m_EnvLoader.GetData(), playerId);
        }

        public async void OnSyncEnvData()
        {
            await m_CloudConnector.OnSaveEnvData();
        }

        #endregion

        #region REMOTE CONFIG

        public int GetMaxMove()
        {
            return m_CloudConnector.GetNumericByConfig(CommandName.JI_MAX_MOVE.ToString());
        }

        public int GetTownhouseSpace()
        {
            return m_CloudConnector.GetNumericByConfig(NumericConfigName.JI_TOWNHOUSE_SPACE.ToString());
        }

        public async Task<JIRemoteConfigManager.BattleLoot> GetBattleLootByStar(int stars)
        {
            return await m_CloudConnector.GetBattleWinLoot(stars);
        }

        public List<JIRemoteConfigManager.Reward> GetRewardByCommand(string commandId)
        {
            return m_CloudConnector.GetRewardByCommandId(commandId);
        }

        public MainHallTier GetCurrentTier()
        {
            return m_CloudConnector.GetCurrentTier();
        }

        public MainHallTier GetUpcomingTier()
        {
            return m_CloudConnector.GetNextTier();
        }

        #endregion

        #region ECONOMY

        public void SaveLocalBalances(LocalBalancesData balances)
        {
            var currenciesPath = GetSavingPath(SavingPath.Currencies);
            SaveManager.Instance.Save(balances, currenciesPath, LocalBalancesWasSaved, encrypt);
        }

        private void LocalBalancesWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
                Debug.LogError($"Error saving currencies:\n{result}\n{message}");
        }

        private void LoadLocalCurrencies()
        {
            var currenciesPath = GetSavingPath(SavingPath.Currencies);
            SaveManager.Instance.Load<LocalBalancesData>(currenciesPath, LocalCurrenciesLoaded, encrypt);
        }

        private void LocalCurrenciesLoaded(LocalBalancesData currencies, SaveResult result, string message)
        {
            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No currencies data File Found -> Creating new data...");

            if (result == SaveResult.Success)
                m_CurrencyLoader.SetLocalBalances(currencies);
        }

        private void LoadEconomy()
        {
            m_InventoryLoader.SetData(m_CloudConnector.GetPlayerInventory());
            m_CloudConnector.OnLoadVirtualPurchase();
            m_CurrencyLoader.Init(m_CloudConnector.GetBalances());
            // m_CurrencyLoader.GrantMove(await m_CloudConnector.OnGrantMove());
        }

        public async Task RefreshEconomy()
        {
            m_CurrencyLoader.RefreshCurrencies(await m_CloudConnector.OnLoadCurrency());
            m_InventoryLoader.SetData(await m_CloudConnector.OnLoadInventory());
        }

        private async void RefreshBalances()
        {
            m_CurrencyLoader.ResetCurrencies(await m_CloudConnector.OnLoadCurrency());
        }

        public void IncrementLocalCurrency(string rewardID, int rewardAmount)
        {
            m_CurrencyLoader.GainCurrency(rewardID, rewardAmount);
        }

        public async void DeductCurrency(string currencyId, int amount)
        {
            await RefreshEconomy();
            m_CloudConnector.DeductCurrency(currencyId, amount);
            m_CurrencyLoader.DeductCurrency(currencyId, amount);
        }

        /// <summary>
        /// All currency increment must go through this function
        /// </summary>
        public void StoreCurrencyAtBuildings(string commandId, Vector3 fromPos)
        {
            if (commandId.Equals("NONE"))
                return;

            var rewards = m_CloudConnector.GetRewardByCommandId(commandId);
            foreach (var reward in rewards)
            {
                switch (reward.service)
                {
                    case "currency":
                    {
                        if (reward.id.Equals(CurrencyType.GOLD.ToString()) ||
                            reward.id.Equals(CurrencyType.GEM.ToString()) ||
                            reward.id.Equals(CurrencyType.MOVE.ToString()))
                            IncrementLocalCurrency(reward.id, reward.amount);
                        else
                            m_StorageHandler.StoreRewardAtBuildings(reward.id, reward.amount);

                        // show currency vfx
                        MainUI.Instance.OnShowCurrencyVfx.Invoke(reward.id, reward.amount, fromPos);

                        break;
                    }
                }
            }
        }

        public void StoreCurrencyAtBuildings(string currency, int amount)
        {
            if (currency.Equals(CurrencyType.GOLD.ToString()) ||
                currency.Equals(CurrencyType.GEM.ToString()) ||
                currency.Equals(CurrencyType.MOVE.ToString()))
                IncrementLocalCurrency(currency, amount);
            else
                m_StorageHandler.StoreRewardAtBuildings(currency, amount);
        }

        ///<summary>
        ///Store currency into buildings of a given EnvData
        ///</summary>
        public void StoreCurrencyByEnvData(string currencyId, int amount, EnvironmentData envData)
        {
            envData.StoreRewardAtBuildings(currencyId, amount);
        }

        /// <summary>
        /// All currency deduction must go through this function
        /// </summary>
        public void DeductCurrencyFromBuildings(string currencyId, int amount)
        {
            DeductCurrency(currencyId, amount);

            // if (currencyId.Equals(CurrencyType.GOLD.ToString()) ||
            //     currencyId.Equals(CurrencyType.GEM.ToString()) ||
            //     currencyId.Equals(CurrencyType.MOVE.ToString()))
            //     DeductCurrency(currencyId, amount);
            // else
            //     m_StorageHandler.DeductCurrencyFromBuildings(currencyId, amount);
        }

        public IEnumerable<PlayerBalance> GetCurrencies()
        {
            return m_CurrencyLoader.GetCurrencies();
        }

        public int GetCurrencyById(string currencyId)
        {
            return (int)m_CurrencyLoader.GetCurrency(currencyId);
        }

        public bool CheckEnoughCurrency(string currencyId, int currencyAmount)
        {
            return m_CurrencyLoader.CheckEnoughCurrency(currencyId, currencyAmount);
        }

        public void GrantCurrency(string currencyId, int amount)
        {
            m_CloudConnector.OnGrantCurrency(currencyId, amount);
            m_CurrencyLoader.GainCurrency(currencyId, amount);
        }

        public void GrantMoveForTest()
        {
            m_CurrencyLoader.GrantMove(50);
        }

        public void OnSetCloudCurrency(string currencyId, int amount)
        {
            if (currencyId == CurrencyType.MOVE.ToString())
            {
                var maxMove = m_CloudConnector.GetNumericByConfig(CommandName.JI_MAX_MOVE.ToString());
                amount = amount < maxMove ? amount : maxMove;
            }

            m_CloudConnector.OnSetCurrency(currencyId, amount);
        }

        public string GetCurrencySprite(string currencyId)
        {
            return m_CloudConnector.GetCurrencySprite(currencyId);
        }

        #endregion

        #region INVENTORY

        private async void ResetBasicInventory()
        {
            await m_CloudConnector.OnResetBasicInventory();
        }

        public void OnAskForShowingBuildingMenu()
        {
            m_InventoryLoader.SendInventoriesToBuildingMenu();
        }

        public void OnAskForShowingCreatureMenu()
        {
            m_InventoryLoader.SendInventoriesToCreatureMenu();
        }

        public void OnAskForShowingShoppingMenu()
        {
            m_CloudConnector.SendPurchasesToShoppingMenu();
        }

        // REFACTOR: Move any function related to this function to this script
        public JIInventoryItem GetInventoryItemByName(string entityName)
        {
            return m_CloudConnector.GetInventoryByNameOrId(entityName);
        }

        public async Task<PlayersInventoryItem> GrantInventory(string inventoryId)
        {
            return await m_CloudConnector.OnGrantInventory(inventoryId);
        }

        public async void GrantInventory(string inventoryId, int level)
        {
            await m_CloudConnector.OnGrantInventory(inventoryId, level);
        }

        public async void UpgradeInventory(string inventoryId)
        {
            await m_CloudConnector.OnUpdateInventory(inventoryId, 0);
        }

        public async void UpgradeInventory(string inventoryId, int level)
        {
            await m_CloudConnector.OnUpdateInventory(inventoryId, level);
            OnCreatureUpgrade.Invoke(inventoryId);
        }

        public int GetInventoryLevel(string inventoryId)
        {
            return m_CloudConnector.GetInventoryLevel(inventoryId);
        }

        #endregion

        #region VIRTUAL PURCHASE

        public VirtualPurchaseDefinition GetPurchaseDefinition(string id)
        {
            return m_CloudConnector.GetPurchaseDefinition(id);
        }

        public List<JIItemAndAmountSpec> GetPurchaseCost(string purchaseId)
        {
            return m_CloudConnector.GetVirtualPurchaseCost(purchaseId);
        }

        public List<JIItemAndAmountSpec> GetPurchaseReward(string purchaseId)
        {
            return m_CloudConnector.GetVirtualPurchaseReward(purchaseId);
        }

        public async Task<bool> OnConductVirtualPurchase(string virtualPurchaseId)
        {
            await RefreshEconomy();

            var purchaseHandler = await m_CloudConnector.OnMakeAPurchase(virtualPurchaseId);
            if (purchaseHandler == null)
            {
                MainUI.Instance.OnConversationUI.Invoke($"\"Lack of resource for {virtualPurchaseId}\"", false);
                return false;
            }

            // Pay for constructing the building...
            var constructingCost = m_CloudConnector.GetVirtualPurchaseCost(virtualPurchaseId);
            foreach (var cost in constructingCost)
                DeductCurrencyFromBuildings(cost.id, cost.amount);
            // m_CurrencyLoader.IncrementCurrency(cost.id, cost.amount * -1);

            var rewards = m_CloudConnector.GetVirtualPurchaseReward(virtualPurchaseId);
            foreach (var reward in rewards)
            {
                if (reward.id.Equals(CurrencyType.GOLD.ToString()) ||
                    reward.id.Equals(CurrencyType.GEM.ToString()) ||
                    reward.id.Equals(CurrencyType.MOVE.ToString()))
                    IncrementLocalCurrency(reward.id, reward.amount);
                else
                    m_StorageHandler.StoreRewardAtBuildings(reward.id, reward.amount);
            }

            return true;
        }

        #endregion

        #region COMMAND

        private void StackUpCommand(CommandName commandName)
        {
            m_CloudConnector.OnCommandStackUp(commandName);
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

        private void LoadCommands()
        {
            var commandPath = GetSavingPath(SavingPath.Commands);
            SaveManager.Instance.Load<CommandsCache>(commandPath, CommandWasLoaded, encrypt);
        }

        private async void CommandWasLoaded(CommandsCache commands, SaveResult result, string message)
        {
            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No Command data File Found -> Creating new data...");

            if (result == SaveResult.Success)
            {
                Debug.Log(
                    $"Commands Was Loaded:{result}\nNumber of commands: {commands.commandList.Count}\nMessage: {message}");

                // Just load command when the game disconnect in the latest session
                if (commands.commandList.Count > 0)
                    await m_CloudConnector.SubmitCommands(commands);
            }
        }

        #endregion

        #region LEADERBOARD

        public async Task<EnvironmentData> GetEnemyEnv()
        {
            return await m_CloudConnector.GetEnemyEnvironment();
        }

        public async Task<List<LeaderboardEntry>> GetPlayerRange()
        {
            return await m_CloudConnector.GetPlayerRange();
        }

        private int GetScore()
        {
            return m_CloudConnector.GetPlayerScore();
        }

        public void GainExp(int amount)
        {
            m_CloudConnector.PlayerRecordExp(amount);
        }

        public int GetPlayerExp()
        {
            return m_CloudConnector.GetPlayerExp();
        }

        public string GetEnemyId()
        {
            return m_CloudConnector.GetEnemyId();
        }

        #endregion

        #region GAME PROCESS

        private void LoadGameProcess()
        {
            m_GameProcess = m_CloudConnector.OnLoadGameProcess();

            if (GameFlowManager.Instance.GameMode == GameMode.ECONOMY)
                GameFlowManager.Instance.LoadTutorialManager(m_GameProcess == null
                    ? "/Tutorials/Tutorial0"
                    : m_GameProcess.currentTutorial);
        }

        public async void SaveCurrentTutorial(string tutorial)
        {
            m_GameProcess.currentTutorial = tutorial;
            await m_CloudConnector.OnSaveGameProcess(m_GameProcess);
        }

        public async void SaveBattleResult(string enemyId, int starAmount, int score, float winRate)
        {
            if (starAmount == 0)
                m_GameProcess.winStack = 0;
            else
                m_GameProcess.winStack++;

            switch (starAmount)
            {
                case 1:
                    m_GameProcess.win1StarCount++;
                    break;
                case 2:
                    m_GameProcess.win2StarCount++;
                    break;
                case 3:
                    m_GameProcess.win3StarCount++;
                    break;
            }

            m_GameProcess.battleCount++;
            m_GameProcess.score = Mathf.Clamp(m_GameProcess.score + score, 0, m_GameProcess.score + score);

            // var battleRecord = GetComponent<BattleRecorder>().GetBattleRecord();
            // battleRecord.winStar = starAmount;
            // battleRecord.winStack = m_GameProcess.winStack;
            // battleRecord.winRate = winRate;
            // battleRecord.score = score;
            // battleRecord.isRecorded = true;

            // Send an email to enemy
            // m_CloudConnector.AddBattleEmail(enemyId ,battleRecord);

            m_CloudConnector.PlayerRecordScore(m_GameProcess.score);
            await m_CloudConnector.OnSaveGameProcess(m_GameProcess);
        }

        public async void SaveBossBattle()
        {
            m_GameProcess.battleCount++;
            await m_CloudConnector.OnSaveGameProcess(m_GameProcess);
        }

        public async void SaveBossUnlock(int bossIndex)
        {
            m_GameProcess.bossUnlock = bossIndex;
            await m_CloudConnector.OnSaveGameProcess(m_GameProcess);
        }

        public async void SaveEcoBoss(int bossIndex)
        {
            m_GameProcess.ecoBoss = bossIndex;
            await m_CloudConnector.OnSaveGameProcess(m_GameProcess);
        }

        #endregion

        #region CUSTOM EVENTS SENDER

        public void SendBossQuestEvent(int bossId)
        {
            m_CloudConnector.SendBossQuestEvent(GetScore(), bossId);
        }

        public void SendTutorialTrackEvent(string stepId)
        {
            m_CloudConnector.SendTutorialTrackEvent(stepId);
        }

        #endregion

        #region RESEARCHES

        public IEnumerable<Research> GetResearchTopics()
        {
            return m_ResearchSup.GetTopics();
        }

        // public void RemoveResearch(Research research)
        // {
        //     m_ResearchSup.RemoveResearch(research);
        // }

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

        /// <summary>
        /// In battleMode, EnvData for saving is player's envData, and EnvData from GetEnvData is enemy's envData
        /// </summary>
        public EnvironmentData GetEnvDataForSave()
        {
            return m_EnvLoader.GetDataForSave();
        }

        public GameProcessData GetGameProcess()
        {
            return m_GameProcess;
        }

        public IEnvironmentLoader GetEnvLoader()
        {
            return m_EnvLoader;
        }

        public IStoragesControl GetStorageController()
        {
            return m_StorageController;
        }

        // public IMonsterControler GetMonsterController()
        // {
        //     return m_MonsterController;
        // }

        #endregion
    }
    
    public enum SavingPath
    {
        PlayerEnvData,
        Currencies,
        Commands,
        GameState,
        QuestData
    }
}