using Unity.Services.Authentication;
using UnityEngine;

namespace JumpeeIsland
{
    public class LeaderBoard : MonoBehaviour
    {
        [SerializeField] private GameObject _board;
        [SerializeField] private LeaderBoardItem _playerItem;
        [SerializeField] private LeaderBoardItem[] _boardItems;

        public async void OnShowLeaderBoard()
        {
            foreach (var item in _boardItems)
                item.gameObject.SetActive(false);

            var range = await SavingSystemManager.Instance.GetPlayerRange();
            for (int i = 0; i < range.Count; i++)
            {
                if (range[i].PlayerId.Equals(AuthenticationService.Instance.PlayerId))
                    _playerItem.UpdateItem(range[i].Rank.ToString(),range[i].PlayerName,range[i].Score.ToString());
                
                _boardItems[i].UpdateItem(range[i].Rank.ToString(),range[i].PlayerName,range[i].Score.ToString());
                _boardItems[i].gameObject.SetActive(true);
            }
            
            _board.SetActive(true);
        }
    }
}