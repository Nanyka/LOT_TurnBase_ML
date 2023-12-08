using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class JILeaderboardManager : MonoBehaviour
    {
        [Tooltip("Returns a total of rangeLimit*2+1 entries (the given player plus rangeLimit on either side)")]
        [SerializeField] private int _rangeLimit = 5;
        
        private string _battleScoreId = "my-first-leaderboard";
        private string _expBoard = "experience-leaderboard";
        private int _playerScore;
        private int _playerExp;

        #region BATTLE SCORE

        public async Task RefreshBoards()
        {
            // await Task.WhenAll(RefreshPlayerScore(), RefreshPlayerExp());
            await RefreshPlayerScore();
            await RefreshPlayerExp();
        }

        private async Task RefreshPlayerScore()
        {
            _playerScore = await UpdatePlayerScore();
        }
        
        public async void AddScore(int playerScore)
        {
            var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(_battleScoreId, playerScore);
        }

        public async void GetScores()
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(_battleScoreId);
            // Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        }

        public async Task<int> UpdatePlayerScore()
        {
            try
            {
                var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(_battleScoreId);
                return (int)scoreResponse.Score;
            }
            catch (Exception e)
            {
                Debug.Log("Player still not on the Leaderboard");
                await LeaderboardsService.Instance.AddPlayerScoreAsync(_battleScoreId, 0);
                return 0;
            }
        }
        
        public async Task<List<LeaderboardEntry>> GetPlayerRange()
        {
            var scoresResponse = await LeaderboardsService.Instance.GetPlayerRangeAsync(
                _battleScoreId,
                new GetPlayerRangeOptions{ RangeLimit = _rangeLimit }
            );
            
            return scoresResponse.Results;
        }

        public int GetPlayerScore()
        {
            return _playerScore;
        }

        #endregion

        #region EXPERIENCE

        private async Task RefreshPlayerExp()
        {
            _playerExp = await UpdatePlayerExp();
        }
        
        public async void AddExp(int playerExp)
        {
            _playerExp += playerExp;
            var expResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(_expBoard, _playerExp);
        }

        private async Task<int> UpdatePlayerExp()
        {
            var expResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(_expBoard);
            return (int)expResponse.Score;
        }

        public int GetPlayerExp()
        {
            return _playerExp;
        }

        #endregion
    }
}