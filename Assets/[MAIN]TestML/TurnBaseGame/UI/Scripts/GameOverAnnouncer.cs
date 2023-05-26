using System;
using System.Collections;
using System.Collections.Generic;
using LOT_Turnbase;
using TMPro;
using UnityEngine;

public class GameOverAnnouncer : MonoBehaviour
{
    [SerializeField] private GameObject _gameoverPanel;
    [SerializeField] private TextMeshProUGUI _gameoverText;
    
    private void Start()
    {
        UIManager.Instance.OnGameOver.AddListener(ShowGameOverPanel);
    }

    private void ShowGameOverPanel(int winFaction)
    {
        _gameoverText.text = $"Faction {winFaction} WIN";
        _gameoverPanel.SetActive(true);
    }
}
