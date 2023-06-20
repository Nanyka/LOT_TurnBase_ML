using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class JICommandBatchSystem : MonoBehaviour
    {
        private CommandsCache _commands = new();

        public void EnqueueCommand(JICommand command)
        {
            _commands.commandBatch.Add(command);
        }

        // public async Task FlushBatch(JICloudCodeManager cloudCodeManager)
        // {
        //     try
        //     {
        //         var commandKeys = ConvertCommandBatchToCommandKeys();
        //         await CallCloudCodeEndpoint(commandKeys, cloudCodeManager);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //     }
        // }

        // string[] ConvertCommandBatchToCommandKeys()
        // {
        //     var batchSize = _commands.commandBatch.Count;
        //     var commandKeys = new string[batchSize];
        //
        //     for (var i = 0; i < batchSize; i++)
        //     {
        //         commandKeys[i] = _commands.commandBatch[i].GetKey().ToString();
        //     }
        //
        //     return commandKeys;
        // }

        async Task CallCloudCodeEndpoint(string[] commandKeys, JICloudCodeManager cloudCodeManager)
        {
            await cloudCodeManager.CallProcessBatchEndpoint(commandKeys);
        }

        public CommandsCache GetCommandsForSaving()
        {
            _commands.CreateCommandList();
            return _commands;
        }

        public async Task SubmitListCommands(CommandsCache commandCache, JICloudCodeManager cloudCodeManager, JIRemoteConfigManager remoteConfigManager)
        {
            try
            {
                var commandKeys = ConvertCommandNameToCommandKeys(commandCache.commandList);
                
                // Update local UI
                foreach (var command in commandKeys)
                {
                    var rewards = remoteConfigManager.commandRewards[command];
                    foreach (var reward in rewards)
                    {
                        switch (reward.service)
                        {
                            case "currency":
                                SavingSystemManager.Instance.IncrementLocalCurrency(reward.id, reward.amount);
                                break;
                        }
                    }
                }
                
                // Contribute the previous session changes to cloud
                await CallCloudCodeEndpoint(commandKeys, cloudCodeManager);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _commands = commandCache;
            }
        }
        
        string[] ConvertCommandNameToCommandKeys(List<CommandName> nameList)
        {
            var batchSize = nameList.Count;
            var commandKeys = new string[batchSize];

            for (var i = 0; i < batchSize; i++)
            {
                commandKeys[i] = nameList[i].ToString();
            }

            return commandKeys;
        }
    }
}