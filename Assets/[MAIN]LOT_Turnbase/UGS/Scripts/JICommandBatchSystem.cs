using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class JICommandBatchSystem : MonoBehaviour
    {
        readonly Queue<JICommand> commandBatch = new();
        
        public void EnqueueCommand(JICommand command)
        {
            commandBatch.Enqueue(command);
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
            var batchSize = commandBatch.Count;
            var commandKeys = new string[batchSize];

            for (var i = 0; i < batchSize; i++)
            {
                commandKeys[i] = commandBatch.Dequeue().GetKey();
            }

            return commandKeys;
        }

        async Task CallCloudCodeEndpoint(string[] commandKeys, JICloudCodeManager cloudCodeManager)
        {
            await cloudCodeManager.CallProcessBatchEndpoint(commandKeys);
        }

        public int CountCommand()
        {
            return commandBatch.Count;
        }
    }
}
