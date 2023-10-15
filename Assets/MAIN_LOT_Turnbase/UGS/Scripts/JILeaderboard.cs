using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace JumpeeIsland
{
    public class JILeaderboard : MonoBehaviour
    {
        [SerializeField] private JICloudCodeManager m_CloudCode;
        
        // Create a leaderboard with this ID in the Unity Dashboard
        private string LeaderboardId = "my-first-leaderboard";

        string VersionId { get; set; }
        int Offset { get; set; }
        int Limit { get; set; }
        int RangeLimit { get; set; }
        List<string> FriendIds { get; set; }

        async void Awake()
        {
            var options = new InitializationOptions();
            options.SetEnvironmentName("dev");
            await UnityServices.InitializeAsync(options);

            await SignInAnonymously();
        }

        async Task SignInAnonymously()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
            };
            AuthenticationService.Instance.SignInFailed += s =>
            {
                // Take some action here...
                Debug.Log(s);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void AddScore()
        {
            var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, 102);
            Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        }

        public async void GetScores()
        {
            var scoresResponse =
                await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
            Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        }

        public async void GetPlayerScore()
        {
            var scoreResponse =
                await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
            Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        }
        
        public async Task<List<LeaderboardEntry>> GetPlayerRange()
        {
            // Returns a total of 11 entries (the given player plus 5 on either side)
            var rangeLimit = 5;
            var scoresResponse = await LeaderboardsService.Instance.GetPlayerRangeAsync(
                LeaderboardId,
                new GetPlayerRangeOptions{ RangeLimit = rangeLimit }
            );
            Debug.Log(JsonConvert.SerializeObject(scoresResponse));
            return scoresResponse.Results;
        }

        public async void GetEnemyEnv()
        {
            var getPlayerRange = await GetPlayerRange();

            foreach (var entry in getPlayerRange)
            {
                Debug.Log(entry.PlayerId);
            }
            
            var enemyEnv = await m_CloudCode.CallLoadEnemyEnvironment(getPlayerRange[5].PlayerId);
            Debug.Log($"Enemy environment:\n{enemyEnv.BuildingData[0].EntityName}");
        }
    }
}