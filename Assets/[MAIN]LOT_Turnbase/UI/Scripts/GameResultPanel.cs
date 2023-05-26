using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LOT_Turnbase
{
    public class GameResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _gameoverPanel;
        [SerializeField] private TextMeshProUGUI _gameoverText;
    
        private void Start()
        {
            MainUI.Instance.OnGameOver.AddListener(ShowGameOverPanel);
        }

        private void ShowGameOverPanel(int winFaction)
        {
            _gameoverText.text = $"Faction {winFaction} WIN";
            _gameoverPanel.SetActive(true);
        }
    }
}
