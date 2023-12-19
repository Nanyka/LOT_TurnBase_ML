using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using Unity.Services.Samples.CommandBatching;
using UnityEngine;

namespace JumpeeIsland
{
    public class JIRemoteConfigManager : MonoBehaviour
    {
        public static JIRemoteConfigManager instance { get; private set; }
        public Dictionary<string, List<Reward>> commandRewards = new(5);
        public Dictionary<string, int> numericConfig = new();
        private Dictionary<string, BattleLoot> BattleLoots = new();
        public MainHallTier curTier{ get; private set; }
        public MainHallTier nextTier{ get; private set; }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        #region STATIC CONFIG

        public async Task FetchCommandConfigs()
        {
            try
            {
                await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());

                // Check that scene has not been unloaded while processing async wait to prevent throw.
                if (this == null) return;

                // GetEconomyConfigValues(); // TODO: check removing the Command system
                GetNumericConfigValues();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void GetEconomyConfigValues()
        {
            Debug.Log("Got economic config value");
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_SPEND_MOVE.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_NEUTRAL_WOOD_1_0.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_NEUTRAL_FOOD_1_0.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_FOOD_1.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_FOOD_5.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_FOOD_20.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_FOOD_50.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_FOOD_100.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_FOOD_200.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_FOOD_500.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_WOOD_1.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_WOOD_5.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_WOOD_20.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_WOOD_50.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_WOOD_100.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_WOOD_200.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_WOOD_500.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_COIN_1.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_COIN_5.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_COIN_20.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_COIN_50.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_COIN_100.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_COIN_200.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_COIN_500.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GOLD_1.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GOLD_5.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GOLD_20.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GOLD_50.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GOLD_100.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GOLD_200.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GOLD_500.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GEM_1.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GEM_5.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GEM_20.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GEM_50.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GEM_100.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GEM_200.ToString());
            GetAppConfigCommandRewardsAndProcess(CommandName.JI_GEM_500.ToString());
        }

        void GetAppConfigCommandRewardsAndProcess(string commandKey)
        {
            var json = RemoteConfigService.Instance.appConfig.GetJson(commandKey);
            var commandReward = JsonUtility.FromJson<CommandReward>(json);
            commandRewards[commandKey] = commandReward.rewards;
        }

        private void GetNumericConfigValues()
        {
            Debug.Log("Got numeric config value");
            GetNumericConfig(CommandName.JI_MAX_MOVE.ToString());
            GetNumericConfig(NumericConfigName.JI_COLLECT_CREATURE_RATE.ToString());
            GetNumericConfig(NumericConfigName.JI_TOWNHOUSE_SPACE.ToString());
        }

        private void GetNumericConfig(string configKey)
        {
            var numericValue = RemoteConfigService.Instance.appConfig.GetInt(configKey);
            numericConfig[configKey] = numericValue;
        }
        
        // Remote Config's FetchConfigs call requires passing two non-nullable objects to the method, regardless of
        // whether any data needs to be passed in them. Candidates for what you may want to pass in the UserAttributes
        // struct could be things like device type, however it is completely customizable.
        public struct UserAttributes
        {
        }

        // Candidates for what you can pass in the AppAttributes struct could be things like what level the player
        // is on, or what version of the app is installed. The candidates are completely customizable.
        public struct AppAttributes
        {
        }

        [Serializable]
        public struct CommandReward
        {
            public List<Reward> rewards;
        }

        [Serializable]
        public struct Reward
        {
            public string service;
            public string id;
            public int amount;
        }

        #endregion

        #region LEVEL-BASED CONFIG

