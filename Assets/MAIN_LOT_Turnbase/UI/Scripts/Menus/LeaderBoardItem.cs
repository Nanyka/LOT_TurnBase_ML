using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class LeaderBoardItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rank;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _score;

        public void UpdateItem(string rank, string name, string score)
        {
            _rank.text = rank;
            _name.text = name;
            _score.text = score;
        }
    }
}