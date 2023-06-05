using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class JICommandBatchSystem : MonoBehaviour
    {
        readonly CommandsCache _commands = new();
        // readonly Queue<JICommand> _commands = new();

        public void EnqueueCommand(JICommand command)
        {
            _commands.commandBatch.Add(command);
            // _commands.Enqueue(command);
            Debug.Log($"Number of commands: {_commands.commandBatch.Count}");
        }

        public async Task FlushBatch(JICloudCodeManager cloudCodeManager)
        {
            try
            {
                var commandKeys = ConvertCommandBatchToCommandKeys();
                await CallCloudCodeEndpoint(commandKeys, cloudCodeManager);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        string[] ConvertCommandBatchToCommandKeys()
        {
            var batchSize = _commands.commandBatch.Count;
            var commandKeys = new string[batchSize];

            for (var i = 0; i < batchSize; i++)
            {
                commandKeys[i] = _commands.commandBatch[i].GetKey().ToString();
            }

            return commandKeys;
        }

        async Task CallCloudCodeEndpoint(string[] commandKeys, JICloudCodeManager cloudCodeManager)
        {
            await cloudCodeManager.CallProcessBatchEndpoint(commandKeys);
        }

        public CommandsCache GetCommandsForSaving()
        {
            _commands.CreateCommandList();
            return _commands;
        }

        public async Task SubmitListCommands(List<CommandName> commandNames, JICloudCodeManager cloudCodeManager)
        {
            try
            {
                var commandKeys = ConvertCommandNameToCommandKeys(commandNames);
                await CallCloudCodeEndpoint(commandKeys, cloudCodeManager);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
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