        public async Task<BattleLoot> GetBattleWinConfigs(int playerRange ,string battleWinConfig)
        {
            try
            {
                var userAttribute = new BattleWinUserAttributes { playerScore = playerRange };

                await RemoteConfigService.Instance.FetchConfigsAsync(userAttribute, new BattleWinAppAttributes());

                // Check that scene has not been unloaded while processing async wait to prevent throw.
                if (this == null) return null;

                CacheBattleWinConfigs();
                return BattleLoots[battleWinConfig];
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private void CacheBattleWinConfigs()
        {
            GetBattleWinConfig(BattleWinConfigName.JI_BATTLEWIN_1STAR.ToString());
            GetBattleWinConfig(BattleWinConfigName.JI_BATTLEWIN_2STAR.ToString());
            GetBattleWinConfig(BattleWinConfigName.JI_BATTLEWIN_3STAR.ToString());
        }

        private void GetBattleWinConfig(string configKey)
        {
            var json = RemoteConfigService.Instance.appConfig.GetJson(configKey);
            var battleLoot = JsonUtility.FromJson<BattleLoot>(json);
            BattleLoots[configKey] = battleLoot;
        }

        // Remote Config's FetchConfigs call requires passing two non-nullable objects to the method, regardless of
        // whether any data needs to be passed in them. Candidates for what you may want to pass in the UserAttributes
        // struct could be things like device type, however it is completely customizable.
        private struct BattleWinUserAttributes
        {
            public int playerScore;
        }

        // Candidates for what you can pass in the AppAttributes struct could be things like what level the player
        // is on, or what version of the app is installed. The candidates are completely customizable.
        private struct BattleWinAppAttributes
        {
        }
        
        public class BattleLoot
        {
            public List<string> CurrencyLoots;
            public int AmountOfWithdraw; // The reward is a bundle of AmountOfWithdraw commands from CurrencyLoots
            public List<string> CreatureLoot; // Select one of this creature
        }
        
        public enum BattleWinConfigName
        {
            JI_BATTLEWIN_1STAR,
            JI_BATTLEWIN_2STAR,
            JI_BATTLEWIN_3STAR
        }

        #endregion

        #region MAINHALL TIERs

        public async Task FetchMainHallTierConfigs(int mainHallLevel)
        {
            curTier = await GetMainHallTierConfigs(mainHallLevel);
            nextTier = await GetMainHallTierConfigs(mainHallLevel+1);
        }

        private async Task<MainHallTier> GetMainHallTierConfigs(int curMainHallLevel)
        {
            try
            {
                var userAttribute = new MainHallTierUserAttribute { currentLevel = curMainHallLevel };
                await RemoteConfigService.Instance.FetchConfigsAsync(userAttribute, new MainHallTierAppAttribute());

                // Check that scene has not been unloaded while processing async wait to prevent throw.
                if (this == null) return null;
                return GetTierConfig();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private MainHallTier GetTierConfig()
        {
            var json = RemoteConfigService.Instance.appConfig.GetJson("JI_MAINHALL_TIER");
            return JsonUtility.FromJson<MainHallTier>(json);
        }
        
        private struct MainHallTierUserAttribute
        {
            public int currentLevel;
        }
        
        private struct MainHallTierAppAttribute { }

        #endregion

        #region MAILBOX

        private SampleAudience m_CurrentAudience = SampleAudience.Default;
        private List<string> m_OrderedMessageIds;

        public async Task FetchMailboxConfigs()
        {
            try
            {
                var userAttribute = new MailboxUserAttributes() { audience = m_CurrentAudience.ToString() };

                await RemoteConfigService.Instance.FetchConfigsAsync(userAttribute, new MailboxAppAttributes());

                // Check that scene has not been unloaded while processing async wait to prevent throw.
                if (this == null) return;

                CacheConfigValues();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        void CacheConfigValues()
        {
            var json = RemoteConfigService.Instance.appConfig.GetJson("MESSAGES_ALL", "");
            
            Debug.Log(json);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("Remote config key \"MESSAGES_ALL\" cannot be found.");
                return;
            }

            var messageIds = JsonUtility.FromJson<MessageIds>(json);
            m_OrderedMessageIds = messageIds.messageList;
        }

        public void UpdateAudienceType(SampleAudience newAudience)
        {
            m_CurrentAudience = newAudience;
        }

        public List<InboxMessage> GetNextMessages(int numberOfMessages, string lastMessageId = "")
        {
            if (m_OrderedMessageIds is null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(lastMessageId))
            {
                return GetNextMessagesFromStartLocation(0, numberOfMessages);
            }

            for (var i = 0; i < m_OrderedMessageIds.Count; i++)
            {
                if (string.Equals(m_OrderedMessageIds[i], lastMessageId) && i + 1 < m_OrderedMessageIds.Count)
                {
                    return GetNextMessagesFromStartLocation(i + 1, numberOfMessages);
                }
            }

            return null;
        }

        List<InboxMessage> GetNextMessagesFromStartLocation(int startLocation, int numberOfMessages)
        {
            var newMessages = new List<InboxMessage>();

            for (var i = startLocation; i < m_OrderedMessageIds.Count; i++)
            {
                if (numberOfMessages > 0)
                {
                    var message = FetchMessage(m_OrderedMessageIds[i]);

                    // Some message values will be blank if the player does not fall into a targeted audience.
                    // We want to filter those messages out when downloading a specific number of messages.
                    if (MessageIsValid(message))
                    {
                        newMessages.Add(message);
                        numberOfMessages--;
                    }
                }

                if (numberOfMessages == 0)
                {
                    break;
                }
            }

            return newMessages;
        }

        InboxMessage FetchMessage(string messageId)
        {
            var json = RemoteConfigService.Instance.appConfig.GetJson(messageId, "");
            Debug.Log($"Get message {messageId} with info: {json}");
            
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"Remote config key {messageId} cannot be found.");
                return new InboxMessage();
            }

            var message = JsonUtility.FromJson<MessageInfo>(json);

            return message == null
                ? new InboxMessage()
                : new InboxMessage(messageId, message);
        }

        bool MessageIsValid(InboxMessage inboxMessage)
        {
            var message = inboxMessage.messageInfo;
            
            if (string.IsNullOrEmpty(inboxMessage.messageId) || message == null ||
                string.IsNullOrEmpty(message.title) || string.IsNullOrEmpty(message.content) ||
                string.IsNullOrEmpty(message.expiration) || !TimeSpan.TryParse(message.expiration,
                    new CultureInfo("en-US"), out var timespan))
            {
                return false;
            }

            return true;
        }
    
        public enum SampleAudience
        {
            Default,
            AllSpenders,
            UnengagedPlayers,
            FrenchSpeakers,
            NewPlayers
        }
        
        struct MailboxUserAttributes
        {
            public string audience;
        }
        
        struct MailboxAppAttributes { }
        
        [Serializable]
        public struct MessageIds
        {
            public List<string> messageList;
        }

        #endregion

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
    
    public enum NumericConfigName
    {
        JI_COLLECT_CREATURE_RATE,
        JI_TOWNHOUSE_SPACE
    }
        
    [Serializable]
    public class MainHallTier
    {
        public int MaxAmountOfBuilding;
        public List<TierItem> TierItems = new();
        public List<Research> UnlockedResearches = new();
    }
    
    [Serializable]
    public class TierItem
    {
        public string itemName;
        public string inventoryId;
        public int amount;
    }
}