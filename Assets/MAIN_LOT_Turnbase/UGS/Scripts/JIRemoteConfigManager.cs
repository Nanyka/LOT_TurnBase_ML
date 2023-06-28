using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using Unity.Services.Samples.CommandBatching;
using UnityEngine;

namespace JumpeeIsland
{
    public class JIRemoteConfigManager : MonoBehaviour
    {

        public Dictionary<string, List<Reward>> commandRewards = new(5);

        public async Task FetchConfigs()
        {
            try
            {
                await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());

                // Check that scene has not been unloaded while processing async wait to prevent throw.
                if (this == null) return;

                GetConfigValues();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void GetConfigValues()
        {
            Debug.Log("Got config value");
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

        // Remote Config's FetchConfigs call requires passing two non-nullable objects to the method, regardless of
        // whether any data needs to be passed in them. Candidates for what you may want to pass in the UserAttributes
        // struct could be things like device type, however it is completely customizable.
        public struct UserAttributes { }

        // Candidates for what you can pass in the AppAttributes struct could be things like what level the player
        // is on, or what version of the app is installed. The candidates are completely customizable.
        public struct AppAttributes { }

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
    }
}