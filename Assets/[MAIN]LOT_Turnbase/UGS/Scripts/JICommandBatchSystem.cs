using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class JICommandBatchSystem : MonoBehaviour
    {
        // private Queue<JICommand> commandBatch = new();
        readonly Queue<JICommand> _commands = new();

        public void EnqueueCommand(JICommand command)
        {
            _commands.Enqueue(command);
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
            var batchSize = _commands.Count;
            var commandKeys = new string[batchSize];

            for (var i = 0; i < batchSize; i++)
            {
                commandKeys[i] = _commands.Dequeue().GetKey();
            }

            return commandKeys;
        }

        async Task CallCloudCodeEndpoint(string[] commandKeys, JICloudCodeManager cloudCodeManager)
        {
            await cloudCodeManager.CallProcessBatchEndpoint(commandKeys);
        }

        public int CountCommand()
        {
            return _commands.Count;
        }
    }
}