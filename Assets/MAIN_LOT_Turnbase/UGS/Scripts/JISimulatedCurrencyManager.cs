using System;
using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class JISimulatedCurrencyManager : MonoBehaviour
    {
        // We track the timestamp offset between local time and server time so we can simulate ticking
        // water at the same time as the server would actually grant if updated so client and server
        // can always be synced.
        public long serverTimestampOffset { get; private set; }
        
        public void UpdateServerTimestampOffset(long serverTimestamp)
        {
            var localTimestamp = GetLocalTimestamp();
            serverTimestampOffset = serverTimestamp - localTimestamp;
        }
        
        long GetLocalTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}
