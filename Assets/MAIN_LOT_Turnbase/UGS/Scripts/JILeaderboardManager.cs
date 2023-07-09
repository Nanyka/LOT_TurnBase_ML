using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace JumpeeIsland
{
    public class JILeaderboardManager : MonoBehaviour
    {
        [SerializeField] private string _leaderboardId;
        [Tooltip("Returns a total of rangeLimit*2+1 entries (the given player plus rangeLimit on either side)")]
        [SerializeField] private int _rangeLimit = 5;
        
        public async void AddScore(int playerScore)
        {
            var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(_leaderboardId, playerScore);
        }

        public async void GetScores()
        {
            var scoresResponse =
                await LeaderboardsService.Instance.GetScoresAsync(_leaderboardId);
            // Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        }

        public async Task<int> GetPlayerScore()
        {
            var scoreResponse =
                await LeaderboardsService.Instance.GetPlayerScoreAsync(_leaderboardId);
            return (int)scoreResponse.Score;
        }
        
        public async Task<List<LeaderboardEntry>> GetPlayerRange()
        {
            var scoresResponse = await LeaderboardsService.Instance.GetPlayerRangeAsync(
                _leaderboardId,
                new GetPlayerRangeOptions{ RangeLimit = _rangeLimit }
            );
            
            return scoresResponse.Results;
        }
    }
}