using System;
using JumpeeIsland;
using UnityEngine.Serialization;

namespace Unity.Services.Samples.InGameMailbox
{
    [Serializable]
    public class BattleMessage : InboxMessage
    {
        public EnvironmentData environmentData;

        public BattleMessage(string messageId = "", MessageInfo messageInfo = null, MessageMetadata metadata = null, EnvironmentData environmentData = null)
        {
            this.messageId = messageId;
            this.messageInfo = messageInfo;
            this.metadata = metadata;
            this.environmentData = environmentData;
        }
    }
}