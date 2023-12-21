using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Economy.Model;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class JICloudConnector : Singleton<JICloudConnector>
    {
        [SerializeField] protected JIEconomyManager _economyManager;
        [SerializeField] private JICloudCodeManager _cloudCodeManager;
        [SerializeField] private JICommandBatchSystem _commandBatchManager;
        [SerializeField] private JIRemoteConfigManager _remoteConfigManager;
        [SerializeField] private JILeaderboardManager _leaderboardManager;
        [SerializeField] private JICustomEventSender _customEventSender;
        [SerializeField] private JICloudSaveManager _cloudSaveManager;
        [SerializeField] private JILocalSaveManager _localSaveManager;
        [SerializeField] private string[] m_BasicInventory;

        private string _enemyPlayerId;
        private bool _isInit;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public async Task Init()
        {
            try
            {
                if (_isInit)
                    return;
                
                var options = new InitializationOptions();
                options.SetEnvironmentName("dev");
                await UnityServices.InitializeAsync(options);

                // Check that scene has not been unloaded while processing async wait to prevent throw.
                if (this == null)
                    return;

                if (await SignIn()) 
                    return;
                
                if (this == null)
                    return;
                
                await _economyManager.RefreshEconomyConfiguration();
                if (this == null)
                    return;
                
                await _leaderboardManager.RefreshBoards();
                if (this == null)
                    return;
                
                if (_cloudSaveManager != null)
                {
                    await _cloudSaveManager.Init();
                    if (this == null)
                        return;
                }
                
                await FetchUpdatedServicesData();
                if (this == null) return;
                
                Debug.Log("Initialization and signin complete.");
                _isInit = true;
            }
            catch (AuthenticationException ex)
            {
                // Debug.LogException(ex);
                Debug.Log($"The playerID has been removed. Token exist: {AuthenticationService.Instance.SessionTokenExists}");
                await Init();
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }  
        }

        private async Task<bool> SignIn()
        {
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    if (this == null)
                        return true;
                }

                Debug.Log($"Player id when token exist:{AuthenticationService.Instance.PlayerId}");
                _localSaveManager.LoadEnvironment();
                var mainHall = _localSaveManager.GetEnvData().BuildingData.Find(t => t.BuildingType == BuildingType.MAINHALL);
                await FetchEnvRelevantData(mainHall.CurrentLevel);
            }
            else
            {
                Debug.Log("The playerId still not exist");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (this == null)
                    return true;
                Debug.Log($"Player id new one:{AuthenticationService.Instance.PlayerId}");

                var cloudEnvData = await OnResetEnvData();
                // m_EnvData = await OnResetEnvData();

                if (cloudEnvData != null)
                {
                    _localSaveManager.SavePlayerEnv(cloudEnvData);
                    await OnLoadCurrency();
                    await OnResetBasicInventory();
                    await _leaderboardManager.AddScore(0);
                }
            }

            return false;
        }

        async Task FetchUpdatedServicesData()
        {
            await Task.WhenAll(
                OnLoadInventory(),
                OnLoadCurrency(),
                _remoteConfigManager.FetchCommandConfigs()
            );
        }

        public async Task FetchEnvRelevantData(int mainHallLevel)
        {
            await _remoteConfigManager.FetchMainHallTierConfigs(mainHallLevel);
        }

        #region ENVIRONMENT DATA

        public async Task OnSaveEnvData()
        {
            await _cloudCodeManager.CallSaveEnvData(SavingSystemManager.Instance.GetEnvDataForSave());
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

        public async Task OnSaveEnvById(EnvironmentData envData, string playerId)
        {
            await _cloudCodeManager.SaveEnvById(envData, playerId);
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

        public List<PlayerBalance> GetBalances()
        {
            return _economyManager.GetBalances();
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

        public async void OnGrantCurrency(string currencyId, int amount)
        {
            await _economyManager.OnGrantCurrency(currencyId, amount);
        }

        public async void DeductCurrency(string currencyId, int amount)
        {
            await _economyManager.DeductCurrency(currencyId, amount);
        }

        public async void OnSetCurrency(string currencyId, int amount)
        {
            await _economyManager.OnSetCurrency(currencyId, amount);
        }

        public string GetCurrencySprite(string currencyId)
        {
            return _economyManager.GetSpriteAddress(currencyId);
        }

        #endregion

        #region INVENTORY DATA

        public async Task OnResetBasicInventory()
        {
            await _economyManager.ClearInventory();

            foreach (var inventoryId in m_BasicInventory)
                await OnGrantInventory(inventoryId);

            await OnLoadInventory(); // Refresh new currency
        }

        public List<PlayersInventoryItem> GetPlayerInventory()
        {
            return _economyManager.GetPlayerInventory();
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
            {
                if (itemDefinition.Name.Equals(inventoryInfo) || itemDefinition.Id.Equals(inventoryInfo))
                    return itemDefinition.CustomDataDeserializable.GetAs<JIInventoryItem>();
            }

            return null;
        }

        public async Task<PlayersInventoryItem> OnGrantInventory(string inventoryId)
        {
            return await _economyManager.OnGrantInventory(inventoryId);
        }

        public async Task OnGrantInventory(string inventoryId, int level)
        {
            await _economyManager.OnGrantInventory(inventoryId, level);
        }

        public async Task OnUpdateInventory(string inventoryId, int level)
        {
            await _economyManager.OnUpdatePlayerInventory(inventoryId, level);
        }

        public int GetInventoryLevel(string inventoryId)
        {
            return _economyManager.GetInventoryLevel(inventoryId);
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

        public List<JIItemAndAmountSpec> GetVirtualPurchaseReward(string virtualPurchaseId)
        {
            return _economyManager.GetVirtualPurchaseReward(virtualPurchaseId);
        }

        public VirtualPurchaseDefinition GetPurchaseDefinition(string id)
        {
            return _economyManager.GetPurchaseDefinition(id);
        }

        public void SendPurchasesToShoppingMenu()
        {
            MainUI.Instance.OnShowShoppingMenu.Invoke(_economyManager.GetPurchasedDefinitions());
        }

        #endregion

        #region REMOTE CONFIG

        public List<JIRemoteConfigManager.Reward> GetRewardByCommandId(string commandId)
        {
            return _remoteConfigManager.commandRewards[commandId];
        }

        public int GetNumericByConfig(string configName)
        {
            return _remoteConfigManager.numericConfig[configName];
        }

        public async Task<JIRemoteConfigManager.BattleLoot> GetBattleWinLoot(int star)
        {
            var battleConfig = "";
            switch (star)
            {
                case 1:
                {
                    battleConfig = JIRemoteConfigManager.BattleWinConfigName.JI_BATTLEWIN_1STAR.ToString();
                    break;
                }
                case 2:
                {
                    battleConfig = JIRemoteConfigManager.BattleWinConfigName.JI_BATTLEWIN_2STAR.ToString();
                    break;
                }
                case 3:
                {
                    battleConfig = JIRemoteConfigManager.BattleWinConfigName.JI_BATTLEWIN_3STAR.ToString();
                    break;
                }
            }

            return await _remoteConfigManager.GetBattleWinConfigs(await _leaderboardManager.UpdatePlayerScore(),
                battleConfig);
        }

        public MainHallTier GetCurrentTier()
        {
            return _remoteConfigManager.curTier;
        }

        public MainHallTier GetNextTier()
        {
            return _remoteConfigManager.nextTier;
        }

        #endregion

        #region LEADERBOARD

        public async Task<EnvironmentData> GetEnemyEnvironment()
        {
            var getPlayerRange = await _leaderboardManager.GetPlayerRange();

            getPlayerRange = getPlayerRange.FindAll(t =>
                t.PlayerId.Equals(AuthenticationService.Instance.PlayerId) == false);

            _enemyPlayerId = getPlayerRange[Random.Range(0, getPlayerRange.Count)].PlayerId;
            return await _cloudCodeManager.CallLoadEnemyEnvironment(_enemyPlayerId);
        }

        public async Task<List<LeaderboardEntry>> GetPlayerRange()
        {
            return await _leaderboardManager.GetPlayerRange();
        }

        public void PlayerRecordScore(int playerScore)
        {
            _leaderboardManager.AddScore(playerScore);
        }

        public int GetPlayerScore()
        {
            return _leaderboardManager.GetPlayerScore();
        }

        public void PlayerRecordExp(int playerExp)
        {
            _leaderboardManager.AddExp(playerExp);
        }

        public int GetPlayerExp()
        {
            return _leaderboardManager.GetPlayerExp();
        }

        public string GetEnemyId()
        {
            return _enemyPlayerId;
        }

        #endregion

        #region GAME PROCESS

        public GameProcessData OnLoadGameProcess()
        {
            return _cloudSaveManager.FetchGameProcess();
        }

        public async Task OnSaveGameProcess(GameProcessData currentProcess)
        {
            await _cloudSaveManager.SaveGameProcess(currentProcess);
            // await _cloudCodeManager.CallSaveGameProcess(currentProcess);
        }

        public async Task<long> OnGrantMove()
        {
            return await _cloudCodeManager.CallGrantMove();
        }

        #endregion

        #region CUSTOM EVENT SENDER

        public void SendBossQuestEvent(int playerScore, int bossId)
        {
            _customEventSender.SendBossQuestEvent(playerScore, bossId);
        }

        public void SendTutorialTrackEvent(string stepId)
        {
            _customEventSender.SendTutorialTrackEvent(stepId);
        }

        #endregion

        #region CLOUDSAVE

        public void AddBattleEmail(string playerId, BattleRecord battleRecord)
        {
            _cloudSaveManager.AddBattleMail(playerId, battleRecord);
        }

        #endregion

        #region LOCAL SAVER

        public ILocalSaver GetLocalSaver()
        {
            return _localSaveManager;
        }

        #endregion
    }
}